using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// do语句
    /// </summary>
    public class DoStat : BaseStat
    {
        public DoStat(Block block)
        {
            this.block = block;
        }

        public Block block;

       
    }

}
