using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// 二元运算表达式
    /// </summary>
    public class BinopExp : BaseExp
    {
        public TokenType Op;
        public BaseExp Exp1;
        public BaseExp Exp2;

        public BinopExp(int line, TokenType op, BaseExp exp1, BaseExp exp2) : base(line)
        {
            Op = op;
            Exp1 = exp1;
            Exp2 = exp2;
        }

    }

}
