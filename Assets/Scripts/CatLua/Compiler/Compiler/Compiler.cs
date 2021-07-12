using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    public static partial class Compiler
    {
        /// <summary>
        /// 编译Block
        /// </summary>
        public static void CompileBlock(GenFuncInfo fi,Block node)
        {
            for (int i = 0; i < node.Stats.Length; i++)
            {
                //编译语句
            }

            if (node.ReturnExps != null)
            {
                //编译返回值表达式
            }
        }


    }
}

