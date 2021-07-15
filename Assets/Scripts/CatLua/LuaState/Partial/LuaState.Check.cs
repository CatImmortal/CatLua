using System;
using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    public partial class LuaState
    {
        /// <summary>
        /// 栈中index位置的值是否是Nil
        /// </summary>
        public bool IsNil(int index)
        {
            LuaDataUnion value = globalStack.Get(index);
            return  value.Type == LuaDataType.Nil;
        }

        /// <summary>
        /// 栈中index位置的值是否是string或可以将值转换为stirng
        /// </summary>

        public bool IsString(int index)
        {
            LuaDataUnion value = globalStack.Get(index);

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

        /// <summary>
        /// 检查index位置值的是否为number，若为number就返回该number值
        /// </summary>
        public double CheckNumber(int index)
        {
            bool b = TryGetNumber(index, out double d);
            if (!b)
            {
                throw new Exception("参数类型不正确，需求Number");
            }
            return d;
        }
    }
}