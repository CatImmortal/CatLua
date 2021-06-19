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
            new ArithOpConfig(ArithOpFuncs.IAddFunc,ArithOpFuncs.NAddFunc),
            new ArithOpConfig(ArithOpFuncs.ISubFunc,ArithOpFuncs.NSubFunc),
            new ArithOpConfig(ArithOpFuncs.IMulFunc,ArithOpFuncs.NMulFunc),
            new ArithOpConfig(ArithOpFuncs.IModFunc,ArithOpFuncs.NModFunc),
            new ArithOpConfig(null,ArithOpFuncs.PowFunc),
            new ArithOpConfig(null,ArithOpFuncs.DivFunc),
            new ArithOpConfig(ArithOpFuncs.IIdivFunc,ArithOpFuncs.NIdivFunc),
            new ArithOpConfig(ArithOpFuncs.BAndFunc,null),
            new ArithOpConfig(ArithOpFuncs.BOrFunc,null),
            new ArithOpConfig(ArithOpFuncs.BXorFunc,null),
            new ArithOpConfig(ArithOpFuncs.ShLFunc,null),
            new ArithOpConfig(ArithOpFuncs.ShRFunc,null),
            new ArithOpConfig(ArithOpFuncs.IUnmFunc,ArithOpFuncs.NUnmFunc),
            new ArithOpConfig(ArithOpFuncs.BNotFunc,null),
        };
    }
}

