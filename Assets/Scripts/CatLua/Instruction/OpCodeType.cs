using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatLua
{
    /// <summary>
    /// 操作码类型
    /// </summary>
    public enum OpCodeType : byte
    {

        Move,

        LoadK,
        LoadKX,
        LoadBool,
        LoadNil,

        GetUpValue,
        GetTabup,
        GetTable,

        SetTabup,
        SetUpvalue,
        SetTable,

        NewTable,

        Self,
        Add,
        Sub,
        Mul,
        Mod,
        Pow,
        Div,
        IDiv,

        BAnd,
        BOr,
        BXOr,

        ShL,
        ShR,

        Unm,

        BNot,
        Not,

        Len,
        Concat,
        Jmp,
        Eq,

        Lt,
        Le,

        Test,
        TestSet,

        Call,
        TailCall,
        Return,

        ForLoop,
        ForPrep,
        TForCall,
        TForloop,

        SetList,

        Closure,

        Vararg,

        ExtraArg,

    }

}

