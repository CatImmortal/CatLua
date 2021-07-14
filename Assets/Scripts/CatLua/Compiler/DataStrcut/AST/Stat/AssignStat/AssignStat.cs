using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// 赋值语句
    /// </summary>
    public class AssignStat : BaseStat
    {

        public AssignStat(int lastLine, BaseExp[] varList, BaseExp[] expList)
        {
            LastLine = lastLine;
            VarList = varList;
            ExpList = expList;
        }

        public int LastLine;
        public BaseExp[] VarList;
        public BaseExp[] ExpList;

    }

}
