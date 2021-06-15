﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatLua
{
    /// <summary>
    /// 常量定义类
    /// </summary>
    public static class Constants
    {
        //header校验常量
        public const string LuaSignature = "Lua";
        public const byte LuacVersion = 0x53;
        public const byte LuacFormat = 0;
        public const string LuacData = "�\r\n\n";
        public const byte CintSize = 4;
        public const byte CsizetSize = 4;
        public const byte InstructionSize = 4;
        public const byte LuaIntergerSize = 8;
        public const byte LuaNumberSize = 8;
        public const short LuacInt = 0x5678;
        public const double LuacNum = 370.5;

        //类型tag
        public const byte tagNil = 0x00;
        public const byte tagBoolean = 0x01;
        public const byte tagNumber = 0x03;
        public const byte tagInteger = 0x13;
        public const byte tagShortStr = 0x04;
        public const byte tagLongStr = 0x14;

        //操作数的最大值
        public const uint maxArgBx = (1 << 18) - 1;
        public const int maxArgSbx = (int)(maxArgBx >> 1);

        //SetList指令的默认批次
        public const int SetListDefaultBatch = 50;
    }
}
