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
        private byte[] signature;

        /// <summary>
        /// 版本号
        /// </summary>
        private byte version;

        /// <summary>
        /// 格式号
        /// </summary>
        private byte format;

        /// <summary>
        /// 校验数据 6byte 0x19 93 0D 0A 1A 0A
        /// </summary>
        private byte[] luacData;

        /// <summary>
        /// cint大小
        /// </summary>
        private byte cintSize;

        /// <summary>
        /// sizet大小
        /// </summary>
        private byte sizetSize;

        /// <summary>
        /// Lua虚拟机指令大小
        /// </summary>
        private byte instructionSize;

        /// <summary>
        /// Lua整数大小
        /// </summary>
        private byte luaIntergerSize;

        /// <summary>
        /// Lua浮点数大小
        /// </summary>
        private byte luaNumberSize;

        /// <summary>
        /// Lua整数值 0x56 78
        /// </summary>
        private long luacInt;

        /// <summary>
        /// Lua浮点数 370.5
        /// </summary>
        private double luacNum;
    }

}
