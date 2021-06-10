using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatLua
{
    /// <summary>
    /// lua的二进制chunk
    /// </summary>
    public struct BinaryChunk
    {
        /// <summary>
        /// 头信息
        /// </summary>
        private Header header;

        /// <summary>
        /// 主函数的upvalue数量
        /// </summary>
        private byte sizeUpvalues;

        /// <summary>
        /// 主函数的函数原型
        /// </summary>
        private Prototype mainFunc;

        /// <summary>
        /// 解码trunk
        /// </summary>
        public Prototype Undump(byte[] data)
        {
            Reader reader = new Reader(data);
            header = reader.CheckHeader();
            sizeUpvalues = reader.ReadByte(); 
            mainFunc = reader.ReadProto(string.Empty);
            return mainFunc;
        }
    }

}
