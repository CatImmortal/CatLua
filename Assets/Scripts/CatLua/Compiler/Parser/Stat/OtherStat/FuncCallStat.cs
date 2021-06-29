using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// 函数调用语句
    /// </summary>
    public class FuncCallStat : BaseStat
    {
        public FuncCallStat(FuncCallExp exp)
        {
            PrefixExp = exp.PrefixExp;
            NameExp = exp.NameExp;
            Args = exp.Args;
        }

        public BaseExp PrefixExp;
        public StringExp NameExp;
        public BaseExp[] Args;
    }

}
