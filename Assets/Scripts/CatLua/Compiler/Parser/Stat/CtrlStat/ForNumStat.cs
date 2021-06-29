using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// 数值for语句
    /// </summary>
    public class ForNumStat : BaseStat
    {
        public ForNumStat(int lineOfFor, int lineOfDo, string varName, BaseExp initExp, BaseExp limitExp, BaseExp stepExp, Block block)
        {
            LineOfFor = lineOfFor;
            LineOfDo = lineOfDo;
            VarName = varName;
            InitExp = initExp;
            LimitExp = limitExp;
            StepExp = stepExp;
            Block = block;
        }

        public int LineOfFor;
        public int LineOfDo;
        public string VarName;
        public BaseExp InitExp;
        public BaseExp LimitExp;
        public BaseExp StepExp;
        public Block Block;

       
    }

}
