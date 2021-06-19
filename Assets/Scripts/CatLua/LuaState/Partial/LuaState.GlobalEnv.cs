﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    public partial class LuaState
    {
        /// <summary>
        /// 将_G压入栈顶
        /// </summary>
        public void PushGlobalEnv()
        {
            LuaTable t = registry[Constants.GlobalEnvIndex].Table;
            Push(t);
        }

        /// <summary>
        /// 将_G[key]压入栈顶
        /// </summary>
        public LuaDataType PushGlobalValue(string key)
        {
            LuaDataUnion data = registry[Constants.GlobalEnvIndex];
            return PushTableValue(data, new LuaDataUnion(LuaDataType.String,str:key));
        }

        /// <summary>
        /// 从栈顶弹出value，_G[key] = value
        /// </summary>
        public void SetGlobalValue(string key)
        {
            LuaDataUnion dataKey = new LuaDataUnion(LuaDataType.String, str: key);
            LuaDataUnion value = globalStack.Pop();


            LuaDataUnion data = registry[Constants.GlobalEnvIndex];
            data.Table[dataKey] = value;
            registry[Constants.GlobalEnvIndex] = data;
        }

        /// <summary>
        /// 注册C#函数到_G
        /// </summary>
        public void RegisteCSFunc(string key, Func<LuaState, int> csFunc)
        {
            PushCSFunc(csFunc);
            SetGlobalValue(key);
        }
    }

}