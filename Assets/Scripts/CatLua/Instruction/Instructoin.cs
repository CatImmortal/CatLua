using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// 指令
    /// </summary>
    public struct Instructoin
    {
        public Instructoin(uint code)
        {
            this.code = code;
        }

        /// <summary>
        /// chunk里的指令
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
                return InstructionConfig.InstructionConfigs[OpCode].Type;
            }
        }

        /// <summary>
        /// 获取编码模式
        /// </summary>
        public OpMode OpMode
        {
            get
            {
                return InstructionConfig.InstructionConfigs[OpCode].OpMode;
            }
        }

        /// <summary>
        /// 获取参数B的类型
        /// </summary>
        public OpArgType ArgBType
        {
            get
            {
                return InstructionConfig.InstructionConfigs[OpCode].ArgBType;
            }
        }

        /// <summary>
        /// 获取参数C的类型
        /// </summary>
        public OpArgType ArgCType
        {
            get
            {
                return InstructionConfig.InstructionConfigs[OpCode].ArgCType;
            }
        }

        /// <summary>
        /// 从iABC模式指令中提取参数
        /// </summary>
        public void GetABC(out int a,out int b ,out int c)
        {
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
            sbx = bx - Constants.maxArgSbx;
        }

        /// <summary>
        /// 从iAx模式指令中提取参数
        /// </summary>
        public void GetAx(out int ax)
        {
            ax = (int)(code >> 6);
        }


    }
}

