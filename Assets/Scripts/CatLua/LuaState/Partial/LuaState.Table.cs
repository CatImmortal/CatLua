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
            LuaDataUnion key = Pop();
            return PushTableValue(data, key);
        }

        /// <summary>
        /// 压入data.table[key]
        /// </summary>
        private LuaDataType PushTableValue(LuaDataUnion data, LuaDataUnion key,bool raw = false)
        {
            if (data.Type == LuaDataType.Table)
            {
                //data是table

                LuaDataUnion value = data.Table[key];

                if (value.Type != LuaDataType.Nil)
                {
                    //value不为nil值 直接push   
                    globalStack.Push(value);
                    return value.Type;
                }

            }

            //data不是table 或者 data.table[key] == nil
            //尝试调用__index元方法

            //raw调用 或者 没有关联的__index元方法  push一个nil
            if (raw || !HasMetaMethod(data, "__index"))
            {
                globalStack.Push(default);
                return default;
            }


            //否则根据__index关联的类型进行处理
            LuaTable mt = GetMetaTable(data);
            LuaDataUnion mtValue = mt["__index"];
            switch (mtValue.Type)
            {
                
                case LuaDataType.Table:
                    //__index关联的是table，去这个table里找value
                    return PushTableValue(mtValue, key, false);

                case LuaDataType.Function:
                    //__index关联的是函数 以table和key为参数调用这个函数 把1个返回值压入栈顶
                    Push(mtValue.Closure);
                    Push(data);
                    Push(key);
                    CallFunc(2, 1);
                    LuaDataUnion result = globalStack.Get(-1);
                    return result.Type;
            }

            return default;
        }

        /// <summary>
        /// 从index位置获取table，从栈顶分别弹出value key ，table[key]=value
        /// </summary>
        public void SetTableValue(int index)
        {
            LuaDataUnion data = globalStack.Get(index);
            LuaDataUnion value = Pop();
            LuaDataUnion key = Pop();
            SetTableValue(data, key, value);
        }

        /// <summary>
        /// 从index位置获取table,从栈顶弹出value，table[key]=value
        /// </summary>
        public void SetTableValue(int index, string strKey)
        {
            LuaDataUnion data = globalStack.Get(index);
            LuaDataUnion key = Factory.NewString(strKey);
            LuaDataUnion value = Pop();
            SetTableValue(data, key, value);
        }

        /// <summary>
        /// 从index位置获取table,从栈顶弹出value，table[key]=value
        /// </summary>
        public void SetTableValue(int index, long integerKey)
        {
            LuaDataUnion data = globalStack.Get(index);
            LuaDataUnion key = Factory.NewInteger(integerKey);
            LuaDataUnion value = Pop();
            SetTableValue(data, key, value);
        }

        /// <summary>
        /// data.table[key] = value
        /// </summary>
        private void SetTableValue(LuaDataUnion data, LuaDataUnion key,LuaDataUnion value,bool raw = false)
        {


            if (data.Type == LuaDataType.Table)
            {
                //data是table

                if (data.Table[key].Type != LuaDataType.Nil)
                {
                    //key在表中存在
                    data.Table[key] = value;
                    return;
                }
                
            }

            //data不是table 或者 data.table[key] == nil
            //尝试调用__newindex元方法

            //raw调用 或者 没有关联的__newindex元方法
            if (raw || !HasMetaMethod(data, "__newindex"))
            {
                data.Table[key] = value;
                return;
            }


            //否则根据__index关联的类型进行处理
            LuaTable mt = GetMetaTable(data);
            LuaDataUnion mtValue = mt["__index"];
            switch (mtValue.Type)
            {

                case LuaDataType.Table:
                    //__newindex关联的是table，就set到这个table里
                    SetTableValue(mtValue, key, value, false);
                    break;
                case LuaDataType.Function:
                    //__index关联的是函数 以table和key value为参数调用这个函数 没有返回值
                    Push(mtValue.Closure);
                    Push(data);
                    Push(key);
                    Push(value);
                    CallFunc(3, 0);
                    break;
            }

        }


    }
}

