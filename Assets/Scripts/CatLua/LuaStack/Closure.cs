using System.Collections;
using System.Collections.Generic;
using System;

namespace CatLua
{
    /// <summary>
    /// 闭包
    /// </summary>
    public class Closure
    {
        public Closure(FuncPrototype proto)
        {
            Proto = proto;
            CSFunc = null;
        }

        public Closure(Func<LuaState, int> csFunc)
        {
            Proto = null;
            CSFunc = csFunc;
        }

        /// <summary>
        /// Lua函数闭包
        /// </summary>
        public FuncPrototype Proto;

        /// <summary>
        /// C#函数闭包
        /// </summary>
        public Func<LuaState, int> CSFunc;
    }
}

