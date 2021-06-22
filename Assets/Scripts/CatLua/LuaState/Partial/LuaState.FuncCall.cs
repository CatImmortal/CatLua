﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
namespace CatLua
{
    public partial class LuaState
    {
        /// <summary>
        /// 当前被调栈帧的函数，实际返回的返回值数量
        /// </summary>
        public int CallFrameReturnResultNum;

        /// <summary>
        /// 当前栈帧的栈底索引
        /// </summary>
        public int CurFrameBottom
        {
            get
            {
                return curFrame.Bottom;
            }
        }

        /// <summary>
        /// 当前栈帧的变长参数数量
        /// </summary>
        public int CurFrameVarArgsNum
        {
            get
            {
                return curFrame.VarArgs.Length;
            }
        }

        /// <summary>
        /// 当前栈帧的非预留寄存器区域的大小（预留区域最大索引到top的那部分的长度）
        /// </summary>
        public int CurFrameNonReserveRegisterSize
        {
            get
            {
                return curFrame.GetNonReserveRegisterSize(Top);
            }
        }



        /// <summary>
        /// 加载一段字节码chunk，将主函数原型实例化为闭包，压入栈顶
        /// mode表示加载模式，"b"=加载二进制chunk，"t"=加载文本chunk，"pt"=两者都可
        /// </summary>
        public int LoadChunk(byte[] bytes, string chunkName, string mode)
        {
            Chunk chunk = Chunk.Undump(bytes);
            Closure c = new Closure(chunk.MainFunc);
            Push(c);

            if (c.Proto.UpvalueInfos.Length > 0)
            {
                //设置_G到入口函数的upvalue中
                LuaDataUnion g = registry[Constants.GlobalEnvKey];
                c.Upvalues[0] = new Upvalue(g);
            }

            return 0;
        }

        /// <summary>
        /// 压入函数调用栈帧
        /// </summary>
        public void PushFuncCallFrame(FuncCallFrame frame)
        {
            frame.Prev = curFrame;
            curFrame = frame;
        }

        /// <summary>
        /// 弹出函数调用栈帧
        /// </summary>
        public void PopFuncCallFrame()
        {
            FuncCallFrame frame = curFrame;
            curFrame = curFrame.Prev;
            frame.Prev = null;
        }


        /// <summary>
        /// 将a位置开始的函数与参数压入栈顶，返回参数数量
        /// </summary>
        public int PushFuncAndArgs(int a, int b)
        {
            if (b == 0)
            {
                //b为0  需要压入从a到top的所有值
                b = Top - a + 1;
            }


            for (int i = 0; i < b; i++)
            {
                CopyAndPush(a + i);
            }

            return b - 1;

        }

        /// <summary>
        /// 弹出栈顶来自被调栈帧的返回值并复制到以a位置开始的寄存器里
        /// </summary>
        public void PopResults(int a, int resultNum)
        {
            if (resultNum == 0)
            {
                return;
            }

            if (resultNum == -1)
            {
                //需要弹出所有返回值
                resultNum = CallFrameReturnResultNum;
            }

            //弹出resultNum个返回值
            LuaDataUnion[] datas = PopN(resultNum);

            int targetIndex = (a - 1) + resultNum;
            if (targetIndex > Top)
            {
                //扩充栈顶
                SetTop(targetIndex);
            }

            //复制到a开始的部分
            for (int index = 0; index < datas.Length; index++)
            {
                Push(datas[index]);
                PopAndCopy(a + index);
            }

        }

        /// <summary>
        /// 获取当前栈帧upvalue表中index位置的upvalue值
        /// </summary>
        public Upvalue GetCurFrameUpvalue(int index)
        {
            return curFrame.Closure.Upvalues[index];
        }

        /// <summary>
        /// 将栈中globalStackIndex位置开始的所有开放状态的upvalue关闭
        /// </summary>
        public void CloseUpvalue(int globalStackIndex)
        {
            List<int> needDeleteKey = new List<int>();
            foreach (KeyValuePair<int, Upvalue> item in openUpvalues)
            {
                if (item.Key >= globalStackIndex)
                {
                    needDeleteKey.Add(item.Key);

                    item.Value.IsOpen = false;
                }
            }

            foreach (int key in needDeleteKey)
            {
                openUpvalues.Remove(key);
            }
        }

        /// <summary>
        /// 调用函数
        /// </summary>
        public void CallFunc(int argsNum,int resultNum)
        {
            LuaDataUnion data = globalStack.Get(-(argsNum + 1));
            if (data.Type != LuaDataType.Function)
            {
                //不是函数 尝试调用__call元方法
                LuaTable mt = GetMetaTable(data);
                LuaDataUnion mtData = mt["__call"];
                if (mt != null && mtData.Type == LuaDataType.Function)
                {
                    //以被调用的data为第一个参数，后续跟其他参数

                    //插入到原本要调用的函数和参数前面 后续会调用这个__call关联的函数
                    Push(mtData);
                    PopAndInsert(-(argsNum + 2));  

                    argsNum++;
                    data = mtData;  //这里将原本要调用的换成__call
                }
                else
                {
                    throw new Exception("调用函数失败:" + this);
                }
            }

          

            if (data.Closure.CSFunc == null)
            {

                //调用Lua函数
                Closure c = data.Closure;
                Debug.Log(string.Format("<color=#66ccff>调用Lua函数，函数和参数压入栈顶</color>：{0}<{1}-{2}>,{3}:", c.Proto.Source, c.Proto.LineDefined, c.Proto.LastLineDefined,this));

                PreLuaFuncCall(argsNum);
                ExcuteLuaFuncCall();
                PostLuaFuncCall(resultNum);
            }
            else
            {
                //调用C#函数
                Debug.Log(string.Format("<color=#66ccff>调用C#函数，函数和参数压入栈顶</color>:{0}", this));
                CSFuncCall(argsNum,resultNum);
            }

           
            Debug.Log(string.Format("<color=#66ccff>函数调用结束，返回值压入栈顶</color>:{0}", this));
        }

        /// <summary>
        /// 为Lua函数调用作准备，将被调函数和参数压入新栈帧内
        /// </summary>
        private void PreLuaFuncCall(int argsNum)
        {
            //从主调栈帧的栈顶弹出函数与参数
            LuaDataUnion[] FuncAndParams = globalStack.PopN(argsNum + 1);

            Closure c = FuncAndParams[0].Closure;
           
            int paramsNum = c.Proto.NumParams;
            bool isVarArg = c.Proto.IsVararg != 0;

            //为要调用的函数创建栈帧

            //设置被调栈帧的栈底 在之前的栈帧之上 用max函数保证不会和之前的栈帧数据重叠
            //如果之前的栈帧里保存的数据没超过预留寄存器数量，就设为curFrame.ReserveRegisterMaxIndex + 1，否则设为Top + 1
            int bottom = Math.Max(curFrame.ReserveRegisterMaxIndex + 1, Top + 1);

            FuncCallFrame newFrame = new FuncCallFrame(c,bottom);

            //压入被调栈帧
            PushFuncCallFrame(newFrame);

            //修改栈顶，指向被调栈帧的Bottom - 1，这样接下来push参数就是从Bottom位置开始了
            SetTop(newFrame.Bottom - 1);

            //将固定参数压入被调栈帧
            globalStack.PushN(FuncAndParams, 1, paramsNum);

            //再次修改栈顶 指向最后一个寄存器
            //这样后续push新值不会占用到栈帧自己预留的寄存器位置
            SetTop(newFrame.ReserveRegisterMaxIndex);

            if (argsNum > paramsNum && isVarArg)
            {
                //实际参数多于固定参数 并且这个函数有变长参数
                //就把多出来的参数放入变长参数里处理
                LuaDataUnion[] varArgs = new LuaDataUnion[argsNum - paramsNum];
                Array.Copy(FuncAndParams, paramsNum + 1, varArgs, 0, varArgs.Length);
                newFrame.VarArgs = varArgs;
            }

        }

        /// <summary>
        /// 执行Lua函数调用
        /// </summary>
        private void ExcuteLuaFuncCall()
        {
            while (true)
            {
                //不断取出指令执行 直到遇到return指令
                Instructoin i = new Instructoin(Fetch());
                Debug.Log(string.Format("指令执行前：[{0}] {1} {2}", curFrame.PC, i.OpType, this));
                i.Execute(this);
                Debug.Log(string.Format("指令执行后：[{0}] {1} {2}", curFrame.PC, i.OpType, this));
                if (i.OpType == OpCodeType.Return)
                {
                   //此时已经将返回值准备在栈顶上了
                    break;
                }
            }
        }

        /// <summary>
        /// Lua函数调用完成后，将被调函数栈帧的栈顶返回值压入主调函数栈帧的栈顶
        /// </summary>
        private void PostLuaFuncCall(int resultNum)
        {

            //闭合此栈帧上的局部变量所构造的Upvalue
            CloseUpvalue(CurFrameBottom);

            FuncCallFrame frame = curFrame;

            //弹出栈帧
            PopFuncCallFrame();

            //处理返回值
            if (resultNum != 0)
            {

                //取出返回值
                LuaDataUnion[] results = globalStack.PopN(CallFrameReturnResultNum) ;

                //恢复栈顶指针到主调栈帧的顶部，被调栈帧剩余未被取出的数据就不要了
                SetTop(frame.Bottom - 1);

                //压入resultNum个返回值到主调栈帧上，不足的部分用nil补
                globalStack.PushN(results, 0, resultNum);
            }
            else
            {
                //恢复栈顶指针到主调栈帧的顶部
                SetTop(frame.Bottom - 1);
            }

        }

        /// <summary>
        /// C#函数调用
        /// </summary>
        private void CSFuncCall(int argsNum, int resultNum)
        {
         
            LuaDataUnion[] FuncAndParams = globalStack.PopN(argsNum + 1);

            Closure c = FuncAndParams[0].Closure;

            int bottom = Math.Max(curFrame.ReserveRegisterMaxIndex + 1, Top + 1);
            FuncCallFrame newFrame = new FuncCallFrame(c,bottom);

            PushFuncCallFrame(newFrame);

            SetTop(newFrame.Bottom - 1);
            globalStack.PushN(FuncAndParams, 1, argsNum);

            int r = c.CSFunc(this);


            //闭合此栈帧上的局部变量所构造的Upvalue
            CloseUpvalue(CurFrameBottom);

            PopFuncCallFrame();

            if (resultNum != 0)
            {
                //取出r个返回值
                LuaDataUnion[] results = globalStack.PopN(r);

                SetTop(newFrame.Bottom - 1);

                globalStack.PushN(results, 0, resultNum);
            }
            else
            {
                SetTop(newFrame.Bottom - 1);
            }
        }

    }
}



