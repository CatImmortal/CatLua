using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace CatLua
{
    /// <summary>
    /// 基础库
    /// </summary>
    public static class BasicLib
    {
        /// <summary>
        /// 基础库函数
        /// </summary>
        private static Dictionary<string, Func<LuaState, int, int>> baseFuncs = new Dictionary<string, Func<LuaState, int, int>>()
        {
            { "print",Print},
            { "getmetatable", GetMetaTable },
            {"setmetatable", SetMetaTable},
            {"next", Next},
            {"pairs", Pairs },
            {"ipairs", IPairs },
            {"error", Error },
            {"pcall", PCall },
        }; 

        /// <summary>
        /// 打开基础库
        /// </summary>
        public static int OpenBaseLib(LuaState ls,int argsNum)
        {
            //将基础库函数都放入全局环境表里
            ls.PushGlobalEnv();
            ls.SetCSFuncs(baseFuncs);

            //_ENV[_G] = _ENV 保证用_G也能访问所有到全局变量
            //ls.CopyAndPush(0);
            //ls.SetTableValue(0, "_G");

            //版本号
            ls.Push("Lua 5.3");
            ls.SetTableValue(0, "_VERSION");
            return 1;
        }

        private static int Print(LuaState vm, int argsNum)
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
            long nextIndex = index + 1;
            vm.Push(nextIndex);

            //local nextVal = t[nextIndex]
            //if nextVal == nil then return nil
            if (vm.PushTableValue(vm.CurFrameBottom, nextIndex) == LuaDataType.Nil)
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

        private static int Error(LuaState vm, int argsNum)
        {
            return vm.Error();
        }

        private static int PCall(LuaState vm, int argsNum)
        {
            FuncCallState state = vm.PCall(argsNum - 1, -1, 0);  //有一个参数是要pcall的函数 所以要-1
            vm.Push(state == FuncCallState.Ok);  //将是否调用成功作为返回值压入栈顶
            vm.PopAndInsert(vm.CurFrameBottom);  //移动到栈帧底部
            return vm.CallFrameReturnResultNum + 1;  //在栈帧底部放了个pcall的返回值  所以要+1
        }
    }

}
