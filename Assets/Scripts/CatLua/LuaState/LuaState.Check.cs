﻿using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    public partial class LuaState
    {
        /// <summary>
        /// 栈中index位置的值是否是None或Nil
        /// </summary>
        public bool IsNoneOrNil(int index)
        {
            LuaDataUnion value = CurStack.Get(index);
            return value.Type == LuaDataType.None || value.Type == LuaDataType.Nil;
        }

        /// <summary>
        /// 栈中index位置的值是否是string或可以将值转换为stirng
        /// </summary>

        public bool IsString(int index)
        {
            LuaDataUnion value = CurStack.Get(index);

            return value.Type == LuaDataType.String
                || value.Type == LuaDataType.Number
                || value.Type == LuaDataType.Integer;
        }

        /// <summary>
        /// 栈中index位置的值是否是number或可以转换为number
        /// </summary>
        public bool IsNumber(int index)
        {
            double d;
            bool result = TryGetNumber(index, out d);
            return result;
        }
    }
}