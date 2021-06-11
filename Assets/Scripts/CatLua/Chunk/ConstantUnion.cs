using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace CatLua
{
    /// <summary>
    /// lua常量的模拟union
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct ConstantUnion
    {
        [FieldOffset(0)]
        public byte nil;

        [FieldOffset(0)]
        public bool boolean;

        [FieldOffset(0)]
        public long integer;

        [FieldOffset(0)]
        public double number;

        [FieldOffset(0)]
        public string shortStr;

        [FieldOffset(0)]
        public string longStr;
    }

}
