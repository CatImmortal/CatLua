using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// 一元运算表达式
    /// </summary>
    public class UnopExp : BaseExp
    {
        public TokenType Op;
        public BaseExp Exp;

        public UnopExp(int line, TokenType op, BaseExp exp) : base(line)
        {
            Op = op;
            Exp = exp;
        }




    }

}
