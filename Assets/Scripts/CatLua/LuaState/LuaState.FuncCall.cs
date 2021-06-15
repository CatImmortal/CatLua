using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
namespace CatLua
{
    public partial class LuaState
    {
        /// <summary>
        /// 压入函数调用栈帧
        /// </summary>
        public void PushLuaStack(LuaStack stack)
        {
            stack.Prev = CurStack;
            CurStack = stack;
        }

        /// <summary>
        /// 弹出函数调用栈帧
        /// </summary>
        public void PopLuaStack()
        {
            LuaStack stack = CurStack;
            CurStack = CurStack.Prev;
            stack.Prev = null;
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
        /// resultNum为-1时，会把所有返回值留在栈顶
        /// </summary>
        public void Call(int argsNum,int resultNum)
        {
            //从栈顶倒数(argsNum-1)的位置获取到要调用的函数
            LuaDataUnion data = CurStack.Get(-(argsNum + 1));
            if (data.Type != LuaDataType.Function)
            {
                throw new Exception("Call要调用的数据不是函数");
            }

            Debug.Log(string.Format("调用函数，{0}<{1},{2}>", data.Closure.Proto.Source, data.Closure.Proto.LineDefined, data.Closure.Proto.LastLineDefined));

        }

        /// <summary>
        /// 调用函数
        /// </summary>
        private void Call(int argsNum, int resultNum,Closure c)
        {
            int registerNum = c.Proto.MaxStackSize;
            int paramsNum = c.Proto.NumParams;
            bool isVarArg = c.Proto.IsVararg != 0;

            //为要调用的函数创建调用栈帧
            LuaStack newStack = new LuaStack(registerNum + 20);
            newStack.Closure = c;

            //从当前栈帧弹出函数与参数
            LuaDataUnion[] FuncAndParams = CurStack.PopN(argsNum + 1);

            //将参数压入新的栈帧
            newStack.PushN(FuncAndParams, 1, paramsNum);

            //修改栈顶指针 指向最后一个寄存器
            newStack.Top = registerNum;

            if (argsNum > paramsNum && isVarArg)
            {
                //实际参数多于期望参数 并且这个函数有变长参数
                //就把多出来的参数放入变长参数里处理
                LuaDataUnion[] varArgs = new LuaDataUnion[FuncAndParams.Length - (paramsNum + 1)];
                Array.Copy(FuncAndParams, paramsNum + 1, varArgs, 0, varArgs.Length);
                newStack.VarArgs = varArgs;
            }

            PushLuaStack(newStack);
            RunLuaClosure();
            PopLuaStack();

            //处理返回值
            if (resultNum != 0)
            {
                //返回值会被留在栈顶（寄存器上面）
                LuaDataUnion[] datas = newStack.PopN(newStack.Top - registerNum);

                //将返回值压入
                CurStack.PushN(datas);
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
                if (i.OpType == OpCodeType.Return)
                {
                    break;
                }
            }
        }
    }
}



