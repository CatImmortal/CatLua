using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// 数学与位运算符类型
    /// </summary>
    public enum ArithOpType : byte
    {
        Add,
        Sub,
        Mul,
        Mod,
        Pow,
        Div,

        /// <summary>
        /// 整除
        /// </summary>
        IDiv,

        BAnd,
        BOr,
        BXor,

        /// <summary>
        /// 左移
        /// </summary>
        ShL,

        /// <summary>
        /// 无符号右移
        /// </summary>
        ShR,

        /// <summary>
        /// 取负
        /// </summary>
        Unm,

        BNot,
        
    }
}

