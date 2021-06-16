using System.Collections;
using System.Collections.Generic;
using System;

namespace CatLua
{
    /// <summary>
    /// 指令配置
    /// </summary>
    public struct InstructionConfig
    {
        public InstructionConfig(byte testFlag, byte setAFlag, OpArgType argBType, OpArgType argCType, OpMode opMode, OpCodeType type, Action<Instructoin, LuaState> func = null)
        {
            TestFlag = testFlag;
            SetAFlag = setAFlag;
            ArgBType = argBType;
            ArgCType = argCType;
            OpMode = opMode;
            Type = type;
            Func = func;
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
        /// 指令的函数实现
        /// </summary>
        public Action<Instructoin, LuaState> Func;

        /// <summary>
        /// 所有指令的指令配置
        /// </summary>
        public static InstructionConfig[] Configs =
        {
            //                    T A      B           C           mode          type
            new InstructionConfig(0,1,OpArgType.R,OpArgType.N,OpMode.IABC,OpCodeType.Move,InstructionFuncs.Move),
            new InstructionConfig(0,1,OpArgType.K,OpArgType.N,OpMode.IABx,OpCodeType.LoadK,InstructionFuncs.LoadK),
            new InstructionConfig(0,1,OpArgType.N,OpArgType.N,OpMode.IABx,OpCodeType.LoadKX,InstructionFuncs.LoadKX),
            new InstructionConfig(0,1,OpArgType.U,OpArgType.U,OpMode.IABC,OpCodeType.LoadBool,InstructionFuncs.LoadBool),
            new InstructionConfig(0,1,OpArgType.U,OpArgType.N,OpMode.IABC,OpCodeType.LoadNil,InstructionFuncs.LoadNil),
            new InstructionConfig(0,1,OpArgType.U,OpArgType.N,OpMode.IABC,OpCodeType.GetUpValue),
            new InstructionConfig(0,1,OpArgType.U,OpArgType.K,OpMode.IABC,OpCodeType.GetTabup),
            new InstructionConfig(0,1,OpArgType.R,OpArgType.K,OpMode.IABC,OpCodeType.GetTable,InstructionFuncs.GetTable),
            new InstructionConfig(0,0,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.SetTabup),
            new InstructionConfig(0,0,OpArgType.U,OpArgType.N,OpMode.IABC,OpCodeType.SetUpvalue),
            new InstructionConfig(0,0,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.SetTable,InstructionFuncs.SetTable),
            new InstructionConfig(0,1,OpArgType.U,OpArgType.U,OpMode.IABC,OpCodeType.NewTable,InstructionFuncs.NewTable),
            new InstructionConfig(0,1,OpArgType.R,OpArgType.K,OpMode.IABC,OpCodeType.Self,InstructionFuncs.Self),
            new InstructionConfig(0,1,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.Add,InstructionFuncs.Add),
            new InstructionConfig(0,1,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.Sub,InstructionFuncs.Sub),
            new InstructionConfig(0,1,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.Mul,InstructionFuncs.Mul),
            new InstructionConfig(0,1,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.Mod,InstructionFuncs.Mod),
            new InstructionConfig(0,1,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.Pow,InstructionFuncs.Pow),
            new InstructionConfig(0,1,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.Div,InstructionFuncs.Div),
            new InstructionConfig(0,1,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.IDiv,InstructionFuncs.IDiv),
            new InstructionConfig(0,1,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.BAnd,InstructionFuncs.BAnd),
            new InstructionConfig(0,1,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.BOr,InstructionFuncs.BOr),
            new InstructionConfig(0,0,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.BXOr,InstructionFuncs.BXOr),
            new InstructionConfig(0,0,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.ShL,InstructionFuncs.ShL),
            new InstructionConfig(0,1,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.ShR,InstructionFuncs.ShR),
            new InstructionConfig(0,1,OpArgType.R,OpArgType.N,OpMode.IABC,OpCodeType.Unm,InstructionFuncs.Unm),
            new InstructionConfig(0,1,OpArgType.R,OpArgType.N,OpMode.IABC,OpCodeType.Bnot,InstructionFuncs.BNot),
            new InstructionConfig(0,1,OpArgType.R,OpArgType.N,OpMode.IABC,OpCodeType.Not,InstructionFuncs.Not),
            new InstructionConfig(0,1,OpArgType.R,OpArgType.N,OpMode.IABC,OpCodeType.Len,InstructionFuncs.Len),
            new InstructionConfig(0,1,OpArgType.R,OpArgType.R,OpMode.IABC,OpCodeType.Concat,InstructionFuncs.Concat),
            new InstructionConfig(0,1,OpArgType.R,OpArgType.N,OpMode.IAsBx,OpCodeType.Jmp,InstructionFuncs.Jmp),
            new InstructionConfig(1,0,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.Eq,InstructionFuncs.Eq),
            new InstructionConfig(1,0,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.Lt,InstructionFuncs.Lt),
            new InstructionConfig(1,0,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.Le,InstructionFuncs.Le),
            new InstructionConfig(1,0,OpArgType.N,OpArgType.U,OpMode.IABC,OpCodeType.Test,InstructionFuncs.Test),
            new InstructionConfig(1,1,OpArgType.R,OpArgType.U,OpMode.IABC,OpCodeType.TestSet,InstructionFuncs.TestSet),
            new InstructionConfig(0,1,OpArgType.U,OpArgType.U,OpMode.IABC,OpCodeType.Call,InstructionFuncs.Call),
            new InstructionConfig(0,1,OpArgType.U,OpArgType.U,OpMode.IABC,OpCodeType.TailCall,InstructionFuncs.TailCall),
            new InstructionConfig(0,0,OpArgType.U,OpArgType.N,OpMode.IABC,OpCodeType.Return,InstructionFuncs.Return),
            new InstructionConfig(0,1,OpArgType.R,OpArgType.N,OpMode.IAsBx,OpCodeType.ForLoop,InstructionFuncs.ForLoop),
            new InstructionConfig(0,1,OpArgType.R,OpArgType.N,OpMode.IAsBx,OpCodeType.ForPrep,InstructionFuncs.ForPrep),
            new InstructionConfig(0,0,OpArgType.N,OpArgType.U,OpMode.IABC,OpCodeType.TForCall),
            new InstructionConfig(0,1,OpArgType.R,OpArgType.N,OpMode.IAsBx,OpCodeType.TForloop),
            new InstructionConfig(0,0,OpArgType.U,OpArgType.U,OpMode.IABC,OpCodeType.Setlist,InstructionFuncs.SetList),
            new InstructionConfig(0,1,OpArgType.U,OpArgType.N,OpMode.IABx,OpCodeType.Closure,InstructionFuncs.Closure),
            new InstructionConfig(0,1,OpArgType.U,OpArgType.N,OpMode.IABC,OpCodeType.Vararg,InstructionFuncs.VarArg),
            new InstructionConfig(0,0,OpArgType.U,OpArgType.U,OpMode.IAx,OpCodeType.ExtraArg),
        };

      
    }

}
