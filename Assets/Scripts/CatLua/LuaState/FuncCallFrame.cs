using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// 函数调用栈帧
    /// </summary>
    public class FuncCallFrame
    {

        public FuncCallFrame(Closure closure = null, int bottom = 0)
        {
            Closure = closure;
            Bottom = bottom;

            int size = 0;
            if (Closure != null && Closure.Proto != null)
            {
                size = Closure.Proto.MaxStackSize;
            }
            ReserveRegisterMaxIndex = (Bottom - 1) + size; 
        }


        /// <summary>
        /// 变长参数
        /// </summary>
        public LuaDataUnion[] VarArgs;

        /// <summary>
        /// 指令索引
        /// </summary>
        public int PC;

        /// <summary>
        /// 前一个函数的调用栈帧
        /// </summary>
        public FuncCallFrame Prev;

        /// <summary>
        /// 闭包
        /// </summary>
        public Closure Closure
        {
            get;
            private set;
        }

        /// <summary>
        /// 栈帧的预留寄存器区域最大索引
        /// </summary>
        public int ReserveRegisterMaxIndex
        {
            get;
            private set;
        }

        /// <summary>
        /// 本栈帧的栈底索引
        /// </summary>
        public int Bottom
        {
            get;
            private set;
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

