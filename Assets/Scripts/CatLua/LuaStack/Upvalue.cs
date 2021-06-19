using System.Collections;
using System.Collections.Generic;


namespace CatLua
{
    public class Upvalue
    {
        public Upvalue(LuaDataUnion value)
        {
            this.value = value;
        }

        public LuaDataUnion value;


    }

}
