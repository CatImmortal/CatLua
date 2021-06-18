using System;
using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// Lua解释器状态
    /// </summary>
    public partial class LuaState
    {
        public LuaState(int size)
        {
            globalStack = new LuaStack(size);
            curFrame = new FuncCallFrame();

            registry = new LuaTable();

            //将全局环境表_G放入注册表
            registry[Constants.GlobalEnvIndex] = new LuaDataUnion(LuaDataType.Table, table: new LuaTable());
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

        public override string ToString()
        {
            string s = "";
            FuncCallFrame frame = curFrame;
            Stack<int> bottomStacks = new Stack<int>();
            Stack<int> registerAreaStacks = new Stack<int>();
            while (frame.Prev != null)
            {
                bottomStacks.Push(frame.Bottom);
                registerAreaStacks.Push(frame.ReserveRegisterMaxIndex);
                frame = frame.Prev;
            }

            for (int i = 0; i <= globalStack.Top; i++)
            {
                if (bottomStacks.Count != 0 && i == bottomStacks.Peek())
                {
                    bottomStacks.Pop();

                    //栈帧分割符
                    s += "<color=#ff0000>|</color>";
                }

                LuaDataUnion value = globalStack.Get(i);
                switch (value.Type)
                {
                  
                    case LuaDataType.Boolean:
                        s += string.Format("[{0}]", GetBoolean(i));
                        break;
                  
                    case LuaDataType.Integer:
                        s += string.Format("[{0}]", GetInteger(i));
                        break;
                    case LuaDataType.Number:
                        s += string.Format("[{0}]", GetNumber(i));
                        break;
                    case LuaDataType.String:
                        s += string.Format("[\"{0}\"]", GetString(i));
                        break;
                    default:
                        s += string.Format("[{0}]", value.Type.ToString());
                        break;

                }

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

