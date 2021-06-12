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
        public LuaState()
        {
            stack = new LuaStack(20);
        }

        private LuaStack stack;


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

