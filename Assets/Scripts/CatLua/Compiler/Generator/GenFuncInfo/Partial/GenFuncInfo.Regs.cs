using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    public partial class GenFuncInfo
    {
        /// <summary>
        /// 已使用寄存器数量
        /// </summary>
        public int UsedRegs;

        /// <summary>
        /// 最大寄存器数量
        /// </summary>
        public int MaxRegs;

        /// <summary>
        /// 分配寄存器，返回分配到的寄存器的索引
        /// </summary>
        public int AllocReg()
        {
            UsedRegs++;

            if (UsedRegs >= 255)
            {
                throw new System.Exception("已使用寄存器数量 >= 255");
            }

            if (UsedRegs > MaxRegs)
            {
                MaxRegs = UsedRegs;
            }

            return UsedRegs - 1;
        }

        /// <summary>
        /// 回收寄存器
        /// </summary>
        public void FreeReg()
        {
            UsedRegs--;
        }

        /// <summary>
        /// 分配n个寄存器，返回第一个寄存器的索引
        /// </summary>
        public int AllocRegs(int n)
        {
            for (int i = 0; i < n; i++)
            {
                AllocReg();
            }

            return UsedRegs - n;
        }

        /// <summary>
        /// 回收n个寄存器
        /// </summary>
        public void FreeRegs(int n)
        {
            UsedRegs -= n;
        }
    }

}
