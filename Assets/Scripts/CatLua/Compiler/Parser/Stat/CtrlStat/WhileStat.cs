using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// while语句
    /// </summary>
    public class WhileStat : BaseStat
    {
        public WhileStat(BaseExp exp , Block block)
        {
            Exp = exp;
            Block = block;
           
        }

        public BaseExp Exp;
        public Block Block;
        

       
    }

}
