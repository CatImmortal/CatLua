using System.Collections;
using System.Collections.Generic;
using System;

namespace CatLua
{
    public partial class GenFuncInfo
    {
        /// <summary>
        /// 指令
        /// </summary>
        public List<uint> Instructions = new List<uint>();


        /// <summary>
        /// 已经生成的最后一条指令的索引
        /// </summary>
        public int PC
        {
            get
            {
                return Instructions.Count - 1;
            }
        }

        /// <summary>
        /// 生成ABC模式的编码指令
        /// </summary>
        public void EmitABC(int opcode, int a, int b, int c)
        {
            int i = b << 23 | c << 14 | a << 6 | opcode;
            Instructions.Add((uint)i);
        }

        /// <summary>
        /// 生成ABx模式的编码指令
        /// </summary>
        public void EmitABx(int opcode, int a, int bx)
        {
            int i = bx << 14 | a << 6 | opcode;
            Instructions.Add((uint)i);
        }

        /// <summary>
        /// 生成AsBx模式的编码指令
        /// </summary>
        public void EmitAsBx(int opcode, int a, int sbx)
        {
            int i = (sbx + Constants.MaxArgSbx) << 14 | a << 6 | opcode;
            Instructions.Add((uint)i);
        }

        /// <summary>
        /// 生成Ax模式的编码指令
        /// </summary>
        public void EmitAx(int opcode, int ax)
        {
            int i = ax << 6 | opcode;
            Instructions.Add((uint)i);
        }

        /// <summary>
        /// 将pc位置的指令的sBx操作数设置为指定的sBx(跳转指令用)
        /// </summary>
        public void FixSbx(int sBx, int pc)
        {
            uint i = Instructions[pc];
            i = i << 18 >> 18; //清空左边18位sBx操作数
            i |= (uint)(sBx + Constants.MaxArgSbx) << 14;  //设置新的sBx
            Instructions[pc] = i;
        }

        /// <summary>
        /// 根据upvalue获取jmp指令的参数a
        /// </summary>
        public int GetJmpArgA()
        {


            int minSlot = -1;

            foreach (KeyValuePair<string, GenUpvalueInfo> item in UpvalueDict)
            {
                if (item.Value.LocalVarSlot >= 0)
                {
                    //找出还在栈上的upvalue中 寄存器索引最小的那个值
                    minSlot = Math.Min(minSlot, item.Value.LocalVarSlot);
                }

            }

            return minSlot + 1;  //需要额外+1 这样如果没有在栈中的upvalue 就返回0
        }

        /// <summary>
        /// 生成Return指令
        /// </summary>
        public void EmitReturn(int a , int b)
        {

        }
    }

}
