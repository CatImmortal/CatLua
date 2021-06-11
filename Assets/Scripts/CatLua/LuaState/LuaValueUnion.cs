using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CatLua
{
    /// <summary>
    /// Lua值的模拟Union
    /// </summary>

    [StructLayout(LayoutKind.Explicit)]
    public struct LuaValueUnion
    {
        [FieldOffset(0)]
        public LuaDataType Type;

        [FieldOffset(8)]
        public byte Nil;

        [FieldOffset(8)]
        public bool Boolean;

        [FieldOffset(8)]
        public long Integer;

        [FieldOffset(8)]
        public double Number;

        [FieldOffset(8)]
        public string Str;
    }
}

