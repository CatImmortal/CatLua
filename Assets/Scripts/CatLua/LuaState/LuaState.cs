using System;
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

            curFrame = new FuncCallFrame();

            registry = new LuaTable();

            //将全局环境表_G放入注册表
            registry[Constants.GlobalEnvIndex] = Factory.NewTable(new LuaTable());

            openUpvalues = new Dictionary<int, Upvalue>();
        }

        /// <summary>
        /// 全局Lua虚拟栈
        /// </summary>
        private LuaStack globalStack;

        /// <summary>
        /// 当前函数调用栈帧
        /// </summary>
        private FuncCallFrame curFrame;

        /// <summary>
        /// 全局Lua注册表
        /// </summary>
        private LuaTable registry;

        /// <summary>
        /// 开放状态的upvalue
        /// </summary>
        private Dictionary<int, Upvalue> openUpvalues;

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

