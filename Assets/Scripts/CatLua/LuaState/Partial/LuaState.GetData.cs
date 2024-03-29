﻿using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    public partial class LuaState
    {
        /// <summary>
        /// 获取栈中index位置的值的类型
        /// </summary>
        public LuaDataType GetType(int index)
        {
            if (!globalStack.IsValid(index))
            {
                return LuaDataType.None;
            }
            return globalStack.Get(index).Type;
        }

        /// <summary>
        /// 从栈中index位置取出一个bool值，如果不是bool类型则进行类型转换
        /// </summary>
        public bool GetBoolean(int index)
        {
            LuaDataUnion value = globalStack.Get(index);
            return value.ConvertToBoolean();
        }

        /// <summary>
        /// 从栈中index位置取出一个number，如果无法转换会返回0
        /// </summary>
        public double GetNumber(int index)
        {
            double d;
            TryGetNumber(index, out d);
            return d;
        }

        /// <summary>
        /// 从栈中index位置取出一个number，如果无法转换会返回false
        /// </summary>
        public bool TryGetNumber(int index, out double d)
        {
            LuaDataUnion value = globalStack.Get(index);
            bool result = value.TryConvertToNumber(out d);
            return result;
        }

        /// <summary>
        /// 从栈中index位置取出一个integer，如果无法转换会返回0
        /// </summary>
        public long GetInteger(int index)
        {
            long l;
            TryGetInteger(index, out l);
            return l;
        }

        /// <summary>
        /// 从栈中index位置取出一个integer，如果无法转换会返回false
        /// </summary>
        public bool TryGetInteger(int index, out long l)
        {
            LuaDataUnion value = globalStack.Get(index);
            bool result = value.TryConvertToInteger(out l);
            return result;
        }

        /// <summary>
        /// 从栈中index位置取出一个string，如果是数字，会将值转换为字符串，并修改栈
        /// </summary>
        public string GetString(int index)
        {
            LuaDataUnion data = globalStack.Get(index);
            string str = string.Empty;
            switch (data.Type)
            {

                case LuaDataType.Integer:
                    str = data.Integer.ToString();
                    globalStack.Set(index, Factory.NewString(str));
                    break;

                case LuaDataType.Number:
                    str = data.Number.ToString();
                    globalStack.Set(index, Factory.NewString(str));
                    break;

                case LuaDataType.String:
                    str = data.Str;
                    break;
            }

            return str;
        }
    }
}