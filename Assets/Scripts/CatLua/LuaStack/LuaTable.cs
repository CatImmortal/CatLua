using System.Collections;
using System.Collections.Generic;
using System;

namespace CatLua
{
    /// <summary>
    /// Lua中的Table数据结构
    /// </summary>
    public struct LuaTable
    {
        public LuaTable(int arrSize,int dictSize)
        {
            arr = default;
            dict = default;

            if (arrSize > 0)
            {
                
                for (int i = 0; i < arrSize; i++)
                {
                    arr.Add(default);
                }
            }

            if (dictSize > 0)
            {
                dict = new Dictionary<LuaDataUnion, LuaDataUnion>(dictSize);
            }
           
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
                LuaDataUnion key = new LuaDataUnion(LuaDataType.Integer, integer: index);
                return this[key];
            }
            set {
                LuaDataUnion key = new LuaDataUnion(LuaDataType.Integer, integer: index);
                this[key] = value;
            }
        }

        public LuaDataUnion this[double index]
        {
            get
            {
                LuaDataUnion key = new LuaDataUnion(LuaDataType.Number,number:index);
                return this[key];
            }
            set
            {
                LuaDataUnion key = new LuaDataUnion(LuaDataType.Number, number: index);
                this[key] = value;
            }
        }

        public LuaDataUnion this[string index]
        {
            get
            {
                LuaDataUnion key = new LuaDataUnion(LuaDataType.String,str:index);
                return this[key];
            }
            set
            {
                LuaDataUnion key = new LuaDataUnion(LuaDataType.String, str: index);
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

                //否则从字典取
                return dict[key];
            }

            set {
                if (key.Type == LuaDataType.Nil)
                {
                    throw new Exception("table的index不能为nil");
                }

                if (key.Type == LuaDataType.Number && double.IsNaN(key.Number))
                {
                    throw new Exception("table的index不能为nan");
                }

                if (TryConvertToArrIndex(key, out long index) && index >= 1)
                {
                    //key是整数或者是可以转换为整数索引的浮点数 

                    if (arr == null)
                    {
                        arr = new List<LuaDataUnion>();
                    }

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

                    if (dict == null)
                    {
                        dict = new Dictionary<LuaDataUnion, LuaDataUnion>(8);
                    }

                    dict[key] = value;
                }
                else
                {
                    //value是个nil 删掉key
                    dict.Remove(key);
                }
            }
        }

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
                LuaDataUnion key = new LuaDataUnion(LuaDataType.Integer, integer: index);
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
    
        
    }
}

