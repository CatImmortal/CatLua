using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace CatLua
{
    public static class BasicLib
    {
        private static Dictionary<string, Func<LuaState, int, int>> baseFuncs = new Dictionary<string, Func<LuaState, int, int>>()
        {
            { "print",BasePrint},
        };

        public static int OpenBaseLib(LuaState ls,int argsNum)
        {

        }

        private static int BasePrint(LuaState ls,int argsNum)
        {
            string s = string.Empty;
            s += "Lua Print:";
            LuaDataUnion[] datas = ls.PopN(argsNum);
            for (int i = 0; i < argsNum; i++)
            {
                s += datas[i].ToString();
                s += '\t';
            }

            Debug.Log(s);

            return 0;
        }
    }

}
