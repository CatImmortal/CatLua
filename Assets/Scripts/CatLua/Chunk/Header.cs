using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatLua
{
    /// <summary>
    /// Chunk的头信息
    /// </summary>
    public struct Header
    {
        /// <summary>
        /// 签名 4byte 魔数0x1B 4C 75 61
        /// </summary>
        public byte[] Signature;

        /// <summary>
        /// 版本号
        /// </summary>
        public byte Version;

        /// <summary>
        /// 格式号
        /// </summary>
        public byte Format;

        /// <summary>
        /// 校验数据 6byte 0x19 93 0D 0A 1A 0A
        /// </summary>
        public byte[] LuacData;

        /// <summary>
        /// cint大小
        /// </summary>
        public byte CintSize;

        /// <summary>
        /// sizet大小
        /// </summary>
        public byte SizetSize;

        /// <summary>
        /// Lua虚拟机指令大小
        /// </summary>
        public byte InstructionSize;

        /// <summary>
        /// Lua整数大小
        /// </summary>
        public byte LuaIntergerSize;

        /// <summary>
        /// Lua浮点数大小
        /// </summary>
        public byte LuaNumberSize;

        /// <summary>
        /// Lua整数值 0x56 78
        /// </summary>
        public long LuacInt;

        /// <summary>
        /// Lua浮点数 370.5
        /// </summary>
        public double LuacNum;
    }

}
