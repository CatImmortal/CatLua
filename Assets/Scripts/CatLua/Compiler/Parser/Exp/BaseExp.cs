using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatLua
{
    /// <summary>
    /// 表达式基类
    /// </summary>
    public abstract class BaseExp
    {
        protected BaseExp(int line, int lastLine)
        {
            Line = line;
            LastLine = lastLine;
        }

        public int Line;
        public int LastLine;

       
    }

}


