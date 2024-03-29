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
            curFrame.PC += n;
        }

        /// <summary>
        /// 取出当前指令，并将PC指向下一条指令
        /// </summary>
        public uint Fetch()
        {
            uint code = curFrame.Closure.Proto.Code[curFrame.PC];
            curFrame.PC++;
            return code;    
        }

        /// <summary>
        /// 将指定常量压入栈顶
        /// </summary>
        public void PushConst(int index)
        {
           LuaConstantUnion constant = curFrame.Closure.Proto.Constants[index];
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
                case LuaConstantType.ShortStr:
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
        public void PushConstOrData(int arg)
        {
            //arg是一个iABC指令编码模式下的K类型参数，共9 bit
            //最高位是1的话低8位表示常量表索引，否则低8位表示寄存器索引

            if (arg > 0xff)
            {
                //常量 取出低8位
                PushConst(arg & 0xff);
            }
            else
            {
                //栈值
                CopyAndPush(CurFrameBottom + arg);
            }
        }

        /// <summary>
        /// 将变长参数压入栈顶
        /// </summary>
        public void PushVarArg(int n)
        {
            globalStack.PushN(curFrame.VarArgs,0,n);
        }

        /// <summary>
        /// 将子函数原型中index位置的函数原型实例化为闭包，然后压入栈顶
        /// </summary>
        public void PushProto(int index)
        {
            FuncPrototype proto = curFrame.Closure.Proto.Protos[index];
            Closure c = new Closure(proto);
            Push(c);

            //处理upvalue
            for (int i = 0; i < proto.UpvalueInfos.Length; i++)
            {
                UpvalueInfo info = proto.UpvalueInfos[i];

                //upvalue在stack中的全局索引
                int globalStackIndex = info.Index + CurFrameBottom;
                
                if (info.Instack == 1)
                {
                    //upvalue是直接外围函数的局部变量

                    //尝试从openUpvlaues里获取
                    if (!openUpvalues.TryGetValue(globalStackIndex,out Upvalue upvalue))
                    {
                        //没从openUpvlaues获取到就从栈里获取，并保存到openUpvlaues中
                        upvalue = new Upvalue(globalStack.Get(globalStackIndex),true,globalStackIndex);
                        openUpvalues.Add(globalStackIndex, upvalue);
                    }
                    c.Upvalues[i] = upvalue;
                }
                else
                {
                    //upvalue是间接外围函数的局部变量 已被直接外围函数捕获过了 直接从直接外围函数的upvalue表里获取
                    //因为直接外围函数的upvalue表不是全局的 所以不用+CurFrameBottom
                    c.Upvalues[i] = curFrame.Closure.Upvalues[info.Index];
                }
            }
        }


    }

}
