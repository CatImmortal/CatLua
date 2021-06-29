using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// 一元运算表达式
    /// </summary>
    public class UnopExp : BaseExp
    {
        public int Op;
        public BaseExp Exp;
    }

}
