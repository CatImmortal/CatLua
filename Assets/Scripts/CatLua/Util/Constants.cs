using System.Collections;
using System.Collections.Generic;

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

        //常量类型tag
        public const byte TagNil = 0x00;
        public const byte TagBoolean = 0x01;
        public const byte TagNumber = 0x03;
        public const byte TagInteger = 0x13;
        public const byte TagShortStr = 0x04;
        public const byte TagLongStr = 0x14;

        //操作数的最大值
        public const uint MaxArgBx = (1 << 18) - 1;
        public const int MaxArgSbx = (int)(MaxArgBx >> 1);

        //SetList指令的默认批次
        public const int SetListDefaultBatch = 50;

        //全局环境在注册表table里的key
        public const long GlobalEnvKey = 2;
    }
}

