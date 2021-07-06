using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    public partial class GenFuncInfo
    {
        /// <summary>
        /// 按顺序记录内部声明的局部变量
        /// </summary>
        public List<GenLocalVarInfo> LocalVars = new List<GenLocalVarInfo>();

        /// <summary>
        /// 当前生效的局部变量
        /// </summary>
        public Dictionary<string, GenLocalVarInfo> activeLocalVarDict = new Dictionary<string, GenLocalVarInfo>();

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
            if (activeLocalVarDict.TryGetValue(name, out GenLocalVarInfo varInfo))
            {
                return varInfo.Slot;
            }

            return -1;
        }
    }

}
