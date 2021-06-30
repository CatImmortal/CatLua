﻿using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// 圆括号表达式
    /// </summary>
    public class ParensExp : BaseExp
    {
        public BaseExp Exp;

        public ParensExp( BaseExp exp) : base(-1)
        {
            Exp = exp;
        }
    }

}
