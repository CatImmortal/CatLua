using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// upvalue信息
    /// </summary>
    public class GenUpvalueInfo
    {

        public GenUpvalueInfo(int localVarSlot, int upvalueIndex, int index)
        {
            LocalVarSlot = localVarSlot;
            UpvalueIndex = upvalueIndex;
            Index = index;
        }

        /// <summary>
        /// upvalue捕获的是直接外围函数的局部变量时，此字段表示该局部变量的寄存器索引
        /// </summary>
        public int LocalVarSlot;

        /// <summary>
        /// upvalue要捕获的局部变量已经被直接外围函数捕获时，此字段表示该upvalue在在直接外围函数upvalue表的索引
        /// </summary>
        public int UpvalueIndex;

        /// <summary>
        /// upvalue在函数中出现的顺序
        /// </summary>
        public int Index;

    }
}

