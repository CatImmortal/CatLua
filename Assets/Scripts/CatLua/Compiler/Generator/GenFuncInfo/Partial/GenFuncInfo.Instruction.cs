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

        private static Dictionary<TokenType, OpCodeType> ArithAndBitwiseBinops = new Dictionary<TokenType, OpCodeType>()
        {
            { TokenType.OpAdd,OpCodeType.Add},
            { TokenType.OpSub,OpCodeType.Sub},
            { TokenType.OpMul,OpCodeType.Mul},
            { TokenType.OpMod,OpCodeType.Mod},
            { TokenType.OpPow,OpCodeType.Pow},
            { TokenType.OpDiv,OpCodeType.Div},
            { TokenType.OpIDiv,OpCodeType.IDiv},
            { TokenType.OpBAnd,OpCodeType.BAnd},
            { TokenType.OpBOr,OpCodeType.BOr},
            { TokenType.OpBXor,OpCodeType.BXOr},
            { TokenType.OpShL,OpCodeType.ShL},
            { TokenType.OpShR,OpCodeType.ShR},

        };

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
        public void EmitABC(OpCodeType opcode, int a, int b, int c)
        {
            int i = b << 23 | c << 14 | a << 6 | (int)opcode;
            Instructions.Add((uint)i);
        }

        /// <summary>
        /// 生成ABx模式的编码指令
        /// </summary>
        public void EmitABx(OpCodeType opcode, int a, int bx)
        {
            int i = bx << 14 | a << 6 | (int)opcode;
            Instructions.Add((uint)i);
        }

        /// <summary>
        /// 生成AsBx模式的编码指令
        /// </summary>
        public void EmitAsBx(OpCodeType opcode, int a, int sbx)
        {
            int i = (sbx + Constants.MaxArgSbx) << 14 | a << 6 | (int)opcode;
            Instructions.Add((uint)i);
        }

        /// <summary>
        /// 生成Ax模式的编码指令
        /// </summary>
        public void EmitAx(OpCodeType opcode, int ax)
        {
            int i = ax << 6 | (int)opcode;
            Instructions.Add((uint)i);
        }

        /// <summary>
        /// 将pc位置的指令的sBx操作数设置为指定的sBx(跳转用)
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

            //是否有被upvalue捕获的局部变量
            bool hasCaptureLocalVars = false;

            //生效中的局部变量里 绑定到的寄存器索引最小值
            int minSlotOfLocalVars = MaxRegs;

            //遍历当前生效的局部变量 检查是否有被upvalue捕获过的 同时获取局部变量中的寄存器索引最小值
            foreach (KeyValuePair<string, GenLocalVarInfo> item in activeLocalVarDict)
            {
                if (item.Value.ScopeLv == ScopeLv)
                {
                    GenLocalVarInfo v = item.Value;

                    while (v != null && v.ScopeLv == ScopeLv)
                    {

                        if (v.Captured)
                        {
                            hasCaptureLocalVars = true;
                        }

                        if (v.Slot < minSlotOfLocalVars && v.Name[0] != '(')  // ( 开头的局部变量 是编译器生成的 不考虑在内
                        {
                            minSlotOfLocalVars = v.Slot;
                        }

                        v = v.Prev;
                    }
                }
            }

            if (hasCaptureLocalVars)
            {
                //需要闭合upvalue
                return minSlotOfLocalVars + 1;
            }
            else
            {
                return 0;
            }
        }

        public int EmitMove(int a, int b)
        {

        }

        public int EmitLoadK(int a,long val)
        {

        }

        public int EmitLoadK(int a, double val)
        {

        }

        public int EmitLoadK(int a, string val)
        {

        }


        public int EmitLoadKX(int a, int ax)
        {

        }

        public int EmitLoadBool(int a,int b,int c)
        {

        }

        public int EmitLoadNil(int a, int b)
        {

        }

        public int EmitGetUpvalue(int a,int b)
        {

        }

        public int EmitGetTabUp(int a, int b, int c)
        {

        }

        public int EmitGetTable(int a, int b, int c)
        {

        }



        public int EmitSetTabUp(int a, int b, int c)
        {

        }

        public int EmitSetUpValue(int a, int b)
        {

        }

        public int EmitSetTable(int a, int b, int c)
        {

        }



        public int EmitNewTable(int a, int b, int c)
        {
            EmitABC(OpCodeType.NewTable, a, LMath.Int2Fb(b), LMath.Int2Fb(c));
        }

        public int EmitSelf(int a, int b, int c)
        {

        }

        public void EmitConcat(int a,int b ,int c)
        {
            EmitABC(OpCodeType.Concat, a, b, c);
        }


        public int EmitJmp(int a, int b)
        {

        }

        public int EmitTest(int a,int c)
        {

        }

        public int EmitTestSet(int a, int b,int c)
        {

        }

        public int EmitCall(int a, int b, int c)
        {

        }

        public int EmitTailCall()
        {

        }


        public void EmitReturn(int a, int b)
        {

        }

        public int EmitForLoop(int a, int sbx)
        {

        }

        public int EmitForPrep(int a, int sbx)
        {

        }


        public int EmitTForCall(int a, int c)
        {

        }

        public int EmitTForLoop(int a, int sbx)
        {

        }

        public int EmitSetList(int a, int b , int c)
        {

        }

        public int EmitClosure(int a,int bx)
        {

        }

        public int EmitVararg(int a, int b)
        {

        }

        /// <summary>
        /// 编译一元运算指令
        /// </summary>
       public void EmitUnaryOp(TokenType type,int a,int b)
        {
            switch (type)
            {
                case TokenType.OpNot:
                    EmitABC(OpCodeType.Not, a, b, 0);
                    break;
                case TokenType.OpBNot:
                    EmitABC(OpCodeType.BNot, a, b, 0);
                    break;
                case TokenType.OpLen:
                    EmitABC(OpCodeType.Len, a, b, 0);
                    break;
                case TokenType.OpUnm:
                    EmitABC(OpCodeType.Unm, a, b, 0);
                    break;
            }
        }

        /// <summary>
        /// 编译二元运算指令
        /// </summary>
        public void EmitBinaryOp(TokenType type, int a, int b,int c)
        {
    
            if (ArithAndBitwiseBinops.TryGetValue(type,out OpCodeType opCodeType))
            {
                //数学与位运算
                EmitABC(opCodeType, a, b, c);
            }
            else
            {
                //比较运算

                switch (type)
                {
                    //将 b c位置的值进行比较 
                    //如果不是a 就跳过下一条jmp指令 将结果设为false
                    //如果是a 就通过jmp 将结果设为true  

                    case TokenType.OpEq:
                        EmitABC(OpCodeType.Eq, 1, b, c);
                        break;
                    case TokenType.OpNe:
                        EmitABC(OpCodeType.Eq, 0, b, c);
                        break;
                    case TokenType.OpLt:
                        EmitABC(OpCodeType.Lt, 1, b, c);
                        break;
                    case TokenType.OpGt:
                        EmitABC(OpCodeType.Lt, 1, c, b);
                        break;
                    case TokenType.OpLe:
                        EmitABC(OpCodeType.Le, 1, b, c);
                        break;
                    case TokenType.OpGe:
                        EmitABC(OpCodeType.Le, 1, c, b);
                        break;
                }

                //todo:感觉可以优化成2条指令
                //EmitLoadBool(a, 1, 1);
                //EmitLoadBool(a, 0, 0);

                EmitJmp(0, 1);  //跳过下一条指令 执行第二条LoadBool
                EmitLoadBool(a, 0, 1);  //将a位置的值设置为false 并跳过下一条LoadBool
                EmitLoadBool(a, 1, 0);  //将a位置的值设置为true
            }

         
        }




    }

}
