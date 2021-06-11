using System.Collections;
using System.Collections.Generic;


namespace CatLua
{
    /// <summary>
    /// Lua数据类型
    /// </summary>
    public enum LuaDataType : byte
    {
        None,
        Nil,
        Boolean,
        LightUserdata,
        Number,
        String,
        Table,
        Function,
        Userdata,
        Thread,
    }
}

