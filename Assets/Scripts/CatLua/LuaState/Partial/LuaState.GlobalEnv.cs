using System;
using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    public partial class LuaState
    {
        /// <summary>
        /// 将全局环境表压入栈顶
        /// </summary>
        public void PushGlobalEnv()
        {
            LuaTable t = registry[Constants.GlobalEnvKey].Table;
            Push(t);
        }

        /// <summary>
        /// 将全局环境表[key]压入栈顶
        /// </summary>
        public LuaDataType PushGlobalValue(string key)
        {
            LuaDataUnion data = registry[Constants.GlobalEnvKey];
            return PushTableValue(data, Factory.NewString(key));
        }

        /// <summary>
        /// 从栈顶弹出value，全局环境表[key] = value
        /// </summary>
        public void SetGlobalValue(string key)
        {
            LuaDataUnion dataKey = Factory.NewString(key);
            LuaDataUnion value = Pop();


            LuaDataUnion data = registry[Constants.GlobalEnvKey];
            data.Table[dataKey] = value;
        }

        /// <summary>
        /// 注册C#函数到全局环境表
        /// </summary>
        public void RegisteCSFunc(string key, Func<LuaState, int, int> csFunc)
        {
            PushCSFunc(csFunc);
            SetGlobalValue(key);
        }
    }

}
