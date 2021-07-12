using System.Collections;
using System.Collections.Generic;
using System;

namespace CatLua
{
    public static partial class Compiler
    {
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

            //参数a是返回值的寄存器索引起点
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
        public static void CompileExp(GenFuncInfo fi, BaseExp exp, int reg, int num)
        {
            if (exp is NilExp)
            {
                fi.EmitLoadNil(reg, num);
                return;
            }

            if (exp is FalseExp)
            {
                fi.EmitLoadBool(reg, 0, 0);
                return;
            }

            if (exp is TrueExp)
            {
                fi.EmitLoadBool(reg, 1, 0);
                return;
            }

            if (exp is IntegerExp)
            {

                fi.EmitLoadK(reg, ((IntegerExp)exp).Val);
                return;
            }

            if (exp is FloatExp)
            {

                fi.EmitLoadK(reg, ((FloatExp)exp).Val);
                return;
            }

            if (exp is StringExp)
            {

                fi.EmitLoadK(reg, ((StringExp)exp).Str);
                return;
            }

            if (exp is ParensExp)
            {
                CompileExp(fi, ((ParensExp)exp).Exp, reg, num);
                return;
            }

            if (exp is VarargExp)
            {
                CompileVarargExp(fi, (VarargExp)exp, reg, num);
                return;
            }

            if (exp is FuncDefExp)
            {
                CompileFuncDefExp(fi, (FuncDefExp)exp, reg);
                return;
            }

            if (exp is TableConstructorExp)
            {
                CompileTableConstructorExp(fi, (TableConstructorExp)exp, reg);
                return;
            }

            if (exp is UnopExp)
            {
                CompileUnopExp(fi, (UnopExp)exp, reg);
                return;
            }

            if (exp is BinopExp)
            {
                CompileBinopExp(fi, (BinopExp)exp, reg);
                return;
            }

            if (exp is ConcatExp)
            {
                CompileConcatExp(fi, (ConcatExp)exp, reg);
                return;
            }

            if (exp is NameExp)
            {
                CompileNameExp(fi, (NameExp)exp, reg);
                return;
            }

            if (exp is TableAccessExp)
            {
                CompileTableAccessExp(fi, (TableAccessExp)exp, reg);
                return;
            }

            if (exp is FuncCallExp)
            {
                CompileFuncCallExp(fi, (FuncCallExp)exp, reg, num);
                return;
            }

            throw new Exception("不支持编译的表达式：" + exp.GetType());
        }

        /// <summary>
        /// 编译Vararg表达式
        /// </summary>
        public static void CompileVarargExp(GenFuncInfo fi, VarargExp exp, int reg, int num)
        {
            if (!fi.IsVararg)
            {
                throw new Exception("要编译的Vararg表达式所在函数不是vararg函数");
            }
            fi.EmitVararg(reg, num);
        }


        /// <summary>
        /// 编译函数定义表达式
        /// </summary>
        public static void CompileFuncDefExp(GenFuncInfo fi, FuncDefExp exp, int reg)
        {
            GenFuncInfo child = new GenFuncInfo(fi, exp);
            fi.Children.Add(child);

            //把子函数的固定参数作为子函数的局部变量添加进去
            for (int i = 0; i < exp.ParamList.Length; i++)
            {
                child.AddLocalVar(exp.ParamList[i]);
            }

            CompileBlock(child, exp.Block);

            child.ExitScope();
            child.EmitReturn(0, 0);

            //子函数位置
            int bx = fi.Children.Count - 1;

            fi.EmitClosure(reg, bx);
        }

        /// <summary>
        /// 编译表构造表达式
        /// </summary>
        public static void CompileTableConstructorExp(GenFuncInfo fi, TableConstructorExp exp, int reg)
        {

        }

        /// <summary>
        /// 编译一元运算表达式
        /// </summary>
        public static void CompileUnopExp(GenFuncInfo fi, UnopExp exp, int reg)
        {

        }

        /// <summary>
        /// 编译二元运算表达式
        /// </summary>
        public static void CompileBinopExp(GenFuncInfo fi, BinopExp exp, int reg)
        {

        }

        /// <summary>
        /// 编译ConCat表达式
        /// </summary>
        public static void CompileConcatExp(GenFuncInfo fi, ConcatExp exp, int reg)
        {

        }

        /// <summary>
        /// 编译名字表达式
        /// </summary>
        public static void CompileNameExp(GenFuncInfo fi, NameExp exp, int reg)
        {

        }


        /// <summary>
        /// 编译表访问表达式
        /// </summary>
        public static void CompileTableAccessExp(GenFuncInfo fi, TableAccessExp exp, int reg)
        {

        }

        /// <summary>
        /// 编译函数调用表达式
        /// </summary>
        public static void CompileFuncCallExp(GenFuncInfo fi, FuncCallExp exp, int reg, int num)
        {

        }
    }
}

