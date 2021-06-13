using System.Collections;
using System.Collections.Generic;
using System;

namespace CatLua
{
    /// <summary>
    /// 比较运算符配置
    /// </summary>
    public struct CompareOpConfig
    {
        public CompareOpConfig(Func<LuaDataUnion, LuaDataUnion, bool> compareFunc)
        {
            CompareFunc = compareFunc;
        }

        /// <summary>
        /// 比较运算的函数实现
        /// </summary>
        public Func<LuaDataUnion, LuaDataUnion, bool> CompareFunc;

        /// <summary>
        /// 所有比较运算符的配置
        /// </summary>
        public static CompareOpConfig[] Configs =
        {
           new CompareOpConfig(CompareOpFuncs.Eq),
           new CompareOpConfig(CompareOpFuncs.Lt),
           new CompareOpConfig(CompareOpFuncs.Le),
        };


    }
}

