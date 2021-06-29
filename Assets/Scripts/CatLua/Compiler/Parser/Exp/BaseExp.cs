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
        protected BaseExp(int line)
        {
            Line = line;
        }

        public int Line;
    }

}


