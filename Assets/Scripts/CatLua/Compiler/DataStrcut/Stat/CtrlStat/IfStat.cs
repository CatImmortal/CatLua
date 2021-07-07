using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// if语句
    /// </summary>
    public class IfStat : BaseStat
    {
        public IfStat(BaseExp[] exps, Block[] blocks)
        {
            Exps = exps;
            Blocks = blocks;
        }

        public BaseExp[] Exps;
        public Block[] Blocks;

       
    }

}
