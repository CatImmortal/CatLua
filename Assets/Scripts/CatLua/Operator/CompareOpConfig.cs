using System.Collections;
using System.Collections.Generic;
using System;

namespace CatLua
{
    /// <summary>
    /// 比较运算符配置
    /// </summary>
    public class CompareOpConfig
    {
        public CompareOpConfig(Func<LuaDataUnion, LuaDataUnion,LuaState, bool> compareFunc)
        {
            CompareFunc = compareFunc;
        }

        /// <summary>
        /// 比较运算的函数实现
        /// </summary>
        public Func<LuaDataUnion, LuaDataUnion,LuaState, bool> CompareFunc;

        /// <summary>
        /// 所有比较运算符的配置
        /// </summary>
        public static CompareOpConfig[] Configs =
        {
           new CompareOpConfig(CompareOpFuncs.EqFunc),
           new CompareOpConfig(CompareOpFuncs.LtFunc),
           new CompareOpConfig(CompareOpFuncs.LeFunc),
        };


    }
}

