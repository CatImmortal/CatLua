using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatLua
{
    /// <summary>
    /// 给Lua调用的C#函数实现
    /// </summary>
    public static class CSFuncs
    {
        public static void Init(LuaState vm)
        {
            vm.RegisteCSFunc("print", Print);
            vm.RegisteCSFunc("getmetatable", GetMetaTable);
            vm.RegisteCSFunc("setmetatable", SetMetaTable);
            vm.RegisteCSFunc("next", Next);
            vm.RegisteCSFunc("pairs", Pairs);
            vm.RegisteCSFunc("ipairs", IPairs);
        }

        private static int Print(LuaState vm,int argsNum)
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

        private static int GetMetaTable(LuaState vm, int argsNum)
        {
            if (!vm.PushMetaTable(vm.CurFrameBottom))
            {
                vm.Push();
            }
            return 1;
        }

        private static int SetMetaTable(LuaState vm, int argsNum)
        {
            vm.SetMetaTable(vm.CurFrameBottom);
            return 1;
        }

        private static int Next(LuaState vm, int argsNum)
        {
            //next第二个参数key是可选的 如果没传得保证有个nil值可弹出
            vm.SetTop(vm.CurFrameBottom + 1);

            if (vm.Next(vm.CurFrameBottom))
            {
                //返回next key 和对应的vlaue
                return 2;
            }
            else
            {
                //返回1个nil
                vm.Push();
                return 1;
            }
        }

        private static int Pairs(LuaState vm, int argsNum)
        {
            //压入泛型for需要的3个数据
            //为后续的TForCall和TForLoop准备

            vm.PushCSFunc(Next);  //f 迭代器函数
            vm.CopyAndPush(vm.CurFrameBottom);  //s 要遍历的表
            vm.Push();  //var 一个nil值，对应next需要的key的初始值

            return 3;
        }

        private static int IPairsAux(LuaState vm, int argsNum)
        {
            //local nextIndex = i + 1
            long index = vm.GetInteger(vm.CurFrameBottom + 1);
            long nextIndex =  index + 1;
            vm.Push(nextIndex);

            //local nextVal = t[nextIndex]
            //if nextVal == nil then return nil
            if (vm.PushTableValue(vm.CurFrameBottom,nextIndex) == LuaDataType.Nil)
            {
                return 1;
            }

            //else return nextIndex,nextVal
            //end
            return 2;
        }

        private static int IPairs(LuaState vm, int argsNum)
        {
            //压入泛型for需要的3个数据
            //为后续的TForCall和TForLoop准备

            vm.PushCSFunc(IPairsAux);  //f 迭代器函数
            vm.CopyAndPush(vm.CurFrameBottom);  //s 要遍历的表
            vm.Push(0);  //var 一个0值，对应next需要的key的初始值

            return 3;
        }


    }

}
