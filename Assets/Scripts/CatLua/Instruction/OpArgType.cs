using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatLua
{
    /// <summary>
    /// 操作数类型
    /// </summary>
    public enum OpArgType
    {
        /// <summary>
        /// 不表示任何信息
        /// </summary>
        N,

        /// <summary>
        /// 布尔值，整数值，upvalue索引，子函数索引等
        /// </summary>
        U,

        /// <summary>
        /// iABC模式下表示寄存器索引，iAsBx模式下表示跳转偏移
        /// </summary>
        R,

        /// <summary>
        /// 表示常量表索引或寄存器索引
        /// </summary>
        K
    }

}

