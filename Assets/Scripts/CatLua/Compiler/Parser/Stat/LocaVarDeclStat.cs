using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// 局部变量声明语句
    /// </summary>
    public class LocaVarDeclStat
    {
        public int LastLine;
        public string[] NameList;
        public BaseExp[] ExpList;
    }

}
