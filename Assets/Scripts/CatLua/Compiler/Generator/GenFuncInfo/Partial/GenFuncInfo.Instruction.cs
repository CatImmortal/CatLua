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
        public void FixSbx(int pc , int sBx)
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


            //int minSlot = -1;

            //foreach (KeyValuePair<string, GenUpvalueInfo> item in UpvalueDict)
            //{
            //    if (item.Value.LocalVarSlot >= 0)
            //    {
            //        //找出还在栈上的upvalue中 寄存器索引最小的那个值
            //        minSlot = Math.Min(minSlot, item.Value.LocalVarSlot);
            //    }

            //}

            //return minSlot + 1;  //需要额外+1 这样如果没有在栈中的upvalue 就返回0

            //是否有被upvalue捕获的局部变量
            bool hasCaptureLocalVars = false;

            //生效中的局部变量里 绑定到的寄存器索引最小值
            int minSlotOfLocalVars = MaxRegs;

            //遍历当前生效的局部变量
            foreach (KeyValuePair<string, GenLocalVarInfo> item in activeLocalVarDict)
            {
                if (item.Value.ScopeLv == ScopeLv)
                {
                    //当前作用域
                    GenLocalVarInfo v = item.Value;
                    
                    //同名 且在当前作用域的局部变量
                    while (v != null && v.ScopeLv == ScopeLv)
                    {

                        if (v.Captured)
                        {
                            //被Upvalue捕获过
                            hasCaptureLocalVars = true;
                        }

                        if (v.Slot < minSlotOfLocalVars && v.Name[0] != '(')
                        {
                            minSlotOfLocalVars = v.Slot;
                        }

                        v = v.Prev;
                    }
                }
            }

            if (hasCaptureLocalVars)
            {
                return minSlotOfLocalVars + 1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 生成Return指令
        /// </summary>
        public void EmitReturn(int a , int b)
        {

        }

        /// <summary>
        /// 生成jmp指令
        /// </summary>
        public int EmitJmp(int a ,int b)
        {

        }

        /// <summary>
        /// 生成test指令
        /// </summary>
        public int EmitTest(int reg,int a)
        {

        }
    }

}
