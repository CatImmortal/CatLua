using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// 名字表达式
    /// </summary>
    public class NameExp : BaseExp
    {
        public NameExp(int line, string name) : base(line)
        {
            Name = name;
        }

        public string Name;

    }

}
