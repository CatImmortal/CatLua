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
        public LuaState(int size, FuncPrototype proto)
        {
            stack = new LuaStack(size);
            this.proto = proto;
            PC = 0;
        }

        /// <summary>
        /// Lua虚拟栈
        /// </summary>
        private LuaStack stack;

        /// <summary>
        /// 函数原型
        /// </summary>
        private FuncPrototype proto;

        public override string ToString()
        {
            string s = "";
            for (int i = 1; i <= stack.Top; i++)
            {
                LuaDataUnion value = stack.Get(i);
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
            }
            return s;
        }


    }
}

