using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// 局部变量信息
    /// </summary>
    public class GenLocalVarInfo
    {

        public GenLocalVarInfo(GenLocalVarInfo prev, string name, int scopeLv, int slot)
        {
            Prev = prev;
            Name = name;
            ScopeLv = scopeLv;
            Slot = slot;
        }

        /// <summary>
        /// 前一个同名局部变量
        /// </summary>
        public GenLocalVarInfo Prev;

        /// <summary>
        /// 绑定的变量名
        /// </summary>
        public string Name;

        /// <summary>
        /// 所在的作用域层次
        /// </summary>
        public int ScopeLv;

        /// <summary>
        /// 绑定的寄存器索引
        /// </summary>
        public int Slot;

        /// <summary>
        /// 是否被闭包捕获
        /// </summary>
        public bool Captured;


    }

}
