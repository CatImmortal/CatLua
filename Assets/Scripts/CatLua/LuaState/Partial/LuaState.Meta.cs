using System.Collections;
using System.Collections.Generic;


namespace CatLua
{
    public partial class LuaState
    {
        /// <summary>
        /// 设置元表
        /// </summary>
        public void SetMetaTable(LuaDataUnion data, LuaTable mt)
        {
            if (data.Type == LuaDataType.Table)
            {
                //每个table实例有自己的专属元表
                data.Table.MetaTable = mt;
                return;
            }

            //其他类型 一种类型一个元表
            string key = "__mt" + data.Type;
            registry[key] = Factory.NewTable(mt);
        }

        /// <summary>
        /// 获取元表
        /// </summary>
        public LuaTable GetMetaTable(LuaDataUnion data)
        {
            if (data.Type == LuaDataType.Table)
            {
                return data.Table.MetaTable;
            }

            string key = "__mt" + data.Type;
            LuaDataUnion mt = registry[key];
            if (mt.Type != LuaDataType.Nil)
            {
                return mt.Table;
            }

            return null;
        }

        /// <summary>
        /// 尝试调用元方法
        /// </summary>
        public bool TryCallMetaMethod(LuaDataUnion a, LuaDataUnion b,string metaMethodName,out LuaDataUnion result)
        {
            result = default;
            LuaTable mt = GetMetaTable(a);

            if (mt == null || mt[metaMethodName].Type == LuaDataType.Nil)
            {
                mt = GetMetaTable(b);
                if (mt == null || mt[metaMethodName].Type == LuaDataType.Nil)
                {
                    //两个lua值都没有元表或指定的元方法
                    return false;
                }
            }

            //调用指定元方法
            Push(mt[metaMethodName].Closure);
            Push(a);
            Push(b);
            CallFunc(2, 1);

            result = Pop();

            return true;
        }

    }

}
