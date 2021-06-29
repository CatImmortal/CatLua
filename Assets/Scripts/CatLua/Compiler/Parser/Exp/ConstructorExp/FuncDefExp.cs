using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// 函数定义表达式
    /// </summary>
    public class FuncDefExp : BaseExp
    {
        public string[] ParamList;
        public bool IsVararg;
        public Block Block;
    }

}
