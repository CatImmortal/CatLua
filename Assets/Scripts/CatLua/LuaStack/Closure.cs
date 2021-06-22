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

            if (Proto.UpvalueInfos.Length > 0)
            {
                Upvalues = new Upvalue[Proto.UpvalueInfos.Length];
            }
        }

        public Closure(Func<LuaState, int, int> csFunc,int upvalueNum = 0)
        {
            Proto = null;
            CSFunc = csFunc;

            if (upvalueNum > 0)
            {
                Upvalues = new Upvalue[upvalueNum];
            }
        }

        /// <summary>
        /// Lua函数闭包
        /// </summary>
        public FuncPrototype Proto;

        /// <summary>
        /// C#函数闭包
        /// </summary>
        public Func<LuaState, int,int> CSFunc;

        /// <summary>
        /// 捕获到的Upvalue列表
        /// </summary>
        public Upvalue[] Upvalues;
    }
}

