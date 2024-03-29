﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// Lua解释器核心
    /// </summary>
    public partial class LuaState
    {
        public LuaState(int size)
        {
            globalStack = new LuaStack(size);

            //将全局环境表放入注册表
            registry[Constants.GlobalEnvKey] = Factory.NewTable(new LuaTable());

            //开启所有标准库
            OpenStdLibs();
        }

        /// <summary>
        /// 全局Lua虚拟栈
        /// </summary>
        private LuaStack globalStack;


        /// <summary>
        /// 全局Lua注册表
        /// </summary>
        private LuaTable registry = new LuaTable();


        /// <summary>
        /// 加载Lua源码
        /// </summary>
        public int LoadSourceCode(string SourceCode,string chunkName)
        {
            FuncPrototype mainFunc = Compiler.Compile(SourceCode, chunkName);
            LoadMainFunc(mainFunc);
            return 0;
        }

        /// <summary>
        /// 加载Lua字节码
        /// </summary>
        public int LoadChunk(byte[] bytes, string chunkName)
        {
            Chunk chunk = Chunk.Undump(bytes);
            LoadMainFunc(chunk.MainFunc);
            return 0;
        }

        /// <summary>
        /// 加载主函数原型,将其实例化为闭包，压入栈顶
        /// </summary>
        private void LoadMainFunc(FuncPrototype mainFunc)
        {
            Closure c = new Closure(mainFunc);
            Push(c);

            if (c.Proto.UpvalueInfos.Length > 0)
            {
                //设置全局环境表到入口函数的upvalue中
                LuaDataUnion g = registry[Constants.GlobalEnvKey];
                c.Upvalues[0] = new Upvalue(g);
            }

        }

        public override string ToString()
        {
          
            FuncCallFrame frame = curFrame;
            Stack<int> bottomStacks = new Stack<int>();
            Stack<int> registerAreaStacks = new Stack<int>();
            while (frame.Prev != null)
            {
                bottomStacks.Push(frame.Bottom);
                registerAreaStacks.Push(frame.ReserveRegisterMaxIndex);
                frame = frame.Prev;
            }

            string s = string.Empty;

            for (int i = 0; i <= globalStack.Top; i++)
            {
                if (bottomStacks.Count != 0 && i == bottomStacks.Peek())
                {
                    bottomStacks.Pop();

                    //栈帧分割符
                    s += "<color=#ff0000>|</color>";
                }

                LuaDataUnion value = globalStack.Get(i);
                s += $"[{value}]";

                if (registerAreaStacks.Count != 0 && i == registerAreaStacks.Peek())
                {
                    registerAreaStacks.Pop();

                    //寄存器预留区域分割符
                    s += "<color=#00ff00>|</color>";
                }
            }
            return s;
        }


    }
}

