using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// Token类型
    /// </summary>
    public enum TokenType
    {
        Eof,  //end-of-file
        Vararg,  //...

        SepSemi,  //;
        SepComma,  //,
        SepDot,  //.
        SepColon,  //:
        SepLable,  //::
        SepLparen,  //(
        SepRparen,  //)
        SepLbrack,  //[
        SepRbrack,  //]
        SepLcurly,  //{
        SepRcurly,  //}


        OpAsssign,  //=
        OpMinus,  //-
        OpWave,  //~
        OpAdd,  //+
        OpMul,  //*
        OpDiv,  // /
        OpIDiv,  // //
        OpPow,  //^
        OpMod,  //%
        OpBAnd,  //&
        OpBOr,  //|
        OpShR,  //>>
        OpShL,  //<<
        OpConcat,  //..
        OpLt,  //<
        OpLe,  //<=
        OpGt,  //>
        OpGe,  //>=
        OpEq,  //==
        OpNe,  //~=
        OpLen,  //#
        OpAnd,  //and
        OpOr,  //or
        OpNot,  //not


        KwBreak,  //break
        KwDo,  //do
        KwElse,  //else
        KwElseif,  //elseif
        KwEnd,  //end
        KwFalse,  //false
        KwFor,  //for
        KwFunction,  //function
        KwGoto,  //goto
        KwIf,  //if
        KwIn,  //in
        KwLocal,  //local
        KwNil,  //nil
        KwRepeat,  //repeat
        KwReturn,  //return
        KwThen,  //then
        KwTrue,  //true
        KwUntil,  //until
        KwWhile,  //while

        Identifier,  //identifier
        Number,  //number literal
        String,   //string literal

        OpUnm = OpMinus,
        OpSub = OpMinus,
        OpBNot = OpWave,
        OpBXor = OpWave,
    }
}


