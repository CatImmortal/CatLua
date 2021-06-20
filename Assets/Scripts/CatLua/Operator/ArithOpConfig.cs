using System.Collections;
using System.Collections.Generic;
using System;

namespace CatLua
{
    /// <summary>
    /// 数学与位运算符配置
    /// </summary>
    public class ArithOpConfig
    {
        public ArithOpConfig(Func<long, long, long> integerFunc, Func<double, double, double> numberFunc, string metaMethodName)
        {
            IntegerFunc = integerFunc;
            NumberFunc = numberFunc;
            MetaMethodName = metaMethodName;
        }

        /// <summary>
        /// 数学与位运算整数版本的函数实现
        /// </summary>
        public Func<long, long, long> IntegerFunc;

        /// <summary>
        /// 数学与位运算浮点数版本的函数实现
        /// </summary>
        public Func<double, double, double> NumberFunc;

        /// <summary>
        /// 元方法名
        /// </summary>
        public string MetaMethodName;

        /// <summary>
        /// 所有数学与位运算符的配置
        /// </summary>
        public static ArithOpConfig[] Configs =
        {
            new ArithOpConfig(ArithOpFuncs.IAddFunc,ArithOpFuncs.NAddFunc,"__add"),
            new ArithOpConfig(ArithOpFuncs.ISubFunc,ArithOpFuncs.NSubFunc,"__sub"),
            new ArithOpConfig(ArithOpFuncs.IMulFunc,ArithOpFuncs.NMulFunc,"__mul"),
            new ArithOpConfig(ArithOpFuncs.IModFunc,ArithOpFuncs.NModFunc,"__mod"),
            new ArithOpConfig(null,ArithOpFuncs.PowFunc,"__pow"),
            new ArithOpConfig(null,ArithOpFuncs.DivFunc,"__div"),
            new ArithOpConfig(ArithOpFuncs.IIdivFunc,ArithOpFuncs.NIdivFunc,"__idiv"),
            new ArithOpConfig(ArithOpFuncs.BAndFunc,null,"__band"),
            new ArithOpConfig(ArithOpFuncs.BOrFunc,null,"__bor"),
            new ArithOpConfig(ArithOpFuncs.BXorFunc,null,"__bxor"),
            new ArithOpConfig(ArithOpFuncs.ShLFunc,null,"__shl"),
            new ArithOpConfig(ArithOpFuncs.ShRFunc,null,"__shr"),
            new ArithOpConfig(ArithOpFuncs.IUnmFunc,ArithOpFuncs.NUnmFunc,"__unm"),
            new ArithOpConfig(ArithOpFuncs.BNotFunc,null,"__bnot"),
        };
    }
}

