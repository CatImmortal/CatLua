using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// 表构造器表达式
    /// </summary>
    public class TableConstructorExp : BaseExp
    {
        public BaseExp[] KeyExps;
        public BaseExp[] ValueExps;
    }

}
