using System;
using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    public class MathLib
    {
        /// <summary>
        /// 数学库函数
        /// </summary>
        private static Dictionary<string, Func<LuaState, int, int>> mathFuncs = new Dictionary<string, Func<LuaState, int, int>>()
        {
            { "sin",Sin},
        };

        /// <summary>
        /// 打开数学库
        /// </summary>
        public static int OpenMathLib(LuaState ls, int argsNum)
        {
            ls.NewLib(mathFuncs);

            
            ls.Push(Math.PI);
            ls.SetTableValue(-2, "pi");

            return 1;
        }

        private static int Sin(LuaState ls, int argsNum)
        {
            double arg = ls.CheckNumber(-1);
            ls.Push(Math.Sin(arg));
            return 1;
        }
    }

}
