using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CatLua
{
    /// <summary>
    /// Lua数据的模拟Union
    /// </summary>


    public class LuaDataUnion
    {
        public LuaDataUnion(LuaDataType type, bool boolean = default, long integer = default, double number = default, string str = default, LuaTable table = default, Closure closure = default)
        {
            Type = type;

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

        public static LuaDataUnion Nil = new LuaDataUnion(LuaDataType.Nil);

        public LuaDataType Type
        {
            get;
            private set;
        }


        public bool Boolean
        {
            get;
            private set;
        }


        public long Integer
        {
            get;
            private set;
        }


        public double Number
        {
            get;
            private set;
        }


        public string Str
        {
            get;
            private set;
        }


        public LuaTable Table
        {
            get;
            private set;
        }


        public Closure Closure
        {
            get;
            private set;
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
                case LuaDataType.Table:
                    s = Table.ToString();
                    break;
                default:
                    s = Type.ToString();
                    break;
            }
            return s;
        }

        public bool Equals(LuaDataUnion other)
        {
            bool result = false;

            switch (Type)
            {
                case LuaDataType.Nil:
                    result = other.Type == LuaDataType.Nil;
                    break;

                case LuaDataType.Boolean:
                    result = Boolean == other.Boolean;
                    break;

                case LuaDataType.Integer:

                    switch (other.Type)
                    {

                        case LuaDataType.Integer:
                            result = Integer == other.Integer;
                            break;
                        case LuaDataType.Number:
                            result = Integer == other.Number;
                            break;
                        default:
                            result = false;
                            break;
                    }

                    break;
                case LuaDataType.Number:

                    switch (other.Type)
                    {

                        case LuaDataType.Integer:
                            result = Number == other.Integer;
                            break;
                        case LuaDataType.Number:
                            result = Number == other.Number;
                            break;
                        default:
                            result = false;
                            break;
                    }

                    break;

                case LuaDataType.String:
                    result = other.Type == LuaDataType.String && Str == other.Str;
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

        public override bool Equals(object obj)
        {
            return Equals(obj as LuaDataUnion);
        }

        public override int GetHashCode()
        {
            int hashCode;
            switch (Type)
            {
                case LuaDataType.Nil:
                    hashCode = 0;
                    break;
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
                default:
                    hashCode = base.GetHashCode();
                    break;
            }

            return hashCode;
        }

        public static bool operator ==(LuaDataUnion x, LuaDataUnion y)
        {
            if (x is null && y is null)
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            return x.Equals(y);
        }

        public static bool operator !=(LuaDataUnion x, LuaDataUnion y)
        {
            return !(x == y);
        }

      

        /// <summary>
        /// 转换bool
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
        /// 尝试转换number
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
        /// 尝试转换integer
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

        public void ChangeData(LuaDataUnion data)
        {
            Type = data.Type;

            switch (Type)
            {

                case LuaDataType.Boolean:
                    Boolean = data.Boolean;
                    break;
                case LuaDataType.Integer:
                    Integer = data.Integer;
                    break;
                case LuaDataType.Number:
                    Number = data.Number;
                    break;
                case LuaDataType.String:
                    Str = data.Str;
                    break;
                case LuaDataType.Table:
                    Table = data.Table;
                    break;
                case LuaDataType.Function:
                    Closure = data.Closure;
                    break;
            }

        }
   
        public LuaDataUnion Copy()
        {
            return (LuaDataUnion)MemberwiseClone();
        }
    }
}

