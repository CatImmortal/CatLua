using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// 局部函数定义语句
    /// </summary>
    public class LocalFuncDefStat : BaseStat
    {
        public LocalFuncDefStat(string name, BaseExp funcDefExp)
        {
            Name = name;
            FuncDefExp = funcDefExp;
        }

        public string Name;
        public BaseExp FuncDefExp;


    }

}
