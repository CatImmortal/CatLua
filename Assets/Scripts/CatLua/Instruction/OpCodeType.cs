using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatLua
{
    /// <summary>
    /// 操作码类型
    /// </summary>
    public enum OpCodeType
    {
        /// <summary>
        /// 移动寄存器值
        /// </summary>
        Move,

        /// <summary>
        /// 加载常量到寄存器值
        /// </summary>
        LoadK,
    }

}

