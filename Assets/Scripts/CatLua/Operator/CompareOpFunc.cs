using System.Collections;
using System.Collections.Generic;
using System;

namespace CatLua
{
    /// <summary>
    ///比较运算符对应的函数实现
    /// </summary>
    public static class CompareOpFunc
    {

        public static Func<LuaDataUnion, LuaDataUnion, bool> Eq = EqMethod;

        public static Func<LuaDataUnion, LuaDataUnion, bool> Lt = LtMethod;

        public static Func<LuaDataUnion, LuaDataUnion, bool> Le = LeMethod;

        private static bool EqMethod(LuaDataUnion a, LuaDataUnion b)
        {
            return a.Equals(b);
            
        }

        private static bool LtMethod(LuaDataUnion a, LuaDataUnion b)
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

            throw new Exception(string.Format("{0}不能与{1}进行比较",a.Type,b.Type));
        }

        private static bool LeMethod(LuaDataUnion a, LuaDataUnion b)
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

            throw new Exception(string.Format("{0}不能与{1}进行比较", a.Type, b.Type));
        }
    }

}

