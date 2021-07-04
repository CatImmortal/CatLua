using System.Collections;
using System.Collections.Generic;


namespace CatLua
{

    /// <summary>
    /// 函数信息
    /// </summary>
    public class GenFuncInfo
    {
        /// <summary>
        /// 常量值与对应常量表索引的映射
        /// </summary>
        public Dictionary<LuaConstantUnion, int> ConstantDict;

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
        public List<GenLocalVarInfo> LocalVars;

        /// <summary>
        /// 当前生效的局部变量
        /// </summary>
        public Dictionary<string, GenLocalVarInfo> activeLocalVarDict;

        /// <summary>
        /// 记录循环块中break语句的跳转指令 [scope][pc]
        /// </summary>
        public List<List<int>> Breaks;

        /// <summary>
        /// 直接外围函数
        /// </summary>
        public GenFuncInfo Parent;

        /// <summary>
        /// 该函数的所有upvalue
        /// </summary>
        public Dictionary<string, GenUpvalueInfo> UpvalueDict;

        /// <summary>
        /// 指令
        /// </summary>
        public List<uint> Instructions;

        /// <summary>
        /// 已经生成的最后一条指令的索引
        /// </summary>
        public int PC
        {
            get
            {
                return Instructions.Count - 1;
            }
        }

        /// <summary>
        /// 获取常量在常量表的索引
        /// </summary>
        public int IndexOfConstant(LuaConstantUnion constant)
        {
            if (ConstantDict.TryGetValue(constant, out int index))
            {
                return index;
            }


            //常量不在表里 放入表里 返回索引
            index = ConstantDict.Count;
            ConstantDict.Add(constant, index);
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
        public void EnterScope(bool breakable)
        {
            ScopeLv++;

            if (breakable)
            {
                //循环块
                Breaks.Add(new List<int>());
            }
            else
            {
                //非循环块
                Breaks.Add(null);
            }
        }

        /// <summary>
        /// 离开作用域
        /// </summary>
        public void ExitScope()
        {
            ScopeLv--;

            //修复跳转指令todo：
            List<int> lastBreakJumps = Breaks[Breaks.Count - 1];
            Breaks.RemoveAt(Breaks.Count - 1);

            
            //删除离开作用域的局部变量
            List<GenLocalVarInfo> needRemoveVars = new List<GenLocalVarInfo>();
            foreach (KeyValuePair<string, GenLocalVarInfo> item in activeLocalVarDict)
            {
                if (item.Value.ScopeLv > ScopeLv)
                {
                    needRemoveVars.Add(item.Value);
                }
            }

            for (int i = 0; i < needRemoveVars.Count; i++)
            {
                RemoveLocalVar(needRemoveVars[i]);
            }
        }

        /// <summary>
        /// 在当前作用域添加一个局部变量，返回分配的寄存器索引
        /// </summary>
        public int AddLocalVar(string name)
        {
            //变量名name当前表示的局部变量 作为新的同名局部变量的prev
            activeLocalVarDict.TryGetValue(name, out GenLocalVarInfo prev);

            GenLocalVarInfo newVar = new GenLocalVarInfo(prev, name, ScopeLv, AllocReg());

            LocalVars.Add(newVar);
            activeLocalVarDict[name] = newVar;

            return newVar.Slot;
        }

        /// <summary>
        /// 删除局部变量
        /// </summary>
        public void RemoveLocalVar(GenLocalVarInfo localVal)
        {
            FreeReg();

            if (localVal.Prev == null)
            {
                //没有同名prev 直接删除
                activeLocalVarDict.Remove(localVal.Name);
                return;
            }


            if (localVal.Prev.ScopeLv == localVal.ScopeLv)
            {
                //prev的作用域和当前要删除的局部变量的作用域相同 递归删除prev
                //这样才能处理prev的prev
                RemoveLocalVar(localVal.Prev);

            }
            else
            {
                //否则修改变量名关联的局部变量为prev
                activeLocalVarDict[localVal.Name] = localVal.Prev;
            }


        }

        /// <summary>
        /// 返回name绑定的寄存器索引
        /// </summary>
        public int SlotOfLocalVar(string name)
        {
            if (activeLocalVarDict.TryGetValue(name,out GenLocalVarInfo varInfo))
            {
                return varInfo.Slot;
            }

            return -1;
        }

        /// <summary>
        /// 将break语句对应的跳转指令添加到最近的循环块中
        /// </summary>
        public void AddBreakJump(int pc)
        {
            for (int i = ScopeLv - 1; i >= 0; i--)
            {
                if (Breaks[i] != null)
                {
                    Breaks[i].Add(pc);
                    return;
                }
            }

            throw new System.Exception("AddBreakJump调用失败，找不到循环块");
        }

        /// <summary>
        /// 返回name绑定的upvalue索引
        /// </summary>
        public int IndexOfUpvalue(string name)
        {
            if (UpvalueDict.TryGetValue(name,out GenUpvalueInfo upvalue))
            {
                return upvalue.Index;
            }

            //name没和upvalue绑定 尝试绑定

            if (Parent != null)
            {
                //尝试从直接外围函数的局部变量进行绑定
                if (Parent.activeLocalVarDict.TryGetValue(name,out GenLocalVarInfo localVar))
                {
                    int index = UpvalueDict.Count;
                    GenUpvalueInfo info = new GenUpvalueInfo(localVar.Slot, -1, index);
                    UpvalueDict.Add(name, info);

                    localVar.Captured = true;
                    return index;
                }

                //尝试从直接外围函数的Upvalue表进行绑定
                int upvalueIndex = Parent.IndexOfUpvalue(name);
                if (upvalueIndex >= 0)
                {
                    int index = UpvalueDict.Count;
                    GenUpvalueInfo info = new GenUpvalueInfo(localVar.Slot, upvalueIndex, index);
                    UpvalueDict.Add(name, info);
                    return index;
                }
            }

            //绑定失败
            return -1;
        }

        /// <summary>
        /// 生成ABC模式的编码指令
        /// </summary>
        public void EmitABC(int opcode,int a,int b,int c)
        {
            int i = b << 23 | c << 14 | a << 6 | opcode;
            Instructions.Add((uint)i);
        }

        /// <summary>
        /// 生成ABx模式的编码指令
        /// </summary>
        public void EmitABx(int opcode, int a, int bx)
        {
            int i =  bx << 14 | a << 6 | opcode;
            Instructions.Add((uint)i);
        }

        /// <summary>
        /// 生成AsBx模式的编码指令
        /// </summary>
        public void EmitAsBx(int opcode, int a, int sbx)
        {
            int i = (sbx + Constants.MaxArgSbx) << 14 | a << 6 | opcode;
            Instructions.Add((uint)i);
        }

        /// <summary>
        /// 生成Ax模式的编码指令
        /// </summary>
        public void EmitAx(int opcode, int ax)
        {
            int i =  ax << 6 | opcode;
            Instructions.Add((uint)i);
        }

        /// <summary>
        /// 将pc位置的指令的sBx操作数设置为指定的sBx(跳转指令用)
        /// </summary>
        public void FixSbx(int sBx,int pc)
        {
            uint i = Instructions[pc];
            i = i << 18 >> 18; //清除sBx操作数
            i = i | (uint)(sBx + Constants.MaxArgSbx) << 14;  //重设sBx
            Instructions[pc] = i;
        }
    }
}

