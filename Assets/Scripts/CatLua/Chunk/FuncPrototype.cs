using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatLua
{
    /// <summary>
    /// 函数原型
    /// </summary>
    public class FuncPrototype
    {
        /// <summary>
        /// 源文件名
        /// </summary>
        public string Source;

        /// <summary>
        /// 起始行号
        /// </summary>
        public uint LineDefined;

        /// <summary>
        /// 结束行号
        /// </summary>
        public uint LastLineDefined;

        /// <summary>
        /// 固定参数个数
        /// </summary>
        public byte NumParams;

        /// <summary>
        /// 是否有变长参数
        /// </summary>
        public byte IsVararg;

        /// <summary>
        /// 寄存器数量
        /// </summary>
        public byte MaxStackSize;

        /// <summary>
        /// 指令表
        /// </summary>
        public uint[] Code;

        /// <summary>
        /// 常量表
        /// </summary>
        public LuaConstantUnion[] Constants;

        /// <summary>
        /// upvalue表
        /// </summary>
        public Upvalue[] Upvalues;

        /// <summary>
        /// 子函数原型表
        /// </summary>
        public FuncPrototype[] Protos;

        /// <summary>
        /// 行号表（每条指令对应的行号）
        /// </summary>
        public uint[] LineInfo;

        /// <summary>
        /// 局部变量表
        /// </summary>
        public LocalVar[] Locvars;

        /// <summary>
        /// upvalue名列表
        /// </summary>
        public string[] UpvalueNames;

    }
}

