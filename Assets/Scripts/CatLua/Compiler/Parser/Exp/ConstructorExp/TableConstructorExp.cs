using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// 表构造器表达式
    /// </summary>
    public class TableConstructorExp : BaseExp
    {
        public TableConstructorExp(int line, int lastLine, BaseExp[] keyExps, BaseExp[] valueExps) : base(line)
        {
            LastLine = lastLine;
            KeyExps = keyExps;
            ValueExps = valueExps;
        }

        public int LastLine;
        public BaseExp[] KeyExps;
        public BaseExp[] ValueExps;

    }

}
