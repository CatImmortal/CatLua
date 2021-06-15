using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// 闭包
    /// </summary>
    public struct Closure
    {
        public Closure(FuncPrototype proto)
        {
            Proto = proto;
        }

        /// <summary>
        /// 闭包函数原型
        /// </summary>
        public FuncPrototype Proto;

       
    }
}

