﻿using System.Collections;
using System.Collections.Generic;
using System;

namespace CatLua
{
    /// <summary>
    /// Lua中的Table数据结构
    /// </summary>
    public class LuaTable
    {
        public LuaTable(int arrSize = 0,int dictSize = 0)
        {

            arr = new List<LuaDataUnion>(arrSize);
            for (int i = 0; i < arrSize; i++)
            {
                arr.Add(default);
            }

            dict = new Dictionary<LuaDataUnion, LuaDataUnion>(dictSize);
        }

        /// <summary>
        /// 数组部分
        /// </summary>
        private List<LuaDataUnion> arr;

        /// <summary>
        /// 字典部分
        /// </summary>
        private Dictionary<LuaDataUnion, LuaDataUnion> dict;

        /// <summary>
        /// 元表
        /// </summary>
        public LuaTable MetaTable;


        /// <summary>
        /// 迭代器遍历用的key和next key映射
        /// </summary>
        private Dictionary<LuaDataUnion,LuaDataUnion> keyAndNextKey;

        /// <summary>
        /// 数组部分的长度
        /// </summary>
        public int Length
        {
            get
            {
                return arr.Count;
            }
        }

        public LuaDataUnion this[long index]
        {
            get {
                LuaDataUnion key = Factory.NewInteger(index);
                return this[key];
            }
            set {
                LuaDataUnion key = Factory.NewInteger(index);
                this[key] = value;
            }
        }

        public LuaDataUnion this[double index]
        {
            get
            {
                LuaDataUnion key = Factory.NewNumber(index);
                return this[key];
            }
            set
            {
                LuaDataUnion key = Factory.NewNumber(index);
                this[key] = value;
            }
        }

        public LuaDataUnion this[string index]
        {
            get
            {
                LuaDataUnion key = Factory.NewString(index);
                return this[key];
            }
            set
            {
                LuaDataUnion key = Factory.NewString(index);
                this[key] = value;
            }
        }

        public LuaDataUnion this[LuaDataUnion key]
        {
            get {
                if (TryConvertToArrIndex(key, out long index))
                {
                    //key是整数或者是可以转换为整数索引的浮点数 
                    if (index >= 1 && index <= arr.Count)
                    {
                        //在数组长度内 从数组取
                        return arr[(int)index - 1];
                    }
                }

                if (!dict.TryGetValue(key,out LuaDataUnion value))
                {
                    return default;
                }

                //否则从字典取
                return value;
            }

            set {

                if (key.Type == LuaDataType.Nil)
                {
                    throw new Exception("table的key不能为nil");
                }

                if (key.Type == LuaDataType.Number && double.IsNaN(key.Number))
                {
                    throw new Exception("table的key不能为NaN");
                }

                if (TryConvertToArrIndex(key, out long index) && index >= 1)
                {
                    //key是整数或者是可以转换为整数索引的浮点数 

                    if (index <= arr.Count)
                    {
                        //在数组长度内 放入数组
                        arr[(int)index - 1] = value;

                        if (index == arr.Count && value.Type == LuaDataType.Nil)
                        {
                            //value是个nil值 并且被放在数组的末尾 需要清理掉末尾的nil值
                            RemoveArrTailNil();
                        }
                        return;
                    }

                    if (index == arr.Count + 1 && value.Type != LuaDataType.Nil)
                    {
                        //不在数组长度内 但只是刚刚超出1位 并且不是nil值

                        //可能之前存在字典里 先删掉
                        if (dict != null)
                        {
                            dict.Remove(key);
                        }
                        

                        //放入数组 触发扩容
                        arr.Add(value);

                        //将字典里的符合条件的整数key的value移动到扩容后的数组
                        MoveDictToArr();

                        return;
                    }

                    
                }

                //不能放进数组里 只能试试字典了

                if (value.Type != LuaDataType.Nil)
                {
                    //value不是nil值 放入字典里

                    dict[key] = value;
                }
                else
                {
                    //value是个nil 删掉key
                    dict.Remove(key);
                }
            }
        }

        //public override string ToString()
        //{
        //    string s = string.Empty;

        //    s += "table:{";

        //    if (arr != null)
        //    {
        //        for (int i = 0; i < arr.Count; i++)
        //        {
        //            s += $"{i + 1} = {arr[i]},";
        //        }
        //    }


        //    if (dict != null)
        //    {
        //        foreach (KeyValuePair<LuaDataUnion, LuaDataUnion> item in dict)
        //        {
        //            s += $"{item.Key} = {item.Value},";
        //        }
        //    }
            

        //    s += "}";

        //    return s;
        //}

        /// <summary>
        /// 尝试将lua值转换为数组部分的整数索引
        /// </summary>
        private bool TryConvertToArrIndex(LuaDataUnion key,out long l)
        {
            l = 0;

            if (key.Type == LuaDataType.Integer)
            {
                l = key.Integer;
                return true;
            }

            if (key.Type == LuaDataType.Number && key.TryConvertToInteger(out l))
            {
                return true;
            }

            return false;
        }
    
        /// <summary>
        /// 检查数组部分的尾部nil值并删掉
        /// </summary>
        private void RemoveArrTailNil()
        {
            
            for (int i = arr.Count - 1; i >= 0; i++)
            {
                if (arr[i].Type == LuaDataType.Nil)
                {
                    arr.RemoveAt(i);
                }

            }
        }
   
        /// <summary>
        /// 数组部分扩容后，将字典部分的某些值移动到数组里
        /// </summary>
        private void MoveDictToArr()
        {
            if (dict == null)
            {
                return;
            }

            //将dict中从当前数组长度+1的连续整数key的value移动到数组部分
            //比如数组长度为3 就将字典里key分别为4,5,6,7....的value移动到数组

            int index = arr.Count + 1;
            while (true)
            {
                LuaDataUnion key = Factory.NewInteger(index);
                if (dict.TryGetValue(key, out LuaDataUnion data))
                {
                    arr.Add(data);
                    dict.Remove(key);
                    index++;
                }
                else
                {
                    break;
                }
            }
        }
    
        /// <summary>
        /// 获取key对应的next key
        /// </summary>
        public LuaDataUnion NextKey(LuaDataUnion key)
        {
            if (key.Type == LuaDataType.Nil)
            {
                if (keyAndNextKey == null)
                {
                    keyAndNextKey = new Dictionary<LuaDataUnion, LuaDataUnion>(arr.Count + dict.Count);
                }
                else
                {
                    keyAndNextKey.Clear();
                }

                //初始化key列表

                LuaDataUnion curKey = default;

                for (int i = 0; i < arr.Count; i++)
                {
                    if (arr[i].Type != LuaDataType.Nil)
                    {
                        LuaDataUnion nextKey = Factory.NewInteger(i + 1);
                        keyAndNextKey[curKey] = nextKey;

                        curKey = nextKey;
                    }
                }

                foreach (KeyValuePair<LuaDataUnion, LuaDataUnion> item in dict)
                {
                    LuaDataUnion nextKey = item.Key;
                    keyAndNextKey[curKey] = nextKey;

                    curKey = nextKey;
                }

                //最后一个Key对应的next key是nil值
                keyAndNextKey[curKey] = default;
            }

            return keyAndNextKey[key];
        }
    }
}

