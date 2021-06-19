using System;
using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// 指令
    /// </summary>
    public class Instructoin
    {
        public Instructoin(uint code)
        {
            this.code = code;
        }

        /// <summary>
        /// chunk里的指令数据
        /// </summary>
        private uint code;

        /// <summary>
        /// 获取操作码
        /// </summary>
        public int OpCode
        {
            get
            {
                return (int)(code & 0x3f);
            }
        }

        /// <summary>
        /// 获取指令类型
        /// </summary>
        public OpCodeType OpType
        {
            get
            {
                return InstructionConfig.Configs[OpCode].Type;
            }
        }

        /// <summary>
        /// 获取编码模式
        /// </summary>
        public OpMode OpMode
        {
            get
            {
                return InstructionConfig.Configs[OpCode].OpMode;
            }
        }

        /// <summary>
        /// 获取参数B的类型
        /// </summary>
        public OpArgType ArgBType
        {
            get
            {
                return InstructionConfig.Configs[OpCode].ArgBType;
            }
        }

        /// <summary>
        /// 获取参数C的类型
        /// </summary>
        public OpArgType ArgCType
        {
            get
            {
                return InstructionConfig.Configs[OpCode].ArgCType;
            }
        }

        /// <summary>
        /// 从iABC模式指令中提取参数
        /// </summary>
        public void GetABC(out int a,out int b ,out int c)
        {
            //iABC编码的指令，操作数在内存上是按ACB来排列的

            a = (int)((code >> 6) & 0xff);
            c = (int)((code >> 14) & 0x1ff);
            b = (int)((code >> 23) & 0x1ff);
        }

        /// <summary>
        /// 从iABx模式指令中提取参数
        /// </summary>
        public void GetABx(out int a, out int bx)
        {
            a = (int)((code >> 6) & 0xff);
            bx = (int)(code >> 14);
        }

        /// <summary>
        /// 从iAsBx模式指令中提取参数
        /// </summary>
        public void GetAsBx(out int a, out int sbx)
        {
            int bx;
            GetABx(out a,out bx);

            //sbx表示有符号整数
            sbx = bx - Constants.MaxArgSbx;
        }

        /// <summary>
        /// 从iAx模式指令中提取参数
        /// </summary>
        public void GetAx(out int ax)
        {
            ax = (int)(code >> 6);
        }

        /// <summary>
        /// 执行指令
        /// </summary>
        public void Execute(LuaState vm)
        {
            Action<Instructoin, LuaState> function = InstructionConfig.Configs[OpCode].Func;
            if (function != null)
            {
                function(this, vm);
            }
            else
            {
                throw new Exception("指令没有对应的函数实现：" + OpType.ToString());
            }
        }
    }
}

