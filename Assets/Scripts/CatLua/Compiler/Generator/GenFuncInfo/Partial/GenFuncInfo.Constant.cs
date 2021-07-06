using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    public partial class GenFuncInfo
    {
        /// <summary>
        /// 常量值与对应常量表索引的映射
        /// </summary>
        public Dictionary<LuaConstantUnion, int> ConstantDict = new Dictionary<LuaConstantUnion, int>();


        /// <summary>
        /// 获取常量在常量表的索引
        /// </summary>
        public int IndexOfConstant(LuaConstantUnion constant)
        {
            if (ConstantDict.TryGetValue(constant, out int index))
            {
                return index;
            }


            //常量不在表里 放入表里 返回索引
            index = ConstantDict.Count;
            ConstantDict.Add(constant, index);
            return index;
        }
    }

}
