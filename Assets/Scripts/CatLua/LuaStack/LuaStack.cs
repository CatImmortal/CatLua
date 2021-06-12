using System.Collections;
using System.Collections.Generic;
using System;

namespace CatLua
{
    /// <summary>
    /// Lua虚拟栈
    /// </summary>
    public class LuaStack
    {
        public LuaStack (int size)
        {
            stack = new LuaDataUnion[size + 1];  //0号位不使用，从1开始索引
            Top = 0;
        }

        /// <summary>
        /// 存放Lua值的栈
        /// </summary>
        private LuaDataUnion[] stack;

        /// <summary>
        /// 栈顶索引
        /// </summary>
        public int Top
        {
            get;
            private set;
        }

        /// <summary>
        /// 往栈顶压入值
        /// </summary>
        public void Push(LuaDataUnion value)
        {
            if (Top == stack.Length)
            {
                throw new Exception("stack overflow");
            }

            Top++;
            stack[Top] = value;
            
        }

        /// <summary>
        /// 从栈顶弹出值
        /// </summary>
        public LuaDataUnion Pop()
        {
            if (Top < 1)
            {
                throw new Exception("stack underflow");
            }

            
            LuaDataUnion value = stack[Top];
            stack[Top] = default;
            Top--;

            return value;
        }
        
        /// <summary>
        /// 获取绝对索引
        /// </summary>
        public int GetAbsIndex(int index)
        {
            if (index >= 0)
            {
                return index;
            }

            return Top + 1 + index;  //负数索引就从栈顶减去 [-1]表示[top]
        }

        /// <summary>
        /// 索引是否有效
        /// </summary>
        public bool IsValid(int index)
        {
            int absIndex = GetAbsIndex(index);
            bool result = absIndex > 0 && absIndex <= Top;
            return result;
        }

        /// <summary>
        /// 根据索引从栈中获取值
        /// </summary>
        public LuaDataUnion Get(int index)
        {
            int absIndex = GetAbsIndex(index);
            if (IsValid(index))
            {
                return stack[absIndex];
            }

            return default;
        }

        /// <summary>
        /// 根据索引在栈中设置值
        /// </summary>
        public void Set(int index, LuaDataUnion value)
        {
            int absIndex = GetAbsIndex(index);
            if (IsValid(index))
            {
                stack[absIndex] = value;
            }
        }

        /// <summary>
        /// 将栈中from到to部分的值翻转
        /// </summary>
        public void Reverse(int from,int to)
        {
            while (from < to)
            {
                LuaDataUnion temp = stack[from];
                stack[from] = stack[to];
                stack[to] = temp;

                from++;
                to--;
            }
        }
    }

}
