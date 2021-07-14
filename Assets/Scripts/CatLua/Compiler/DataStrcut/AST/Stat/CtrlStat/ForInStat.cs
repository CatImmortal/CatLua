using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// 通用for语句
    /// </summary>
    public class ForInStat : BaseStat
    {
        public ForInStat(int lineOfDo, string[] nameList, BaseExp[] expList, Block block)
        {
            LineOfDo = lineOfDo;
            NameList = nameList;
            ExpList = expList;
            Block = block;
        }

        public int LineOfDo;
        public string[] NameList;
        public BaseExp[] ExpList;
        public Block Block;

       
    }

}
