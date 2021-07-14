using System;
using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    public partial class LuaState
    {
        private Dictionary<string, Func<LuaState, int, int>> stdLibs = new Dictionary<string, Func<LuaState, int, int>>()
        {
            { "_G",BasicLib.OpenBaseLib},
        };

        /// <summary>
        /// 开启所有标准库
        /// </summary>
        public void OpenLibs()
        {
            foreach (KeyValuePair<string, Func<LuaState, int, int>> item in stdLibs)
            {
                RequireF(item.Key, item.Value, true);
            }
        }

        /// <summary>
        /// 开启单个标准库
        /// </summary>
        public void RequireF(string modName, Func<LuaState, int, int> csFunc,bool isGlobal)
        {
            LuaTable globalTable = registry[Constants.GlobalEnvKey].Table;
            if (globalTable["_LOADED"].Type == LuaDataType.Nil)
            {
                globalTable["_LOADED"] = Factory.NewTable(new LuaTable());
            }

            LuaTable loaded = globalTable["_LOADED"].Table;

            if (loaded[modName].Type == LuaDataType.Nil)
            {
                //包未加载

                //调用加载函数
                PushCSFunc(csFunc);
                Push(modName);
                CallFunc(1, 1);

                //_LOADED[modName] = module
                loaded[modName] = Pop();

            }

            if (isGlobal)
            {
                //_G[modName] = module
                globalTable[modName] = loaded[modName];
            }

        }

        /// <summary>
        /// 检查index处的表的某个字段是否为表，如果是表，就将其压入栈顶，否则创建一个空表赋值给该字段
        /// </summary>
        public bool PushSubTable(int index,string fName)
        {
            if (PushTableValue(index,fName) == LuaDataType.Table)
            {
                return true;
            }

            //不是table 就放一个空的进去
            Pop();

            index = globalStack.GetAbsIndex(index);

            LuaDataUnion data = Factory.NewTable(new LuaTable());
            Push(data);

            SetTableValue(index, fName);
            return false;
        }
    }
}

