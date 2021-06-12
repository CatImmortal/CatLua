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
        public byte Nil;

        [FieldOffset(0)]
        public bool Boolean;

        [FieldOffset(0)]
        public long Integer;

        [FieldOffset(0)]
        public double Number;

        [FieldOffset(0)]
        public string ShortStr;

        [FieldOffset(0)]
        public string LongStr;
    }

}
