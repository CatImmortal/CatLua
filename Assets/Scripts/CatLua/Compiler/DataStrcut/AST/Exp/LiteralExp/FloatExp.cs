using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// float表达式
    /// </summary>
    public class FloatExp : BaseExp
    {
        public FloatExp(int line, double val) : base(line)
        {
            Val = val;
        }

        public double Val;


    }

}
