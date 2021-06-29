using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// integer表达式
    /// </summary>
    public class IntegerExp : BaseExp
    {
        public IntegerExp(int line, int lastLine, long val) : base(line, lastLine)
        {
            Val = val;
        }

        public long Val;

      
    }

}
