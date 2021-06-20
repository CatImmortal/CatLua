using System.Collections;
using System.Collections.Generic;
using System;

namespace CatLua
{
    /// <summary>
    ///比较运算符对应的函数实现
    /// </summary>
    public static class CompareOpFuncs
    {

        public static Func<LuaDataUnion, LuaDataUnion, LuaState, bool> EqFunc = Eq;

        public static Func<LuaDataUnion, LuaDataUnion, LuaState, bool> LtFunc = Lt;

        public static Func<LuaDataUnion, LuaDataUnion, LuaState, bool> LeFunc = Le;

        private static bool Eq(LuaDataUnion a, LuaDataUnion b, LuaState vm)
        {
            bool flag = a.Equals(b);

            //a b是不同的table时 尝试调用元方法
            if (!flag && a.Type == LuaDataType.Table && b.Type == LuaDataType.Table)
            {
                if (vm.TryCallMetaMethod(a,b,"__eq",out LuaDataUnion result))
                {
                    return result.ConvertToBoolean();
                }
            }

            return flag;
        }

        private static bool Lt(LuaDataUnion a, LuaDataUnion b, LuaState vm)
        {
            switch (a.Type)
            {
                case LuaDataType.Integer:
                    switch (b.Type)
                    {
                        case LuaDataType.Integer:
                            return a.Integer < b.Integer;
                        case LuaDataType.Number:
                            return a.Integer < b.Number;
                    }
                    break;
                case LuaDataType.Number:
                    switch (b.Type)
                    {
                        case LuaDataType.Integer:
                            return a.Number < b.Integer;
                        case LuaDataType.Number:
                            return a.Number < b.Number;
                    }
                    break;
                case LuaDataType.String:
                    if (b.Type == LuaDataType.String)
                    {
                        return a.Str.CompareTo(b.Str) < 0;
                    }
                    break;
              
            }

            //<比较的是数字和字符串以外的 尝试调用元方法
            if (vm.TryCallMetaMethod(a,b,"__lt",out LuaDataUnion result))
            {
                return result.ConvertToBoolean();
            }

            throw new Exception(string.Format("{0}不能与{1}进行比较",a.Type,b.Type));
        }

        private static bool Le(LuaDataUnion a, LuaDataUnion b, LuaState vm)
        {
            switch (a.Type)
            {
                case LuaDataType.Integer:
                    switch (b.Type)
                    {
                        case LuaDataType.Integer:
                            return a.Integer <= b.Integer;
                        case LuaDataType.Number:
                            return a.Integer <= b.Number;
                    }
                    break;
                case LuaDataType.Number:
                    switch (b.Type)
                    {
                        case LuaDataType.Integer:
                            return a.Number <= b.Integer;
                        case LuaDataType.Number:
                            return a.Number <= b.Number;
                    }
                    break;
                case LuaDataType.String:
                    if (b.Type == LuaDataType.String)
                    {
                        int result = a.Str.CompareTo(b.Str);
                        return result < 0 || result == 0 ;
                    }
                    break;

            }

            //<=比较的是数字和字符串以外的 尝试调用元方法
            //找不到__le会去尝试找__lt
            //因为a <= b 等价于not (b < a)
            if (vm.TryCallMetaMethod(a,b,"__le",out LuaDataUnion data))
            {
                return data.ConvertToBoolean();
            }
            else if (vm.TryCallMetaMethod(b, a, "__lt", out data))
            {
                return !data.ConvertToBoolean();
            }

            throw new Exception(string.Format("{0}不能与{1}进行比较", a.Type, b.Type));
        }
    }

}

