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

        public Func<LuaDataUnion, LuaDataUnion, bool> CompareFunc;

        /// <summary>
        /// 所有比较运算符的配置
        /// </summary>
        public static CompareOpConfig[] CompareOpConfigs =
        {
           new CompareOpConfig(CompareOpFunc.Eq),
           new CompareOpConfig(CompareOpFunc.Lt),
           new CompareOpConfig(CompareOpFunc.Le),
        };


    }
}

