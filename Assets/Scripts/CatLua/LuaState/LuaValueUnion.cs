using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CatLua
{
    /// <summary>
    /// Lua值的模拟Union
    /// </summary>

    [StructLayout(LayoutKind.Explicit)]
    public struct LuaValueUnion
    {
        public LuaValueUnion(LuaDataType type, byte nil = 0, bool boolean = false, long integer = 0, double number = 0, string str = null)
        {
            Type = type;

            Nil = nil;
            Boolean = boolean;
            Integer = integer;
            Number = number;
            Str = str;

            switch (type)
            {
                case LuaDataType.Boolean:
                    Boolean = boolean;
                    break;
                case LuaDataType.Integer:
                    Integer = integer;
                    break;
                case LuaDataType.Number:
                    Number = number;
                    break;
                case LuaDataType.String:
                    Str = str;
                    break;
            }
        }

        [FieldOffset(0)]
        public LuaDataType Type;

        [FieldOffset(8)]
        public byte Nil;

        [FieldOffset(8)]
        public bool Boolean;

        [FieldOffset(8)]
        public long Integer;

        [FieldOffset(8)]
        public double Number;

        [FieldOffset(8)]
        public string Str;

        public override string ToString()
        {
            string s;
            switch (Type)
            {

                case LuaDataType.Boolean:
                    s = Boolean.ToString();
                    break;
                case LuaDataType.Integer:
                    s = Integer.ToString();
                    break;
                case LuaDataType.Number:
                    s = Number.ToString();
                    break;
                case LuaDataType.String:
                    s = Str;
                    break;
                default:
                    s = Type.ToString();
                    break;
            }
            return s;
        }

       
    }
}

