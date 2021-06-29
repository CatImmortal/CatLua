using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// 局部变量声明语句
    /// </summary>
    public class LocalVarDeclStat : BaseStat
    {
        public LocalVarDeclStat(int lastLine, string[] nameList, BaseExp[] expList)
        {
            LastLine = lastLine;
            NameList = nameList;
            ExpList = expList;
        }

        public int LastLine;
        public string[] NameList;
        public BaseExp[] ExpList;


    }

}
