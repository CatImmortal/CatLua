using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    public class LuaState
    {
        public LuaState()
        {
            stack = new LuaStack(20);
        }

        private LuaStack stack;

        /// <summary>
        /// 获取栈顶索引
        /// </summary>
        public int Top
        {
            get
            {
                return stack.Top;
            }
        }


        /// <summary>
        /// 获取绝对索引
        /// </summary>
        public int GetAbsIndex(int index)
        {
            return stack.GetAbsIndex(index);
        }

        /// <summary>
        /// 从栈顶弹出n个值
        /// </summary>
        public void Pop(int n)
        {
            for (int i = 0; i < n; i++)
            {
                stack.Pop();
            }
        }

        /// <summary>
        /// 将栈中form位置的值复制到target位置
        /// </summary>
        public void Copy(int form, int target)
        {
            LuaValueUnion value = stack.Get(form);
            stack.Set(target,value);
        }

        /// <summary>
        /// 将index位置的值复制并压入栈顶
        /// </summary>
        public void PushValue(int index)
        {
            LuaValueUnion value = stack.Get(index);
            stack.Push(value);
        }

        /// <summary>
        /// 将栈顶值弹出，然后复制到index位置
        /// </summary>
        public void Replace(int index)
        {
            LuaValueUnion value = stack.Pop();
            stack.Set(index,value);
        }

        /// <summary>
        /// 将栈顶值弹出，然后插入到index位置
        /// </summary>
        public void Insert(int index)
        {
            //Rotate
        }
    }
}

