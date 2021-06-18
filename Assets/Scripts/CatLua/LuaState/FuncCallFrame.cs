using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// 函数调用栈帧
    /// </summary>
    public class FuncCallFrame
    {

        /// <summary>
        /// 闭包
        /// </summary>
        public Closure Closure;

        /// <summary>
        /// 变长参数
        /// </summary>
        public LuaDataUnion[] VarArgs;

        /// <summary>
        /// 指令索引
        /// </summary>
        public int PC;
        
        /// <summary>
        /// 本栈帧的栈底索引
        /// </summary>
        public int Bottom;

        /// <summary>
        /// 前一个函数的调用栈帧
        /// </summary>
        public FuncCallFrame Prev;

        /// <summary>
        /// 栈帧的预留寄存器区域最大索引
        /// </summary>
        public int ReserveRegisterMaxIndex
        {
            get
            {
                return (Bottom - 1) + Closure.Proto.MaxStackSize;
            }
        }

        /// <summary>
        /// 获取栈帧的非预留寄存器区域的大小（预留区域最大索引到top的那部分的长度）
        /// </summary>
        public int GetNonReserveRegisterSize(int top)
        {
            return top - ReserveRegisterMaxIndex;
        }

    }
}

