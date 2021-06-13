using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace CatLua
{
    /// <summary>
    /// lua常量的模拟union
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct LuaConstantUnion
    {
        public LuaConstantUnion(LuaConstantType type, bool boolean = false, long integer = default, double number = default, string str = default)
        {
            Type = type;
            Boolean = default;
            Integer = default;
            Number = default;
            Str = default;

            switch (type)
            {
                case LuaConstantType.Boolean:
                    Boolean = boolean;
                    break;
                case LuaConstantType.Integer:
                    Integer = integer;
                    break;
                case LuaConstantType.Number:
                    Number = number;
                    break;
                case LuaConstantType.ShorStr:
                case LuaConstantType.LongStr:
                    Str = str;
                    break;
            }
        }

        [FieldOffset(0)]
        public LuaConstantType Type;

        [FieldOffset(8)]
        public bool Boolean;

        [FieldOffset(8)]
        public long Integer;

        [FieldOffset(8)]
        public double Number;

        [FieldOffset(8)]
        public string Str;



    }

}
