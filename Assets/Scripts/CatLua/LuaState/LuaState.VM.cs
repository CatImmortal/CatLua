﻿using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    public partial class LuaState
    {
  



        /// <summary>
        /// 修改PC
        /// </summary>
        public void AddPC(int n )
        {
            CurStack.PC += n;
        }

        /// <summary>
        /// 取出当前指令，并将PC指向下一条指令
        /// </summary>
        public uint Fetch()
        {
            uint code = CurStack.Closure.Proto.Code[CurStack.PC];
            CurStack.PC++;
            return code;    
        }

        /// <summary>
        /// 将指定常量压入栈顶
        /// </summary>
        public void PushConst(int index)
        {
           LuaConstantUnion constant = CurStack.Closure.Proto.Constants[index];
            switch (constant.Type)
            {
                case LuaConstantType.Nil:
                    Push();
                    break;
                case LuaConstantType.Boolean:
                    Push(constant.Boolean);
                    break;
                case LuaConstantType.Integer:
                    Push(constant.Integer);
                    break;
                case LuaConstantType.Number:
                    Push(constant.Number);
                    break;
                case LuaConstantType.ShorStr:
                case LuaConstantType.LongStr:
                    Push(constant.Str);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 将指定常量或栈里的值压入栈顶
        /// </summary>
        public void PushRK(int rk)
        {
            //rk是一个iABC指令编码模式下的K类型参数，共9 bit
            //最高位是1的话低8位表示常量表索引，否则低8位表示寄存器索引

            if (rk > 0xff)
            {
                //常量 取出低8位
                PushConst(rk & 0xff);
            }
            else
            {
                //栈值
                //寄存器索引从0开始 但是Lua栈索引从1开始 所以需要+1
                CopyAndPush(rk + 1);
            }
        }
    }

}
