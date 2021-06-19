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
        public ArithOpConfig(Func<long, long, long> integerFunc, Func<double, double, double> numberFunc)
        {
            IntegerFunc = integerFunc;
            NumberFunc = numberFunc;
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
        /// 所有数学与位运算符的配置
        /// </summary>
        public static ArithOpConfig[] Configs =
        {
            new ArithOpConfig(ArithOpFuncs.IAdd,ArithOpFuncs.NAdd),
            new ArithOpConfig(ArithOpFuncs.ISub,ArithOpFuncs.NSub),
            new ArithOpConfig(ArithOpFuncs.IMul,ArithOpFuncs.NMul),
            new ArithOpConfig(ArithOpFuncs.IMod,ArithOpFuncs.NMod),
            new ArithOpConfig(null,ArithOpFuncs.Pow),
            new ArithOpConfig(null,ArithOpFuncs.Div),
            new ArithOpConfig(ArithOpFuncs.IIdiv,ArithOpFuncs.NIdiv),
            new ArithOpConfig(ArithOpFuncs.BAnd,null),
            new ArithOpConfig(ArithOpFuncs.BOr,null),
            new ArithOpConfig(ArithOpFuncs.BXor,null),
            new ArithOpConfig(ArithOpFuncs.ShL,null),
            new ArithOpConfig(ArithOpFuncs.ShR,null),
            new ArithOpConfig(ArithOpFuncs.IUnm,ArithOpFuncs.NUnm),
            new ArithOpConfig(ArithOpFuncs.BNot,null),
        };
    }
}

