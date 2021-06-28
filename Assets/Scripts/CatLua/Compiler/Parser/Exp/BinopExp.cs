using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// 二元运算表达式
    /// </summary>
    public class BinopExp : BaseExp
    {
        public int Op;
        public BaseExp Exp1;
        public BaseExp Exp2;
    }

}
