using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    public partial class LuaState
    {


        /// <summary>
        /// 获取栈顶索引
        /// </summary>
        public int Top
        {
            get
            {
                return globalStack.Top;
            }
        }

        /// <summary>
        /// 从栈顶弹出n个值
        /// </summary>
        public void Pop(int n)
        {
            for (int i = 0; i < n; i++)
            {
                globalStack.Pop();
            }
        }

        /// <summary>
        /// 将栈中form位置的值复制到target位置
        /// </summary>
        public void Copy(int form, int target)
        {
            LuaDataUnion value = globalStack.Get(form);
            globalStack.Set(target, value);
        }

        /// <summary>
        /// 将index位置的值复制并压入栈顶
        /// </summary>
        public void CopyAndPush(int index)
        {
            LuaDataUnion value = globalStack.Get(index);
            globalStack.Push(value);
        }

        /// <summary>
        /// 将栈顶值弹出，然后复制到index位置
        /// </summary>
        public void PopAndCopy(int index)
        {
            LuaDataUnion value = globalStack.Pop();
            globalStack.Set(index, value);
        }

        /// <summary>
        /// 将栈顶值弹出，然后插入到index位置
        /// </summary>
        public void PopAndInsert(int index)
        {
            Rotate(index, 1);
        }

        /// <summary>
        /// 删除index位置的值，然后将上面的值都往下移1位
        /// </summary>
        public void Remove(int index)
        {
            Rotate(index, -1);
            Pop(1);
        }

        /// <summary>
        /// 将index到top的值往栈顶方向旋转n位，如果n是负数，就朝栈底方向旋转
        /// </summary>
        public void Rotate(int index, int n)
        {
            if (n == 0)
            {
                return;
            }

            int absIndex = globalStack.GetAbsIndex(index);

            int middle;
            if (n > 0)
            {
                //向栈顶旋转
                middle = Top - n;
            }
            else
            {
                //向栈底旋转
                middle = absIndex - n - 1;
            }

            //翻转三次
            globalStack.Reverse(absIndex, middle);
            globalStack.Reverse(middle + 1, Top);
            globalStack.Reverse(absIndex, Top);
        }

        /// <summary>
        /// 设置栈顶索引
        /// </summary>
        public void SetTop(int index)
        {
            int newTop = globalStack.GetAbsIndex(index);
            int n = newTop - Top;
            if (n == 0)
            {
                return;
            }

            if (n > 0)
            {
                //需要增加栈顶
                //压入n个值
                int num = n;
                for (int i = 0; i < num; i++)
                {
                    globalStack.Push(default);
                }
               
            }
            else
            {
                //需要缩减栈顶
                //弹出-n个值
                int num = -n;
                for (int i = 0; i < num; i++)
                {
                    globalStack.Pop();
                }
            }

        }

        /// <summary>
        /// 压入nil值
        /// </summary>
        public void Push()
        {
            globalStack.Push(default);
        }

        /// <summary>
        /// 压入bool值
        /// </summary>
        public void Push(bool b)
        {
            globalStack.Push(new LuaDataUnion(LuaDataType.Boolean, boolean: b));
        }

        /// <summary>
        /// 压入integer值
        /// </summary>
        public void Push(long l)
        {
            globalStack.Push(new LuaDataUnion(LuaDataType.Integer, integer: l));
        }

        /// <summary>
        /// 压入number值
        /// </summary>
        public void Push(double d)
        {
            globalStack.Push(new LuaDataUnion(LuaDataType.Number, number: d));
        }

        /// <summary>
        /// 压入string值
        /// </summary>
        public void Push(string str)
        {
            globalStack.Push(new LuaDataUnion(LuaDataType.String, str: str));
        }

        /// <summary>
        /// 压入table值
        /// </summary>
        public void Push(LuaTable table)
        {
            globalStack.Push(new LuaDataUnion(LuaDataType.Table, table: table));
        }

        /// <summary>
        /// 压入Closure值
        /// </summary>
        public void Push(Closure closure)
        {
            globalStack.Push(new LuaDataUnion(LuaDataType.Function, closure: closure));
        }
    }
}