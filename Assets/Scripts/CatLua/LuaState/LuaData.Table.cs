﻿using System.Collections;
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
            LuaDataUnion data = CurStack.Get(index);
            LuaDataUnion key = new LuaDataUnion(LuaDataType.String, str: strKey);
            return PushTableValue(data, key);
        }

        /// <summary>
        /// 从index位置获取table，压入table[key]
        /// </summary>
        public LuaDataType PushTableValue(int index, long integerKey)
        {
            LuaDataUnion data = CurStack.Get(index);
            LuaDataUnion dataKey = new LuaDataUnion(LuaDataType.Integer, integer:integerKey);
            return PushTableValue(data, dataKey);
        }

        /// <summary>
        /// 从index位置获取table，从栈顶弹出key，压入table[key]
        /// </summary>
        public LuaDataType PushTableValue(int index)
        {
            LuaDataUnion data = CurStack.Get(index);
            LuaDataUnion key = CurStack.Pop();
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
            CurStack.Push(value);
            return value.Type;
        }

        /// <summary>
        /// 从index位置获取table，从栈顶弹出key value，table[key]=value
        /// </summary>
        public void SetTableValue(int index)
        {
            LuaDataUnion data = CurStack.Get(index);
            LuaDataUnion value = CurStack.Pop();
            LuaDataUnion key = CurStack.Pop();
            SetTableValue(index,data, key, value);
        }

        /// <summary>
        /// 从index位置获取table,从栈顶弹出value，table[key]=value
        /// </summary>
        public void SetTableValue(int index, string strKey)
        {
            LuaDataUnion data = CurStack.Get(index);
            LuaDataUnion key = new LuaDataUnion(LuaDataType.String,str:strKey);
            LuaDataUnion value = CurStack.Pop();
            SetTableValue(index,data, key, value);
        }

        /// <summary>
        /// 从index位置获取table,从栈顶弹出value，table[key]=value
        /// </summary>
        public void SetTableValue(int index, long integerKey)
        {
            LuaDataUnion data = CurStack.Get(index);
            LuaDataUnion key = new LuaDataUnion(LuaDataType.Integer, integer: integerKey);
            LuaDataUnion value = CurStack.Pop();
            SetTableValue(index, data, key, value);
        }

        /// <summary>
        /// table[key] = value
        /// </summary>
        private void SetTableValue(int index, LuaDataUnion data, LuaDataUnion key,LuaDataUnion value)
        {
            if (data.Type != LuaDataType.Table)
            {
                throw new Exception("SetTableValue的data不是table");
            }
           
            data.Table[key] = value;
            CurStack.Set(index, data);
        }
    }
}
