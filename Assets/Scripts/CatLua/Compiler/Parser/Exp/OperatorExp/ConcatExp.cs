using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// 拼接运算表达式
    /// </summary>
    public class ConcatExp : BaseExp
    {
        public ConcatExp(int line, BaseExp[] exps) : base(line)
        {
            Exps = exps;
        }

        public BaseExp[] Exps;

       
    }

}
