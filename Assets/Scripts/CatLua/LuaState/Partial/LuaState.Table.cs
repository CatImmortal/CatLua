using System.Collections;
using System.Collections.Generic;
using System;

namespace CatLua
{
    public partial class LuaState
    {
        /// <summary>
        /// 创建table，并压入栈顶
        /// </summary>
        public void CreateTable(int arrSize = 0, int dictSize = 0)
         {
            LuaTable table = new LuaTable(arrSize, dictSize);
            Push(table);
        }



        /// <summary>
        /// 从index位置获取table，压入table[key]
        /// </summary>
        public LuaDataType PushTableValue(int index, string strKey)
        {
            LuaDataUnion data = globalStack.Get(index);
            LuaDataUnion key = Factory.NewString(strKey);
            return PushTableValue(data, key);
        }

        /// <summary>
        /// 从index位置获取table，压入table[key]
        /// </summary>
        public LuaDataType PushTableValue(int index, long integerKey)
        {
            LuaDataUnion data = globalStack.Get(index);
            LuaDataUnion key = Factory.NewInteger(integerKey);
            return PushTableValue(data, key);
        }

        /// <summary>
        /// 从index位置获取table，从栈顶弹出key，压入table[key]
        /// </summary>
        public LuaDataType PushTableValue(int index)
        {
            LuaDataUnion data = globalStack.Get(index);
            LuaDataUnion key = globalStack.Pop();
            return PushTableValue(data, key);
        }

        /// <summary>
        /// 压入table[key]
        /// </summary>
        private LuaDataType PushTableValue(LuaDataUnion data, LuaDataUnion key)
        {
            if (data.Type != LuaDataType.Table)
            {
                throw new Exception("GetTableValue的data不是table");
            }

            LuaDataUnion value = data.Table[key];
            globalStack.Push(value);
            return value.Type;
        }

        /// <summary>
        /// 从index位置获取table，从栈顶弹出key value，table[key]=value
        /// </summary>
        public void SetTableValue(int index)
        {
            LuaDataUnion data = globalStack.Get(index);
            LuaDataUnion value = globalStack.Pop();
            LuaDataUnion key = globalStack.Pop();
            SetTableValue(index,data, key, value);
        }

        /// <summary>
        /// 从index位置获取table,从栈顶弹出value，table[key]=value
        /// </summary>
        public void SetTableValue(int index, string strKey)
        {
            LuaDataUnion data = globalStack.Get(index);
            LuaDataUnion key = Factory.NewString(strKey);
            LuaDataUnion value = globalStack.Pop();
            SetTableValue(index,data, key, value);
        }

        /// <summary>
        /// 从index位置获取table,从栈顶弹出value，table[key]=value
        /// </summary>
        public void SetTableValue(int index, long integerKey)
        {
            LuaDataUnion data = globalStack.Get(index);
            LuaDataUnion key = Factory.NewInteger(integerKey);
            LuaDataUnion value = globalStack.Pop();
            SetTableValue(index, data, key, value);
        }

        /// <summary>
        /// data.table[key] = value
        /// </summary>
        private void SetTableValue(int index, LuaDataUnion data, LuaDataUnion key,LuaDataUnion value)
        {
            if (data.Type != LuaDataType.Table)
            {
                throw new Exception("SetTableValue的data不是table");
            }
           
            data.Table[key] = value;
        }
    }
}

