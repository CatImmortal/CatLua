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
            CompileFuncCallExp(fi, stat, reg, 0);
        }

        /// <summary>
        /// 编译Break语句
        /// </summary>
        public static void CompileBreakStat(GenFuncInfo fi, BreakStat stat)
        {
            //生成参数都为0的jump指令
            int pc = fi.EmitJmp(0, 0);

            //添加到Break表中 等块结束时修复
            fi.AddBreakJump(pc);
        }


        /// <summary>
        /// 编译Do语句
        /// </summary>
        public static void CompileDoStat(GenFuncInfo fi, DoStat stat)
        {
            fi.EnterScope(false);

            CompileBlock(fi, stat.block);

            fi.CloseOpenUpvalue();

            fi.ExitScope();
        }


        /// <summary>
        /// 编译Repeat语句
        /// </summary>
        public static void CompileRepeatStat(GenFuncInfo fi, RepeatStat stat)
        {
            fi.EnterScope(true);

            //记录当前pc
            int startPC = fi.PC;

            //处理块
            CompileBlock(fi, stat.Block);

            //分配一个变量 对表达式求值
            int r = fi.AllocReg();
            CompileExp(fi, stat.Exp, r, 1);
            fi.FreeReg();

            //生成test和jmp指令 
            fi.EmitTest(r, 0); //r处的值不为false 就跳过接下来回到循环体开始处的jmp指令 结束循环
            fi.EmitJmp(0, startPC - fi.PC - 1);  //跳回最开始

            fi.CloseOpenUpvalue();

            fi.ExitScope();
        }


        /// <summary>
        /// 编译While语句
        /// </summary>
        public static void CompileWhileStat(GenFuncInfo fi, WhileStat stat)
        {
            //记录当前pc
            int startPC = fi.PC;

            //分配一个变量 对表达式求值
            int r = fi.AllocReg();
            CompileExp(fi, stat.Exp, r, 1);
            fi.FreeReg();

            //生成test和jmp指令 
            fi.EmitTest(r, 0);  //r处的值不为false 就跳过接下来结束循环的jmp指令 循环运行block
            int jmpToEndPC = fi.EmitJmp(0, 0); //结束循环 此时块还没结束 跳转偏移量暂时设为0

            //对块进行处理
            fi.EnterScope(true);

            CompileBlock(fi, stat.Block);
            fi.CloseOpenUpvalue();
            fi.EmitJmp(0, startPC - fi.PC - 1);  //跳回最开始

            fi.ExitScope();

            //修复结束循环的jmp指令的偏移量
            fi.FixSbx(jmpToEndPC, fi.PC - jmpToEndPC);
        }


        /// <summary>
        /// 编译If语句
        /// </summary>
        public static void CompileIfStat(GenFuncInfo fi, IfStat stat)
        {
            int[] jmpToEndPCs = new int[stat.Exps.Length];

            //跳到下一个if表达式的PC
            int jmpToNextExpPC = -1;

            for (int i = 0; i < stat.Exps.Length; i++)
            {
                BaseExp exp = stat.Exps[i];

                //修复跳转到下一个elseif的jmp指令的偏移量
                if (jmpToNextExpPC >= 0)
                {
                    fi.FixSbx(jmpToNextExpPC, fi.PC - jmpToNextExpPC);
                }

                //分配一个变量 对表达式求值
                int r = fi.AllocReg();
                CompileExp(fi, exp, r, 1);
                fi.FreeReg();

                //生成test和jmp指令 
                fi.EmitTest(r, 0); //r处的值不为false 就跳过接下来 跳转到下一个elseif 的jmp指令 直接进入block
                jmpToNextExpPC = fi.EmitJmp(0, 0);  //跳转到下一个elseif 此时还不知道具体pc 跳转偏移量暂时设为0

                //处理块
                fi.EnterScope(true);

                CompileBlock(fi, stat.Blocks[i]);
                fi.CloseOpenUpvalue();

                fi.ExitScope();
            }
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
            //1个局部变量+1个函数定义表达式
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
        public static void CompileFuncCallExp(GenFuncInfo fi, FuncCallStat stat, int reg,int x)
        {

        }


    }
}

