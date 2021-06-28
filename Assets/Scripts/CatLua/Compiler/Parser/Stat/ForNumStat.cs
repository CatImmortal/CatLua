using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// 数值for语句
    /// </summary>
    public class ForNumStat
    {
        public int LineOfFor;
        public int LineOfDo;
        public string VarName;
        public BaseExp InitExp;
        public BaseExp LimitExp;
        public BaseExp StepExp;
        public Block Block;
    }

}
