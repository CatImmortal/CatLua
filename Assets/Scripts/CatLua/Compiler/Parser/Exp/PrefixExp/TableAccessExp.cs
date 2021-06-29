using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// 表访问表达式 table[k]
    /// </summary>
    public class TableAccessExp : BaseExp
    {
        public TableAccessExp(int lastLine, BaseExp prefixExp, BaseExp keyExp) : base(-1)
        {
            lastLine = LastLine;
            PrefixExp = prefixExp;
            KeyExp = keyExp;
        }

        public int LastLine;
        public BaseExp PrefixExp;
        public BaseExp KeyExp;



    }

}
