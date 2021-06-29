using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// 一元运算表达式
    /// </summary>
    public class UnopExp : BaseExp
    {
        public UnopExp(int line, int op, BaseExp exp) : base(line)
        {
            Op = op;
            Exp = exp;
        }

        public int Op;
        public BaseExp Exp;


    }

}
