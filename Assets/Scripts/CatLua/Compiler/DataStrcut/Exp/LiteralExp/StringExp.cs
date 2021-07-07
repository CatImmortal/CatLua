using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// sting表达式
    /// </summary>
    public class StringExp : BaseExp
    {
        public StringExp(int line, string str) : base(line)
        {
            Str = str;
        }

        public string Str;

       
    }

}
