using System.Collections;
using System.Collections.Generic;
using System;

namespace CatLua
{
    /// <summary>
    /// 指令配置
    /// </summary>
    public class InstructionConfig
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
            //                    T A      B           C           mode          type            func
            new InstructionConfig(0,1,OpArgType.R,OpArgType.N,OpMode.IABC,OpCodeType.Move,InstructionFuncs.MoveFunc),
            new InstructionConfig(0,1,OpArgType.K,OpArgType.N,OpMode.IABx,OpCodeType.LoadK,InstructionFuncs.LoadKFunc),
            new InstructionConfig(0,1,OpArgType.N,OpArgType.N,OpMode.IABx,OpCodeType.LoadKX,InstructionFuncs.LoadKXFunc),
            new InstructionConfig(0,1,OpArgType.U,OpArgType.U,OpMode.IABC,OpCodeType.LoadBool,InstructionFuncs.LoadBoolFunc),
            new InstructionConfig(0,1,OpArgType.U,OpArgType.N,OpMode.IABC,OpCodeType.LoadNil,InstructionFuncs.LoadNilFunc),
            new InstructionConfig(0,1,OpArgType.U,OpArgType.N,OpMode.IABC,OpCodeType.GetUpValue,InstructionFuncs.GetUpvalueFunc),
            new InstructionConfig(0,1,OpArgType.U,OpArgType.K,OpMode.IABC,OpCodeType.GetTabup,InstructionFuncs.GetTabUpFunc),
            new InstructionConfig(0,1,OpArgType.R,OpArgType.K,OpMode.IABC,OpCodeType.GetTable,InstructionFuncs.GetTableFunc),
            new InstructionConfig(0,0,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.SetTabup,InstructionFuncs.SetTabUpFunc),
            new InstructionConfig(0,0,OpArgType.U,OpArgType.N,OpMode.IABC,OpCodeType.SetUpvalue,InstructionFuncs.SetUpvalueFunc),
            new InstructionConfig(0,0,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.SetTable,InstructionFuncs.SetTableFunc),
            new InstructionConfig(0,1,OpArgType.U,OpArgType.U,OpMode.IABC,OpCodeType.NewTable,InstructionFuncs.NewTableFunc),
            new InstructionConfig(0,1,OpArgType.R,OpArgType.K,OpMode.IABC,OpCodeType.Self,InstructionFuncs.SelfFunc),
            new InstructionConfig(0,1,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.Add,InstructionFuncs.AddFunc),
            new InstructionConfig(0,1,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.Sub,InstructionFuncs.SubFunc),
            new InstructionConfig(0,1,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.Mul,InstructionFuncs.MulFunc),
            new InstructionConfig(0,1,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.Mod,InstructionFuncs.ModFunc),
            new InstructionConfig(0,1,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.Pow,InstructionFuncs.PowFunc),
            new InstructionConfig(0,1,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.Div,InstructionFuncs.DivFunc),
            new InstructionConfig(0,1,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.IDiv,InstructionFuncs.IDivFunc),
            new InstructionConfig(0,1,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.BAnd,InstructionFuncs.BAndFunc),
            new InstructionConfig(0,1,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.BOr,InstructionFuncs.BOrFunc),
            new InstructionConfig(0,0,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.BXOr,InstructionFuncs.BXOrFunc),
            new InstructionConfig(0,0,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.ShL,InstructionFuncs.ShLFunc),
            new InstructionConfig(0,1,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.ShR,InstructionFuncs.ShRFunc),
            new InstructionConfig(0,1,OpArgType.R,OpArgType.N,OpMode.IABC,OpCodeType.Unm,InstructionFuncs.UnmFunc),
            new InstructionConfig(0,1,OpArgType.R,OpArgType.N,OpMode.IABC,OpCodeType.Bnot,InstructionFuncs.BNotFunc),
            new InstructionConfig(0,1,OpArgType.R,OpArgType.N,OpMode.IABC,OpCodeType.Not,InstructionFuncs.NotFunc),
            new InstructionConfig(0,1,OpArgType.R,OpArgType.N,OpMode.IABC,OpCodeType.Len,InstructionFuncs.LenFunc),
            new InstructionConfig(0,1,OpArgType.R,OpArgType.R,OpMode.IABC,OpCodeType.Concat,InstructionFuncs.ConcatFunc),
            new InstructionConfig(0,1,OpArgType.R,OpArgType.N,OpMode.IAsBx,OpCodeType.Jmp,InstructionFuncs.JmpFunc),
            new InstructionConfig(1,0,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.Eq,InstructionFuncs.EqFunc),
            new InstructionConfig(1,0,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.Lt,InstructionFuncs.LtFunc),
            new InstructionConfig(1,0,OpArgType.K,OpArgType.K,OpMode.IABC,OpCodeType.Le,InstructionFuncs.LeFunc),
            new InstructionConfig(1,0,OpArgType.N,OpArgType.U,OpMode.IABC,OpCodeType.Test,InstructionFuncs.TestFunc),
            new InstructionConfig(1,1,OpArgType.R,OpArgType.U,OpMode.IABC,OpCodeType.TestSet,InstructionFuncs.TestSetFunc),
            new InstructionConfig(0,1,OpArgType.U,OpArgType.U,OpMode.IABC,OpCodeType.Call,InstructionFuncs.CallFunc),
            new InstructionConfig(0,1,OpArgType.U,OpArgType.U,OpMode.IABC,OpCodeType.TailCall,InstructionFuncs.TailCallFunc),
            new InstructionConfig(0,0,OpArgType.U,OpArgType.N,OpMode.IABC,OpCodeType.Return,InstructionFuncs.ReturnFunc),
            new InstructionConfig(0,1,OpArgType.R,OpArgType.N,OpMode.IAsBx,OpCodeType.ForLoop,InstructionFuncs.ForLoopFunc),
            new InstructionConfig(0,1,OpArgType.R,OpArgType.N,OpMode.IAsBx,OpCodeType.ForPrep,InstructionFuncs.ForPrepFunc),
            new InstructionConfig(0,0,OpArgType.N,OpArgType.U,OpMode.IABC,OpCodeType.TForCall,InstructionFuncs.TForCallFunc),
            new InstructionConfig(0,1,OpArgType.R,OpArgType.N,OpMode.IAsBx,OpCodeType.TForloop,InstructionFuncs.TForLoopFunc),
            new InstructionConfig(0,0,OpArgType.U,OpArgType.U,OpMode.IABC,OpCodeType.Setlist,InstructionFuncs.SetListFunc),
            new InstructionConfig(0,1,OpArgType.U,OpArgType.N,OpMode.IABx,OpCodeType.Closure,InstructionFuncs.ClosureFunc),
            new InstructionConfig(0,1,OpArgType.U,OpArgType.N,OpMode.IABC,OpCodeType.Vararg,InstructionFuncs.VarArgFunc),
            new InstructionConfig(0,0,OpArgType.U,OpArgType.U,OpMode.IAx,OpCodeType.ExtraArg),
        };

      
    }

}
