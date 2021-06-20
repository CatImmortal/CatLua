using System.Collections;
using System.Collections.Generic;


namespace CatLua
{
    public class Upvalue
    {
        public Upvalue(LuaDataUnion value,bool isOpen = false,int globalStackIndex = 0)
        {
            Value = value;
            IsOpen = isOpen;
            GlobalStackIndex = globalStackIndex;
        }

        /// <summary>
        /// 是否为开放状态（引用到的Lua值是否在栈中）
        /// </summary>

        public bool IsOpen;

        public LuaDataUnion Value
        {
            get;
            private set;
        }

        /// <summary>
        /// 引用到的Lua值在栈中的全局索引
        /// </summary>
        public int GlobalStackIndex
        {
            get;
            private set;
        }



        public void SetValue(LuaDataUnion value,LuaState vm)
        {

            Value = value;

            if (IsOpen)
            {
                //upvalue处于开放状态 意味着引用的Lua值在栈中，需要更新栈
                vm.Push(Value);
                vm.PopAndCopy(GlobalStackIndex);
            }
        }
    }

}
