using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    public static class Compiler
    {
        /// <summary>
        /// 编译Block
        /// </summary>
        public static void CompileBlock(GenFuncInfo fi,Block node)
        {
            for (int i = 0; i < node.Stats.Length; i++)
            {
                //编译语句
            }

            if (node.ReturnExps != null)
            {
                //编译返回值表达式
            }
        }

        /// <summary>
        /// 编译语句
        /// </summary>
        public static void CompileStat(GenFuncInfo fi , BaseStat stat)
        {
            //根据语句类型调用不同的编译方法

            if (stat is FuncCallStat)
            {
                CompileFuncCallStat(fi, (FuncCallStat)stat);
                return;
            }

            if (stat is BreakStat)
            {
                CompileBreakStat(fi, (BreakStat)stat);
                return;
            }

            if (stat is DoStat)
            {
                CompileDoStat(fi, (DoStat)stat);
                return;
            }

            if (stat is RepeatStat)
            {
                CompileRepeatStat(fi, (RepeatStat)stat);
                return;
            }

            if (stat is WhileStat)
            {
                CompileWhileStat(fi, (WhileStat)stat);
                return;
            }

            if (stat is IfStat)
            {
                CompileIfStat(fi, (IfStat)stat);
                return;
            }

            if (stat is ForNumStat)
            {
                CompileForNumStat(fi, (ForNumStat)stat);
                return;
            }

            if (stat is ForInStat)
            {
                CompileForInStat(fi, (ForInStat)stat);
                return;
            }

            if (stat is AssignStat)
            {
                CompileAssignStat(fi, (AssignStat)stat);
                return;
            }

            if (stat is LocalVarDeclStat)
            {
                CompileLocalVarDeclStat(fi, (LocalVarDeclStat)stat);
                return;
            }

            if (stat is LocalFuncDefStat)
            {
                CompileLocalFuncDefStat(fi, (LocalFuncDefStat)stat);
                return;
            }

            throw new System.Exception("不支持编译的语句：" + stat.GetType());
        }

        /// <summary>
        /// 编译函数调用语句
        /// </summary>
        public static void CompileFuncCallStat(GenFuncInfo fi,FuncCallStat stat)
        {
            //对函数调用表达式求值 但不需要任何返回值
            int reg = fi.AllocReg();

        }

        /// <summary>
        /// 编译Break语句
        /// </summary>
        public static void CompileBreakStat(GenFuncInfo fi, BreakStat stat)
        {

        }


        /// <summary>
        /// 编译Do语句
        /// </summary>
        public static void CompileDoStat(GenFuncInfo fi, DoStat stat)
        {

        }


        /// <summary>
        /// 编译Repeat语句
        /// </summary>
        public static void CompileRepeatStat(GenFuncInfo fi, RepeatStat stat)
        {

        }


        /// <summary>
        /// 编译While语句
        /// </summary>
        public static void CompileWhileStat(GenFuncInfo fi, WhileStat stat)
        {

        }


        /// <summary>
        /// 编译If语句
        /// </summary>
        public static void CompileIfStat(GenFuncInfo fi, IfStat stat)
        {

        }


        /// <summary>
        /// 编译ForNum语句
        /// </summary>
        public static void CompileForNumStat(GenFuncInfo fi, ForNumStat stat)
        {

        }


        /// <summary>
        /// 编译ForIn语句
        /// </summary>
        public static void CompileForInStat(GenFuncInfo fi, ForInStat stat)
        {

        }


        /// <summary>
        /// 编译赋值语句
        /// </summary>
        public static void CompileAssignStat(GenFuncInfo fi, AssignStat stat)
        {

        }


        /// <summary>
        /// 编译局部变量声明语句
        /// </summary>
        public static void CompileLocalVarDeclStat(GenFuncInfo fi, LocalVarDeclStat stat)
        {

        }


        /// <summary>
        /// 编译局部函数定义语句
        /// </summary>
        public static void CompileLocalFuncDefStat(GenFuncInfo fi, LocalFuncDefStat stat)
        {
            int reg = fi.AddLocalVar(stat.Name);
            CompileFuncDefExp(fi, stat.FuncDefExp, reg);
        }

        /// <summary>
        /// 编译返回值表达式
        /// </summary>
        public static void CompileReturnExp(GenFuncInfo fi, BaseExp[] exps)
        {
            if (exps.Length == 0)
            {
                //没返回值
                fi.EmitReturn(0, 0);
                return;
            }

            //最后一个表达式是否是vararg或者函数调用
            bool multReturn = IsVarargOrFuncCall(exps[exps.Length - 1]);

            //循环编译所有表达式
            for (int i = 0; i < exps.Length; i++)
            {
                BaseExp exp = exps[i];

                int reg = fi.AllocReg();
                if (i == exps.Length - 1 && multReturn)
                {
                    CompileExp(fi, exp, reg, -1);
                }
                else
                {
                    CompileExp(fi, exp, reg, 1);
                }
            }

            fi.FreeRegs(exps.Length);

            //返回值的寄存器索引起点
            int a = fi.UsedRegs;

            //根据是否有变长返回值 决定b参数
            if (multReturn)
            {
                
                fi.EmitReturn(a, -1);
            }
            else
            {
                fi.EmitReturn(a, exps.Length);
            }
        }

        /// <summary>
        /// 表达式是否为vararg或函数调用
        /// </summary>
        public static bool IsVarargOrFuncCall(BaseExp exp)
        {
            return exp is VarargExp || exp is FuncCallExp;
        }

        /// <summary>
        /// 编译表达式
        /// </summary>
        public static void CompileExp(GenFuncInfo fi,BaseExp exp,int reg,int x)
        {
            //todo
        }

        /// <summary>
        /// 编译函数定义表达式
        /// </summary>
        public static void CompileFuncDefExp(GenFuncInfo fi, BaseExp exp, int reg)
        {

        }

        /// <summary>
        /// 编译函数调用表达式
        /// </summary>
        public static void CompileFuncCallStat(GenFuncInfo fi, BaseExp exp, int reg,int x)
        {

        }


    }
}

