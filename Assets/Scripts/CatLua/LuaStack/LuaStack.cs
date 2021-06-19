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
            stack = new LuaDataUnion[size];
            Top = -1;
        }

        /// <summary>
        /// 存放Lua数据的可索引的栈
        /// </summary>
        private LuaDataUnion[] stack;

        /// <summary>
        /// 栈顶索引
        /// </summary>
        public int Top;

        public override string ToString()
        {
            string s = "";


            for (int i = 0; i <= Top; i++)
            {

                LuaDataUnion value = Get(i);

                s += $"[{value}]";

            }
            return s;
        }

        /// <summary>
        /// 往栈顶压入值
        /// </summary>
        public void Push(LuaDataUnion data)
        {
            if (Top == stack.Length - 1)
            {
                throw new Exception("stack overflow");
            }

            Top++;
            stack[Top] = data;
            //UnityEngine.Debug.Log("push top ==" + Top + ",data ==" + data);
        }

        /// <summary>
        /// 往栈顶压入datas[startIndex]开始的n个值
        /// 若n为-1则startIndex开始的全部值都压入，若n>可压入的值的数量，则多余部分压入nil值
        /// </summary>
        public void PushN(LuaDataUnion[] datas,int startIndex = 0,int n = -1)
        {
            if (n == -1)
            {
                n = datas.Length - startIndex;
            }

            for (int i = 0; i < n; i++)
            {
                int index = startIndex + i;
                if (index < datas.Length)
                {
                    Push(datas[index]);
                }
                else
                {
                    Push(new LuaDataUnion(LuaDataType.Nil));
                }
            }
        }

        /// <summary>
        /// 从栈顶弹出值
        /// </summary>
        public LuaDataUnion Pop()
        {
            if (Top < 0)
            {
                throw new Exception("stack underflow");
            }

            
            LuaDataUnion data = stack[Top];
            stack[Top] = LuaDataUnion.Nil;
            Top--;
            //UnityEngine.Debug.Log("pop top ==" + Top + ",data ==" + data);
            return data;
        }
        
        /// <summary>
        /// 从栈顶弹出n个值，栈顶的在数组末尾
        /// </summary>
        public LuaDataUnion[] PopN(int n)
        {
            LuaDataUnion[] datas = new LuaDataUnion[n];

            //栈顶的要放在数组末尾 所以要倒着来
            for (int i = n - 1; i >= 0; i--)
            {
                datas[i] = Pop();
            }

            return datas;
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
            bool result = absIndex >= 0 && absIndex <= Top;
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

            return LuaDataUnion.Nil;
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
