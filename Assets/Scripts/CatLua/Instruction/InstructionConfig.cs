using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatLua
{
    /// <summary>
    /// 指令配置
    /// </summary>
    public struct InstructionConfig
    {
        public InstructionConfig(byte testFlag, byte setAFlag, OpArgType argBType, OpArgType argCType, OpMode opMode, OpCodeType type)
        {
            TestFlag = testFlag;
            SetAFlag = setAFlag;
            ArgBType = argBType;
            ArgCType = argCType;
            OpMode = opMode;
            Type = type;
        }

        /// <summary>
        /// 是否为测试类指令
        /// </summary>
        public byte TestFlag;

        /// <summary>
        /// 是否设置寄存器a
        /// </summary>
        public byte SetAFlag;

        /// <summary>
        /// 操作数B的类型
        /// </summary>
        public OpArgType ArgBType;

        /// <summary>
        /// 操作数C的类型
        /// </summary>
        public OpArgType ArgCType;

        /// <summary>
        /// 编码模式
        /// </summary>
        public OpMode OpMode;

        /// <summary>
        /// 操作码类型
        /// </summary>
        public OpCodeType Type;

        /// <summary>
        /// 所有指令的指令配置
        /// </summary>
        public static InstructionConfig[] InstructionConfigs =
        {
            //                    T A      B           C           mode          type
            new InstructionConfig(0,1,OpArgType.R,OpArgType.N,OpMode.IABC,OpCodeType.Move),
            new InstructionConfig(0,1,OpArgType.K,OpArgType.N,OpMode.IABx,OpCodeType.LoadK),
            new InstructionConfig(0,1,OpArgType.N,OpArgType.N,OpMode.IABx,OpCodeType.LoadKX),
            new InstructionConfig(0,1,OpArgType.U,OpArgType.U,OpMode.IABC,OpCodeType.LoadBool),
            new InstructionConfig(0,1,OpArgType.U,OpArgType.N,OpMode.IABC,OpCodeType.LoadNil),
            new InstructionConfig(0,1,OpArgType.U,OpArgType.N,OpMode.IABC,OpCodeType.GetUpValue),
            new InstructionConfig(0,1,OpArgType.U,OpArgType.K,OpMode.IABC,OpCodeType.GetTabup),
            new InstructionConfig(0,1,OpArgType.R,OpArgType.K,OpMode.IABC,OpCodeType.GetTable),
            new InstructionConfig(0,0,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.SetTabup),
            new InstructionConfig(0,0,OpArgType.U,OpArgType.N,OpMode.IABC,OpCodeType.SetUpvalue),
            new InstructionConfig(0,0,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.SetTable),
            new InstructionConfig(0,1,OpArgType.U,OpArgType.U,OpMode.IABC,OpCodeType.NewTable),
            new InstructionConfig(0,1,OpArgType.R,OpArgType.K,OpMode.IABC,OpCodeType.Self),
            new InstructionConfig(0,1,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.Add),
            new InstructionConfig(0,1,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.Sub),
            new InstructionConfig(0,1,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.Mul),
            new InstructionConfig(0,1,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.Mod),
            new InstructionConfig(0,1,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.Pow),
            new InstructionConfig(0,1,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.Div),
            new InstructionConfig(0,1,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.IDiv),
            new InstructionConfig(0,1,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.BAnd),
            new InstructionConfig(0,1,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.BOr),
            new InstructionConfig(0,0,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.BXor),
            new InstructionConfig(0,0,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.ShL),
            new InstructionConfig(0,1,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.ShR),
            new InstructionConfig(0,1,OpArgType.R,OpArgType.N,OpMode.IABC,OpCodeType.Unm),
            new InstructionConfig(0,1,OpArgType.R,OpArgType.N,OpMode.IABC,OpCodeType.Bnot),
            new InstructionConfig(0,1,OpArgType.R,OpArgType.N,OpMode.IABC,OpCodeType.Not),
            new InstructionConfig(0,1,OpArgType.R,OpArgType.N,OpMode.IABC,OpCodeType.Len),
            new InstructionConfig(0,1,OpArgType.R,OpArgType.R,OpMode.IABC,OpCodeType.Concat),
            new InstructionConfig(0,1,OpArgType.R,OpArgType.N,OpMode.IAsBx,OpCodeType.Jmp),
            new InstructionConfig(1,0,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.Eq),
            new InstructionConfig(1,0,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.Lt),
            new InstructionConfig(1,0,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.Le),
            new InstructionConfig(1,0,OpArgType.N,OpArgType.U,OpMode.IABC,OpCodeType.Test),
            new InstructionConfig(1,1,OpArgType.R,OpArgType.U,OpMode.IABC,OpCodeType.TestSet),
            new InstructionConfig(0,1,OpArgType.U,OpArgType.U,OpMode.IABC,OpCodeType.Call),
            new InstructionConfig(0,1,OpArgType.U,OpArgType.U,OpMode.IABC,OpCodeType.TailCall),
            new InstructionConfig(0,0,OpArgType.U,OpArgType.N,OpMode.IABC,OpCodeType.Return),
            new InstructionConfig(0,1,OpArgType.R,OpArgType.N,OpMode.IAsBx,OpCodeType.ForLoop),
            new InstructionConfig(0,1,OpArgType.R,OpArgType.N,OpMode.IAsBx,OpCodeType.ForPrep),
            new InstructionConfig(0,0,OpArgType.N,OpArgType.U,OpMode.IABC,OpCodeType.TForCall),
            new InstructionConfig(0,1,OpArgType.R,OpArgType.N,OpMode.IAsBx,OpCodeType.TForloop),
            new InstructionConfig(0,0,OpArgType.U,OpArgType.U,OpMode.IABC,OpCodeType.Setlist),
            new InstructionConfig(0,1,OpArgType.U,OpArgType.N,OpMode.IABx,OpCodeType.Closure),
            new InstructionConfig(0,1,OpArgType.U,OpArgType.N,OpMode.IABC,OpCodeType.Vararg),
            new InstructionConfig(0,0,OpArgType.U,OpArgType.U,OpMode.IAx,OpCodeType.ExtraArg),
        };

      
    }

}
