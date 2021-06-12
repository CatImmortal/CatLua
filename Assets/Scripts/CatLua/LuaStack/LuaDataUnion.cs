using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CatLua
{
    /// <summary>
    /// Lua数据的模拟Union
    /// </summary>

    [StructLayout(LayoutKind.Explicit)]
    public struct LuaDataUnion
    {
        public LuaDataUnion(LuaDataType type, byte nil = 0, bool boolean = false, long integer = 0, double number = 0, string str = null)
        {
            Type = type;

            Nil = default;
            Boolean = default;
            Integer = default;
            Number = default;
            Str = default;

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
        public override int GetHashCode()
        {
            switch (Type)
            {
                case LuaDataType.None:
                case LuaDataType.Nil:
                    return 0;

                case LuaDataType.Boolean:
                    return Boolean.GetHashCode();

                case LuaDataType.Integer:
                    return Integer.GetHashCode();

                case LuaDataType.Number:
                    return Number.GetHashCode();

                case LuaDataType.String:
                    return Str.GetHashCode();

                default:
                    return base.GetHashCode();
            }
        }

        public bool Equals(LuaDataUnion other)
        {
            switch (Type)
            {
                case LuaDataType.Nil:
                    return other.Type == LuaDataType.Nil;

                case LuaDataType.Boolean:
                    return other.Type == LuaDataType.Boolean && Boolean == other.Boolean;

                case LuaDataType.Integer:
                    switch (other.Type)
                    {

                        case LuaDataType.Integer:
                            return Integer == other.Integer;
                        case LuaDataType.Number:
                            return Integer == other.Number;
                        default:
                            return false;
                    }

                case LuaDataType.Number:
                    switch (other.Type)
                    {

                        case LuaDataType.Integer:
                            return Number == other.Integer;
                        case LuaDataType.Number:
                            return Number == other.Number;
                        default:
                            return false;
                    }


                case LuaDataType.String:
                    return other.Type == LuaDataType.String && Str == other.Str;

                default:
                    return base.Equals(other);
            }
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public static bool operator == (LuaDataUnion a,LuaDataUnion b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(LuaDataUnion a, LuaDataUnion b)
        {
            return !(a == b);
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

       
    }
}

