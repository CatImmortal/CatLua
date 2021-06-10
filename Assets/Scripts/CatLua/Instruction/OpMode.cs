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
        IABC,
        IABx,
        IAsBx,
        IAx,
    }
}

