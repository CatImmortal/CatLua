using System.Collections;
using System.Collections.Generic;
using System;

namespace CatLua
{
    public partial class LuaState
    {
        /// <summary>
        /// 将C#函数转为C#闭包后压入栈
        /// </summary>
        public void PushCSFunc(Func<LuaState, int, int> csFunc, int upvalueNum = 0)
        {
            Closure c = new Closure(csFunc);

            //处理upvalue
            for (int i = upvalueNum; i > 0; i--)
            {
                LuaDataUnion data = Pop();
                c.Upvalues[upvalueNum - 1] = new Upvalue(data);
            }

            Push(c);
        }

        /// <summary>
        /// 栈中index位置的值是否为C#闭包
        /// </summary>
        public bool IsCSFunc(int index)
        {
            LuaDataUnion data = globalStack.Get(index);
            bool result = data.Type == LuaDataType.Function && data.Closure.CSFunc != null;
            return result;
        }

        /// <summary>
        /// 将栈中index位置的值转换为C#函数并返回
        /// </summary>
        public Func<LuaState, int, int> ToCSFunc(int index)
        {
            LuaDataUnion data = globalStack.Get(index);
            if (data.Type == LuaDataType.Function && data.Closure.CSFunc != null)
            {
                return data.Closure.CSFunc;
            }
            return null;
        }
    }

}
