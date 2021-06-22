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
        public static void Init(LuaState vm)
        {
            vm.RegisteCSFunc("print", Print);
            vm.RegisteCSFunc("getmetatable", GetMetaTable);
            vm.RegisteCSFunc("setmetatable", SetMetaTable);
        }

        public static int Print(LuaState vm,int argsNum)
        {
            string s = string.Empty;
            s += "Lua Print:";
            LuaDataUnion[] datas = vm.PopN(argsNum);
            for (int i = 0; i < argsNum; i++)
            {
                s += datas[i].ToString();
                s += '\t';
            }

            Debug.Log(s);

            return 0;
        }

        public static int GetMetaTable(LuaState vm, int argsNum)
        {
            if (!vm.PushMetaTable(vm.CurFrameBottom))
            {
                vm.Push();
            }
            return 1;
        }

        public static int SetMetaTable(LuaState vm, int argsNum)
        {
            vm.SetMetaTable(vm.CurFrameBottom);
            return 1;
        }
    }

}
