using System.Collections;
using System.Collections.Generic;


namespace CatLua
{
    public partial class LuaState
    {
        /// <summary>
        /// 将index位置的值的元表压入栈顶
        /// </summary>
        public bool PushMetaTable(int index)
        {
            LuaDataUnion data = globalStack.Get(index);
            LuaTable mt = GetMetaTable(data);
            if (mt == null)
            {
                return false;
            }

            Push(mt);
            return true;
        }


        /// <summary>
        /// 从栈顶弹出一个table，然后将其设置为index位置的值的元表
        /// </summary>
        public void SetMetaTable(int index)
        {
            LuaDataUnion data = globalStack.Get(index);
            LuaDataUnion mtData = Pop();

            if (mtData.Type == LuaDataType.Nil || mtData.Type == LuaDataType.Table)
            {
                //nil的话就是清除元表 table就算正常设置元表
                SetMetaTable(data, mtData.Table);
            }
            else
            {
                throw new System.Exception("SetMetaTable失败");
            }
            
        }

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
        /// 尝试调用有1到2个参数和1个返回值的元方法
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



        /// <summary>
        /// 是否有指定的元方法
        /// </summary>
        public bool HasMetaMethod(LuaDataUnion data, string metaMethodName)
        {
            LuaTable mt = GetMetaTable(data);
            if (mt == null || mt[metaMethodName].Type == LuaDataType.Nil)
            {
                //没有元表或指定的元方法
                return false;
            }
            return true;
        }

    }

}
