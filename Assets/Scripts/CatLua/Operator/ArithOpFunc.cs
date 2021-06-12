using System.Collections;
using System.Collections.Generic;
using System;

namespace CatLua
{
    /// <summary>
    /// 数学与位运算符对应的函数实现
    /// </summary>
    public static class ArithOpFunc
    {
        public static Func<long, long, long> IAdd = (a, b) => { return a + b; };
        public static Func<double, double, double> NAdd = (a, b) => { return a + b; };

        public static Func<long, long, long> ISub = (a, b) => { return a - b; };
        public static Func<double, double, double> NSub = (a, b) => { return a - b; };

        public static Func<long, long, long> IMul = (a, b) => { return a * b; };
        public static Func<double, double, double> NMul = (a, b) => { return a * b; };

        public static Func<long, long, long> IMod = (a, b) => { return a % b; };
        public static Func<double, double, double> NMod = (a, b) => { return a % b; };

        public static Func<double, double, double> Pow = Math.Pow;


        public static Func<double, double, double> Div = (a, b) => { return a / b; };

        public static Func<long, long, long> IIdiv = (a, b) => { return (long)Math.Floor((double)a / b); };
        public static Func<double, double, double> NIdiv = (a, b) => { return Math.Floor(a / b); };

        public static Func<long, long, long> BAnd = (a, b) => { return a & b; };
        public static Func<long, long, long> BOr = (a, b) => { return a | b; };
        public static Func<long, long, long> BXor = (a, b) => { return a ^ b; };

        public static Func<long, long, long> ShL = LMath.BitLeftMove;
        public static Func<long, long, long> ShR = LMath.BitRightMove;

        public static Func<long, long, long> IUnm = (a, _) => { return -a; };
        public static Func<double, double, double> NUnm= (a, _) => { return -a; };

        public static Func<long, long, long> BNot = (a, _) => { return ~a; };
    }

}

