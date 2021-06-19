using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    public static class Factory
    {
        public static LuaDataUnion NewBool(bool b)
        {
            return new LuaDataUnion(LuaDataType.Boolean, boolean: b);
        }

        public static LuaDataUnion NewInteger(long l)
        {
            return new LuaDataUnion(LuaDataType.Integer,integer:l);
        }

        public static LuaDataUnion NewNumber(double d)
        {
            return new LuaDataUnion(LuaDataType.Number,number:d);
        }

        public static LuaDataUnion NewString(string s)
        {
            return new LuaDataUnion(LuaDataType.String,str:s);
        }

        public static LuaDataUnion NewTable(LuaTable t)
        {
            return new LuaDataUnion(LuaDataType.Table,table:t);
        }

        public static LuaDataUnion NewFunc(Closure c)
        {
            return new LuaDataUnion(LuaDataType.Function,closure:c);
        }


    }

}
