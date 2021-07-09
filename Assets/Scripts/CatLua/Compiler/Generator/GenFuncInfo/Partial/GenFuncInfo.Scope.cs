using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    public partial class GenFuncInfo
    {

        /// <summary>
        /// 当前作用域层次
        /// </summary>
        public int ScopeLv;

        /// <summary>
        /// 记录循环块中break语句的跳转指令 [scope][pc]
        /// </summary>
        public List<List<int>> Breaks = new List<List<int>>();


        /// <summary>
        /// 将break语句对应的跳转指令索引添加到最近的循环块中
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
        /// 进入新的作用域
        /// </summary>
        public void EnterScope(bool breakable)
        {
            ScopeLv++;

            //检查新作用域是否为可被break打断的循环块
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

            //block结束时 修复此block中的break指令

            //取出此block的所有break对应的jmp指令索引
            List<int> pendingBreakJumps = Breaks[Breaks.Count - 1]; 
            Breaks.RemoveAt(Breaks.Count - 1);

            //根据upvalue获取参数a
            int a = GetJmpArgA();

            for (int i = 0; i < pendingBreakJumps.Count; i++)
            {
                //当前作用域已生成的最后一条指令索引PC 和 当前记录的break指令索引pc 的差值，就是正确的sBx参数（即要跳过多少条指令才能到达末尾）
                //在执行jmp指令的时候会使得PC+=sBx，继而从break指令跳转到当前作用域的最后一条指令
                int pc = pendingBreakJumps[i];
                int sBx = PC - pc;

                //修复break的jmp指令的sBx参数
                int instruction = (sBx + Constants.MaxArgSbx) << 14 | a << 6 | (int)OpCodeType.Jmp;
                Instructions[pc] = (uint)instruction;

            }


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



    }

}
