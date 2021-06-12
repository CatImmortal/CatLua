using System.Collections;
using System.Collections.Generic;


namespace CatLua
{
    /// <summary>
    /// Lua数据类型
    /// </summary>
    public enum LuaDataType : byte
    {
        Nil,
        Boolean,
        LightUserdata,
        Integer,
        Number,
        String,
        Table,
        Function,
        Userdata,
        Thread,

        None = byte.MaxValue,
    }
}

