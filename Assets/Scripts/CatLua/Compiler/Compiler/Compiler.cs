using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    public static partial class Compiler
    {
        /// <summary>
        /// 将Lua源代码编译为函数原型
        /// </summary>
        public static FuncPrototype Compile(string sourceCode,string chunkName)
        {
            Block ast = Parser.Parse(sourceCode, chunkName);
            FuncPrototype proto = GenProto(ast);
            return proto;
        }

        /// <summary>
        /// 通过Block生成FuncProto
        /// </summary>
        private static FuncPrototype GenProto(Block block)
        {
            //将整个block视为一个函数体
            FuncDefExp fdExp = new FuncDefExp(0, 0, null, true, block);

            GenFuncInfo fi = new GenFuncInfo(null, null);

            //将ENV作为局部变量放入
            fi.AddLocalVar("_ENV");

            CompileFuncDefExp(fi, fdExp, 0);

            FuncPrototype proto = ToProto(fi.Children[0]);

            return proto;
        }


        /// <summary>
        /// 函数信息转换为函数原型
        /// </summary>
        private static FuncPrototype ToProto(GenFuncInfo fi)
        {
            FuncPrototype proto = new FuncPrototype();
            proto.NumParams = (byte)fi.NumParams;
            proto.MaxStackSize = (byte)fi.MaxRegs;
            proto.Code = fi.Instructions.ToArray();
            proto.Constants = GetConstants(fi);
            proto.UpvalueInfos = GetUpvalueInfos(fi);
            proto.Protos = ToProtos(fi.Children.ToArray());
            proto.LineInfo = new uint[128];
            proto.Locvars = new LocalVarInfo[128];
            proto.UpvalueNames = new string[128];

            if (fi.IsVararg)
            {
                proto.IsVararg = 1;
            }

            return proto;
        }

        private static LuaConstantUnion[] GetConstants(GenFuncInfo fi)
        {
            LuaConstantUnion[] constants = new LuaConstantUnion[fi.ConstantDict.Count];
            foreach (KeyValuePair<LuaConstantUnion, int> item in fi.ConstantDict)
            {
                constants[item.Value] = item.Key;
            }
            return constants;
        }


        private static FuncPrototype[] ToProtos(GenFuncInfo[] fis)
        {
            FuncPrototype[] protos = new FuncPrototype[fis.Length];
            for (int i = 0; i < fis.Length; i++)
            {
                protos[i] = ToProto(fis[i]);
            }
            return protos;
        }

        private static UpvalueInfo[] GetUpvalueInfos(GenFuncInfo fi)
        {
            UpvalueInfo[] upvalueInfos = new UpvalueInfo[fi.UpvalueDict.Count];
            foreach (KeyValuePair<string, GenUpvalueInfo> item in fi.UpvalueDict)
            {
                UpvalueInfo upvalue = new UpvalueInfo();
                if (item.Value.LocalVarSlot >= 0)
                {
                    //in stack
                    upvalue.Index = (byte)item.Value.LocalVarSlot;
                    upvalue.Instack = 1;
                    upvalueInfos[item.Value.Index] = upvalue;
                }
                else
                {
                    //in parent upvalues
                    upvalue.Index = (byte)item.Value.UpvalueIndex;
                    upvalue.Instack = 0;
                    upvalueInfos[item.Value.Index] = upvalue;
                }
            }

            return upvalueInfos;
        }

        /// <summary>
        /// 编译Block
        /// </summary>
        private static void CompileBlock(GenFuncInfo fi, Block block)
        {
            for (int i = 0; i < block.Stats.Length; i++)
            {
                //编译语句
                CompileStat(fi, block.Stats[i]);
            }

            if (block.ReturnExps != null)
            {
                //编译返回值表达式
                CompileReturnExp(fi, block.ReturnExps);
            }
        }

    }
}

