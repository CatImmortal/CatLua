using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatLua
{
    /// <summary>
    /// 局部变量
    /// </summary>
    public struct LocVar
    {
        /// <summary>
        /// 变量名
        /// </summary>
        public string VarName;

        public uint StartPC;

        public uint EndPC;

    }

}
