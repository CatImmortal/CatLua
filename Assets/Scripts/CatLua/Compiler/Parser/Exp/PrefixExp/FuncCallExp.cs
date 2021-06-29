using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// 函数调用表达式
    /// </summary>
    public class FuncCallExp : BaseExp
    {
        public BaseExp PrefixExp;
        public StringExp NameExp;
        public BaseExp[] Args;
    }

}
