using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatLua
{
    /// <summary>
    /// 给Lua调用的C#函数实现
    /// </summary>
    public static class CSFunc
    {
        public static void Init(LuaState ls)
        {
            ls.RegisteCSFunc("print", Print);
        }

        public static int Print(LuaState ls)
        {
            int argsNum = ls.CurFrameNonReserveRegisterSize;
            string s = string.Empty;
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
