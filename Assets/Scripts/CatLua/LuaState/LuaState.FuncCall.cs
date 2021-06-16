using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
namespace CatLua
{
    public partial class LuaState
    {
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
        /// 当前栈帧的最大寄存器数量
        /// </summary>
        public int CurFrameRegsiterCount
        {
            get
            {
                return curFrame.Closure.Proto.MaxStackSize;
            }
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
        public int PushFuncAndArgs(int a, int argsNum)
        {
            if (argsNum >= 0)
            {
                for (int i = a; i <= a + (argsNum - 1); i++)
                {
                    CopyAndPush(i);

                }

                return argsNum;
            }
            else
            {
                //b为0 表示参数个数不确定 可能由另一个函数的返回值决定 也可能是变长参数
                //比如 f(1, 2, g())

                //为f压入函数和参数时，g的返回值和返回值起始位置值全部在栈顶
                //之后将f和1 2压入栈顶，g的返回值在栈底，这时需要旋转栈
                //使得最终f 1 2在栈底，g的返回值在栈顶

                FixStack(a);

                //int num = Top - CurFrameRegsiterCount- 1;

                int num = Top - a;
                return num;
            }
        }

        /// <summary>
        /// 弹出栈顶的返回值并复制到以a位置开始的寄存器里
        /// </summary>
        public void PopResults(int a, int resultNum)
        {
            if (resultNum == 0)
            {
                return;
            }

            if (resultNum >= 1)
            {

                //弹出栈顶的返回值，并复制到从(a - 1 ) + resultNum  到 a 的部分
                for (int i = (a - 1) + resultNum; i >= a; i--)
                {
                    PopAndCopy(i);
                }

            }
            else
            {
                //resultNum为-1
                //返回值需要全部留在栈顶
                //将这些返回值原本的起始位置压入栈顶
                Push(a);
            }
        }


        /// <summary>
        /// 修复栈里的参数排列
        /// </summary>
        public void FixStack(int a)
        {
            //获取栈顶值
            //g函数的返回值的起始位置
            int x = (int)GetInteger(-1);

            //弹出栈顶值
            Pop(1);

            for (int i = a; i < x; i++)
            {
                //将a到x-1位置的值 即f函数和前半部分参数的值 复制并压入栈顶

                CopyAndPush(i);
            }

            //向下旋转栈 让f函数和前半部分的参数移动到栈底，后半部分的参数移动到栈顶 旋转次数是前半部分参数的个数
            Rotate(curFrame.Closure.Proto.MaxStackSize + 1, x - a);
        }


        /// <summary>
        /// 加载一段字节码chunk，将主函数原型实例化为闭包，压入栈顶
        /// mode表示加载模式，"b"=加载二进制chunk，"t"=加载文本chunk，"pt"=两者都可
        /// </summary>
        public int Load(byte[] bytes,string chunkName,string mode)
        {
            Chunk chunk = Chunk.Undump(bytes);
            Closure c = new Closure(chunk.MainFunc);
            Push(c);
            return 0;
        }

        /// <summary>
        /// 调用函数
        /// </summary>
        public void Call(int argsNum,int resultNum)
        {
            //从栈顶倒数(argsNum-1)的位置获取到要调用的函数
            LuaDataUnion data = globalStack.Get(-(argsNum + 1));
            if (data.Type != LuaDataType.Function)
            {
                throw new Exception("Call要调用的数据不是函数");
            }

            Debug.Log(string.Format("<color=#66ccff>调用函数，{0}<{1},{2}></color>", data.Closure.Proto.Source, data.Closure.Proto.LineDefined, data.Closure.Proto.LastLineDefined));

            Call(argsNum, resultNum, data.Closure);
        }

        /// <summary>
        /// 调用函数
        /// </summary>
        private void Call(int argsNum, int resultNum,Closure c)
        {
            //从当前栈帧弹出函数与参数
            LuaDataUnion[] FuncAndParams = globalStack.PopN(argsNum + 1);

            int registerNum = c.Proto.MaxStackSize;
            int paramsNum = c.Proto.NumParams;
            bool isVarArg = c.Proto.IsVararg != 0;

            //为要调用的函数创建调用栈帧
            FuncCallFrame newFrame = new FuncCallFrame();
            newFrame.Closure = c;
            newFrame.Bottom = Top + 1;

            //将参数压入新的栈帧范围内
            globalStack.PushN(FuncAndParams, 1, paramsNum);

            //修改栈顶指针 指向最后一个寄存器
            globalStack.Top = newFrame.Bottom + (registerNum - 1);

            if (argsNum > paramsNum && isVarArg)
            {
                //实际参数多于期望参数 并且这个函数有变长参数
                //就把多出来的参数放入变长参数里处理
                LuaDataUnion[] varArgs = new LuaDataUnion[FuncAndParams.Length - (paramsNum + 1)];
                Array.Copy(FuncAndParams, paramsNum + 1, varArgs, 0, varArgs.Length);
                newFrame.VarArgs = varArgs;
            }

            //压入新栈帧 调用函数 弹出新栈帧
            PushFuncCallFrame(newFrame);
            RunLuaClosure();
            PopFuncCallFrame();

            //处理返回值
            if (resultNum != 0)
            {
                ////返回值在新栈帧的registerNum + 1 到 Top的部分
                //LuaDataUnion[] datas = newStack.PopN(newStack.Top - registerNum);

                ////将resultNum个返回值压入前一个栈帧中
                //globalStack.PushN(datas,0,resultNum);

                //丢弃多余的返回值 或者用nil填补不足的返回值
                SetTop(newFrame.Bottom + (resultNum - 1));
            }
        }

        /// <summary>
        /// 运行当前栈帧的主函数
        /// </summary>
        private void RunLuaClosure()
        {
            while (true)
            {
                //不断取出指令执行 直到遇到return
                Instructoin i = new Instructoin(Fetch());
                i.Execute(this);
                Debug.Log(string.Format("[{0}] {1} {2}", curFrame.PC, i.OpType, this));
                if (i.OpType == OpCodeType.Return)
                {
                    break;
                }
            }
        }
    }
}



