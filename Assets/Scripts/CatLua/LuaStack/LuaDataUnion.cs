using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CatLua
{
    /// <summary>
    /// Lua数据的模拟Union
    /// </summary>


    public struct LuaDataUnion : IEqualityComparer<LuaDataUnion>
    {
        public LuaDataUnion(LuaDataType type, bool boolean = default, long integer = default, double number = default, string str = default, LuaTable table = default, Closure closure = default)
        {
            Type = type;

            Boolean = default;
            Integer = default;
            Number = default;
            Str = default;
            Table = default;
            Closure = default;

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
                case LuaDataType.Table:
                    Table = table;
                    break;
                case LuaDataType.Function:
                    Closure = closure;
                    break;
                    
            }
        }


        public LuaDataType Type;


        public bool Boolean;


        public long Integer;


        public double Number;


        public string Str;


        public LuaTable Table;


        public Closure Closure;

        /// <summary>
        /// 转换为bool值
        /// </summary>
        public bool ConvertToBoolean()
        {
            //Lua中只有nil和false表示假，其他都为真

            switch (Type)
            {
                case LuaDataType.Nil:
                    return false;

                case LuaDataType.Boolean:
                    return Boolean;

                default:
                    return true;

            }
        }

        /// <summary>
        /// 尝试转换到number
        /// </summary>
        public bool TryConvertToNumber(out double d)
        {
            switch (Type)
            {
               
                case LuaDataType.Integer:
                    d = Integer;
                    return true;
          
                case LuaDataType.Number:
                    d = Number;
                    return true;

                case LuaDataType.String:
                   bool result = double.TryParse(Str, out d);
                   return result;
                default:
                    d = 0;
                    return false;
            }
        }

        /// <summary>
        /// 尝试转换到integer
        /// </summary>
        public bool TryConvertToInteger(out long l)
        {
            switch (Type)
            {

                case LuaDataType.Integer:
                    l = Integer;
                    return true;

                case LuaDataType.Number:
                    bool result = LMath.TryNumberToInteger(Number,out l);
                    return result;

                case LuaDataType.String:

                    result = long.TryParse(Str, out l);

                    if (!result)
                    {
                        //不能转long 可能是"3.0"这种字符串，需要转double试试
                        double d;
                        result = double.TryParse(Str, out d);

                        if (result)
                        {
                            //可以转double，尝试转long
                            result = LMath.TryNumberToInteger(d, out l);
                        }
                        
                    }
                    return result;

                default:
                    l = 0;
                    return false;
            }
        }


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

        public bool Equals(LuaDataUnion x, LuaDataUnion y)
        {
            bool result = false;

            switch (x.Type)
            {
                case LuaDataType.Nil:
                    result = y.Type == LuaDataType.Nil;
                    break;
                case LuaDataType.Boolean:
                    result = x.Boolean == y.Boolean;
                    break;
                case LuaDataType.Integer:

                    switch (y.Type)
                    {
                       
                        case LuaDataType.Integer:
                            result = x.Integer == y.Integer;
                            break;
                        case LuaDataType.Number:
                            result = x.Integer == y.Number;
                            break;
                        default:
                            result = false;
                            break;
                    }

                    break;
                case LuaDataType.Number:

                    switch (y.Type)
                    {

                        case LuaDataType.Integer:
                            result = x.Number == y.Integer;
                            break;
                        case LuaDataType.Number:
                            result = x.Number == y.Number;
                            break;
                        default:
                            result = false;
                            break;
                    }

                    break;
                case LuaDataType.String:
                    result = y.Type == LuaDataType.String && x.Str == y.Str;
                    break;

                case LuaDataType.Table:
                    result = Table.Equals(Table);
                    break;

                case LuaDataType.Function:
                    result = Closure.Equals(Closure);
                    break;


            }

            return result;
        }

        public int GetHashCode(LuaDataUnion obj)
        {
            int hashCode = 0;

            switch (obj.Type)
            {
                
                case LuaDataType.Boolean:
                    hashCode = Boolean.GetHashCode();
                    break;
                case LuaDataType.Integer:
                    hashCode = Integer.GetHashCode();
                    break;
                case LuaDataType.Number:
                    hashCode = Number.GetHashCode();
                    break;
                case LuaDataType.String:
                    hashCode = Str.GetHashCode();
                    break;
                case LuaDataType.Table:
                    hashCode = Table.GetHashCode();
                    break;
                case LuaDataType.Function:
                    hashCode = Closure.GetHashCode();
                    break;
            }

            return hashCode;
        }
    }
}

