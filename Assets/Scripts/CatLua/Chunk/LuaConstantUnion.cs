﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace CatLua
{
    /// <summary>
    /// lua常量的模拟union
    /// </summary>
    public class LuaConstantUnion
    {
        public LuaConstantUnion(LuaConstantType type, bool boolean = default, long integer = default, double number = default, string str = default)
        {
            Type = type;

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
                case LuaConstantType.ShortStr:
                case LuaConstantType.LongStr:
                    Str = str;
                    break;
            }
        }

        public LuaConstantType Type;

        public bool Boolean;

        public long Integer;


        public double Number;

        public string Str;

        public override string ToString()
        {
            string s;
            switch (Type)
            {

                case LuaConstantType.Boolean:
                    s = Boolean.ToString();
                    break;
                case LuaConstantType.Integer:
                    s = Integer.ToString();
                    break;
                case LuaConstantType.Number:
                    s = Number.ToString();
                    break;
                case LuaConstantType.ShortStr:
                case LuaConstantType.LongStr:
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
