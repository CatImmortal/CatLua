using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// 函数定义表达式
    /// </summary>
    public class FuncDefExp : BaseExp
    {
        public FuncDefExp(int line, int lastLine, string[] paramList, bool isVararg, Block block) : base(line)
        {
            LastLine = lastLine;
            ParamList = paramList;
            IsVararg = isVararg;
            Block = block;
        }

        public int LastLine;
        public string[] ParamList;
        public bool IsVararg;
        public Block Block;




    }

}
