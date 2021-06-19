using System.Collections;
using System.Collections.Generic;
using System;

namespace CatLua
{
    /// <summary>
    /// 数学与位运算符对应的函数实现
    /// </summary>
    public static class ArithOpFuncs
    {
        public static Func<long, long, long> IAddFunc = (a, b) => { return a + b; };
        public static Func<double, double, double> NAddFunc = (a, b) => { return a + b; };

        public static Func<long, long, long> ISubFunc = (a, b) => { return a - b; };
        public static Func<double, double, double> NSubFunc = (a, b) => { return a - b; };

        public static Func<long, long, long> IMulFunc = (a, b) => { return a * b; };
        public static Func<double, double, double> NMulFunc = (a, b) => { return a * b; };

        public static Func<long, long, long> IModFunc = (a, b) => { return a % b; };
        public static Func<double, double, double> NModFunc = (a, b) => { return a % b; };

        public static Func<double, double, double> PowFunc = Math.Pow;


        public static Func<double, double, double> DivFunc = (a, b) => { return a / b; };

        public static Func<long, long, long> IIdivFunc = (a, b) => { return (long)Math.Floor((double)a / b); };
        public static Func<double, double, double> NIdivFunc = (a, b) => { return Math.Floor(a / b); };

        public static Func<long, long, long> BAndFunc = (a, b) => { return a & b; };
        public static Func<long, long, long> BOrFunc = (a, b) => { return a | b; };
        public static Func<long, long, long> BXorFunc = (a, b) => { return a ^ b; };

        public static Func<long, long, long> ShLFunc = LMath.BitLeftMove;
        public static Func<long, long, long> ShRFunc = LMath.BitRightMove;

        public static Func<long, long, long> IUnmFunc = (a, _) => { return -a; };
        public static Func<double, double, double> NUnmFunc= (a, _) => { return -a; };

        public static Func<long, long, long> BNotFunc = (a, _) => { return ~a; };
    }

}

