using System.Collections;
using System.Collections.Generic;
using System;

namespace CatLua
{
    /// <summary>
    /// 数学与位运算符配置
    /// </summary>
    public struct ArithOpConfig
    {
        public ArithOpConfig(Func<long, long, long> integerFunc, Func<double, double, double> numberFunc)
        {
            IntegerFunc = integerFunc;
            NumberFunc = numberFunc;
        }

        public Func<long, long, long> IntegerFunc;
        public Func<double, double, double> NumberFunc;

        /// <summary>
        /// 所有数学与位运算符的配置
        /// </summary>
        public static ArithOpConfig[] ArithOpConfigs =
        {
            new ArithOpConfig(ArithOpFunc.IAdd,ArithOpFunc.NAdd),
            new ArithOpConfig(ArithOpFunc.ISub,ArithOpFunc.NSub),
            new ArithOpConfig(ArithOpFunc.IMul,ArithOpFunc.NMul),
            new ArithOpConfig(ArithOpFunc.IMod,ArithOpFunc.NMod),
            new ArithOpConfig(null,ArithOpFunc.Pow),
            new ArithOpConfig(null,ArithOpFunc.Div),
            new ArithOpConfig(ArithOpFunc.IIdiv,ArithOpFunc.NIdiv),
            new ArithOpConfig(ArithOpFunc.BAnd,null),
            new ArithOpConfig(ArithOpFunc.BOr,null),
            new ArithOpConfig(ArithOpFunc.BXor,null),
            new ArithOpConfig(ArithOpFunc.ShL,null),
            new ArithOpConfig(ArithOpFunc.ShR,null),
            new ArithOpConfig(ArithOpFunc.IUnm,ArithOpFunc.NUnm),
            new ArithOpConfig(ArithOpFunc.BNot,null),
        };
    }
}

