using System.Collections;
using System.Collections.Generic;
using System;

namespace CatLua
{
    public class PackageLib
    {
        private static Dictionary<string, Func<LuaState, int, int>> llFuncs = new Dictionary<string, Func<LuaState, int, int>>()
        {
            { "require",Require}
        };

        private static Dictionary<string, Func<LuaState, int, int>> packageFuncs = new Dictionary<string, Func<LuaState, int, int>>()
        {
            { "searchpath",SearchPath}
        };


        public static int OpenPackageLib(LuaState ls, int argsNum)
        {
            //push package
            ls.NewLib(packageFuncs);

            //设置package.searchers
            CreateSearchersTable(ls);

            //设置package.path
            ls.Push("./?.lua;./?/init.lua");
            ls.SetTableValue(-2, "path");

            //push _ENV
            ls.PushGlobalEnv();

            //设置package.loaded
            ls.PushSubTable(-1, Constants.LuaLoadedTable);
            ls.SetTableValue(-3, "loaded");

            //设置package.preload
            ls.PushSubTable(-1, Constants.LuaPreloadTable);
            ls.SetTableValue(-3, "preload");

            //pop _ENV
            ls.Pop();

            //设置package为PackageRequire函数的upvalue
            ls.CopyAndPush(-1);
            ls.SetCSFuncs(llFuncs, 1);



            return 1;
        }

        public static int Require(LuaState ls,int argsNum)
        {
            throw new NotImplementedException();
        }

        public static int SearchPath(LuaState ls, int argsNum)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 初始化package.searchers表
        /// </summary>
        private static void CreateSearchersTable(LuaState ls)
        {
            Func<LuaState, int, int>[] searchres = { PreloadSearcher, LuaSearcher };

            LuaTable t = new LuaTable(searchres.Length);
            ls.Push(t);

            for (int i = 0; i < searchres.Length; i++)
            {
                t[i + 1] = Factory.NewFunc(new Closure(searchres[i]));
            }

           
            ls.SetTableValue(-2, "searchers");
        }

        private static int PreloadSearcher(LuaState ls,int argsNum)
        {
            throw new NotImplementedException();
        }

        private static int LuaSearcher(LuaState ls, int argsNum)
        {
            throw new NotImplementedException();
        }
    }

}
