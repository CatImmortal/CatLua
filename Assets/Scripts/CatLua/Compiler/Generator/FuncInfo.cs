using System.Collections;
using System.Collections.Generic;


namespace CatLua
{

    /// <summary>
    /// 函数信息
    /// </summary>
    public class FuncInfo
    {
        /// <summary>
        /// 常量值与对应常量表索引的映射
        /// </summary>
        public Dictionary<LuaConstantUnion, int> Constants;

        /// <summary>
        /// 已使用寄存器数量
        /// </summary>
        public int UsedRegs;

        /// <summary>
        /// 最大寄存器数量
        /// </summary>
        public int MaxRegs;

        /// <summary>
        /// 当前作用域层次
        /// </summary>
        public int ScopeLv;

        /// <summary>
        /// 按顺序记录内部声明的局部变量
        /// </summary>
        public List<LocalVarInfo> LocVars;

        /// <summary>
        /// 当前生效的局部变量
        /// </summary>
        public Dictionary<string, GenLocalVarInfo> LocNames;

        /// <summary>
        /// 获取常量在常量表的索引
        /// </summary>
        public int IndexOfConstant(LuaConstantUnion constant)
        {
            if (Constants.TryGetValue(constant, out int index))
            {
                return index;
            }


            //常量不在表里 放入表里 返回索引
            index = Constants.Count;
            Constants.Add(constant, index);
            return index;
        }

        /// <summary>
        /// 分配寄存器，返回分配到的寄存器的索引
        /// </summary>
        public int AllocReg()
        {
            UsedRegs++;

            if (UsedRegs >= 255)
            {
                throw new System.Exception("已使用寄存器数量 >= 255");
            }

            if (UsedRegs > MaxRegs)
            {
                MaxRegs = UsedRegs;
            }

            return UsedRegs - 1;
        }

        /// <summary>
        /// 回收寄存器
        /// </summary>
        public void FreeReg()
        {
            UsedRegs--;
        }

        /// <summary>
        /// 分配n个寄存器，返回第一个寄存器的索引
        /// </summary>
        public int AllocRegs(int n)
        {
            for (int i = 0; i < n; i++)
            {
                AllocReg();
            }

            return UsedRegs - n;
        }

        /// <summary>
        /// 回收n个寄存器
        /// </summary>
        public void FreeRegs(int n)
        {
            UsedRegs -= n;
        }

        /// <summary>
        /// 进入新的作用域
        /// </summary>
        public void EnterScope()
        {
            ScopeLv++;
        }

        /// <summary>
        /// 离开作用域
        /// </summary>
        public void LeaveScope()
        {
            ScopeLv--;
        }

        /// <summary>
        /// 在当前作用域添加一个局部变量，返回分配的寄存器索引
        /// </summary>
        public int AddLocVar(string name)
        {
            LocNames.TryGetValue(name, out GenLocalVarInfo prev);

            GenLocalVarInfo newVar = new GenLocalVarInfo(prev, name, ScopeLv, AllocReg());

 
        }
    }
}

