using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// repeat语句
    /// </summary>
    public class RepeatStat : BaseStat
    {
        public RepeatStat(Block block, BaseExp exp)
        {
            Block = block;
            Exp = exp;
        }

        public Block Block;
        public BaseExp Exp;

        
    }

}
