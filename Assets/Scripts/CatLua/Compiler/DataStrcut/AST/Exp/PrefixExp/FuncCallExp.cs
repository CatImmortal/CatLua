using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// 函数调用表达式
    /// </summary>
    public class FuncCallExp : BaseExp
    {
        public FuncCallExp(int line, int lastLine, BaseExp prefixExp, StringExp nameExp, BaseExp[] args) : base(line)
        {
            LastLine = lastLine;
            PrefixExp = prefixExp;
            NameExp = nameExp;
            Args = args;
        }

        public int LastLine;
        public BaseExp PrefixExp;
        public StringExp NameExp;
        public BaseExp[] Args;


    }

}
