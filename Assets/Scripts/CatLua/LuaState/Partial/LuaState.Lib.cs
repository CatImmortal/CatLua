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
            { "math",MathLib.OpenMathLib},
            //{ "package",PackageLib.OpenPackageLib},
        };

        /// <summary>
        /// 开启所有标准库
        /// </summary>
        public void OpenStdLibs()
        {
            foreach (KeyValuePair<string, Func<LuaState, int, int>> item in stdLibs)
            {
                OpenStdLib(item.Key, item.Value);
            }
        }

        /// <summary>
        /// 开启单个标准库
        /// </summary>
        public void OpenStdLib(string modName, Func<LuaState, int, int> csFunc)
        {
            LuaTable globalTable = registry[Constants.GlobalEnvKey].Table;

            if (globalTable["_LOADED"].Type == LuaDataType.Nil)
            {
                globalTable["_LOADED"] = Factory.NewTable(new LuaTable());
            }

            LuaTable loaded = globalTable["_LOADED"].Table;


            //检查是否加载过
            if (loaded[modName].Type == LuaDataType.Nil)
            {
                //包未加载

                //调用加载函数
                PushCSFunc(csFunc);
                CallFunc(0, 1);

                //分别放入loaded表和全局环境表里
                loaded[modName] = Pop();
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

        /// <summary>
        /// 将csfuncs都放入栈顶的table里
        /// </summary>
        public void SetCSFuncs(Dictionary<string, Func<LuaState, int, int>> csFuncs, int upvalueNum = 0)
        {
            LuaDataUnion[] upvalues = PopN(upvalueNum);

            foreach (KeyValuePair<string, Func<LuaState, int, int>> item in csFuncs)
            {
                for (int i = 0; i < upvalues.Length; i++)
                {
                    Push(upvalues[i]);
                }
                PushCSFunc(item.Value,upvalueNum);
                SetTableValue(0, item.Key);
            }
        }

        /// <summary>
        /// 用csFuncs里的函数创建一个table，并压入栈顶
        /// </summary>
        public void NewLib(Dictionary<string, Func<LuaState, int, int>> csFuncs)
        {
            LuaTable t = new LuaTable();
            foreach (KeyValuePair<string, Func<LuaState, int, int>> item in csFuncs)
            {
                t[item.Key] = Factory.NewFunc(new Closure(item.Value));
            }

            Push(t);
        }

      
    }
}

