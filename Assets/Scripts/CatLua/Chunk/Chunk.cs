using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatLua
{
    /// <summary>
    /// lua的二进制chunk
    /// </summary>
    public struct Chunk
    {
        /// <summary>
        /// 头信息
        /// </summary>
        public Header Header;

        /// <summary>
        /// 主函数的upvalue数量
        /// </summary>
        public byte UpvaluesSize;

        /// <summary>
        /// 主函数的函数原型
        /// </summary>
        public Prototype MainFunc;

        /// <summary>
        /// 从字节流解码trunk
        /// </summary>
        public static Chunk Undump(byte[] data)
        {
            Chunk chunk;
            ChunkReader reader = new ChunkReader(data);
            chunk.Header = reader.CheckHeader();
            chunk.UpvaluesSize = reader.ReadByte();
            chunk.MainFunc = reader.ReadProto(string.Empty);
            return chunk;
        }
    }

}
