using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatLua
{
    /// <summary>
    /// 指令编码模式
    /// </summary>
    public enum OpMode : byte
    {
        /// <summary>
        /// 三个操作数A B C 分别占用8 9 9 bit
        /// </summary>
        IABC,

        /// <summary>
        /// 两个操作数A Bx 分别占用8 18 bit
        /// </summary>
        IABx,

        /// <summary>
        /// 两个操作数A Bx 分别占用8 18 bit sBx为有符号整数
        /// </summary>
        IAsBx,

        /// <summary>
        /// 一个操作数Ax 占用26 bit
        /// </summary>
        IAx,
    }
}

