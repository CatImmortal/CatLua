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
        private static void CompileReturnExp(GenFuncInfo fi, BaseExp[] exps)
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
        private static bool IsVarargOrFuncCall(BaseExp exp)
        {
            return exp is VarargExp || exp is FuncCallExp;
        }

        /// <summary>
        /// 编译表达式 
        /// </summary>
        private static void CompileExp(GenFuncInfo fi, BaseExp exp, int reg, int num)
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
        private static void CompileVarargExp(GenFuncInfo fi, VarargExp exp, int reg, int num)
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
        private static void CompileFuncDefExp(GenFuncInfo fi, FuncDefExp exp, int reg)
        {
            GenFuncInfo child = new GenFuncInfo(fi, exp);
            fi.Children.Add(child);

            //把子函数的固定参数作为子函数的局部变量添加进去
            for (int i = 0; i < exp.ParamList.Length; i++)
            {
                child.AddLocalVar(exp.ParamList[i]);
            }

            //编译函数体
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
        private static void CompileTableConstructorExp(GenFuncInfo fi, TableConstructorExp exp, int reg)
        {
            //计算数组部分的长度
            int arrLength = 0;
            for (int i = 0; i < exp.KeyExps.Length; i++)
            {
                if (exp.KeyExps[i]  == null)
                {
                    //对应的key为null的就是数组部分了
                    arrLength++;
                }
            }

            //最后一个value是否为变长的
            int keyNum = exp.KeyExps.Length;
            bool multRet = keyNum > 0 && IsVarargOrFuncCall(exp.ValueExps[keyNum - 1]);

            //生成newtable指令
            fi.EmitNewTable(reg, arrLength, keyNum - arrLength);

            int arrIndex = 0;

            for (int i = 0; i < keyNum; i++)
            {
                BaseExp keyExp = exp.KeyExps[i];
                BaseExp valueExp = exp.ValueExps[i];

                if (keyExp == null)
                {
                    //数组部分
                    arrIndex++;
                    int temp = fi.AllocReg();

                   
                    if (i == keyNum - 1 && multRet)
                    {
                        //处理放在末尾的变长的value
                        CompileExp(fi, valueExp, temp, -1);
                    }
                    else
                    {
                        CompileExp(fi, valueExp, temp, 1);
                    }


                    if (arrIndex % 50 == 0 || arrIndex == arrLength)
                    {
                        //为数组部分生成 setlist指令
                        int n = arrIndex % 50;  //当前批次要处理的元素数量
                        if (n == 0)
                        {
                            n = 50;
                        }

                        int c = (arrIndex - 1) / 50 + 1;  //批次
                        fi.FreeRegs(n);  //批次更新 重置寄存器索引

                        if (i == keyNum - 1 && multRet)
                        {
                            //处理变长value
                            fi.EmitSetList(reg, 0, c);
                        }
                        else
                        {
                            fi.EmitSetList(reg, n, c);
                        }
                    }
                }
                else
                {
                    //hash部分

                    int keyReg = fi.AllocReg();
                    CompileExp(fi, keyExp, keyReg, 1);

                    int valueReg = fi.AllocReg();
                    CompileExp(fi, valueExp, valueReg, 1);

                    fi.AllocRegs(2);

                    fi.EmitSetTable(reg, keyReg, valueReg);
                }

            }
        }

        /// <summary>
        /// 编译一元运算表达式
        /// </summary>
        private static void CompileUnopExp(GenFuncInfo fi, UnopExp exp, int reg)
        {
            int b = fi.AllocReg();
            CompileExp(fi, exp.Exp, b, 1);
            fi.EmitUnaryOp(exp.Op, reg, b);
        }

        /// <summary>
        /// 编译二元运算表达式
        /// </summary>
        private static void CompileBinopExp(GenFuncInfo fi, BinopExp exp, int reg)
        {
            switch (exp.Op)
            {
                case TokenType.OpAnd:
                case TokenType.OpOr:

                    //特殊处理 and or 实现短路运算
                    //a and b 如果a为true 就以b的值为结果 否则以a的值为结果
                    //a or b 如果a为false 就以b的值为结果 否则以a的值为结果

                    //编译第一个操作数的表达式
                    int b = fi.AllocReg();
                    CompileExp(fi, exp.Exp1, b, 1);
                    fi.FreeReg();

                    if (exp.Op == TokenType.OpAnd)
                    {
                        //and运算 如果第一个操作数为false 就通过jmp指令跳过对第二个操作数的计算 直接使用这个结果 否则使用第二个操作数的结果
                        fi.EmitTestSet(reg, b, 0);
                    }
                    else
                    {
                        //or运算 如果第一个操作数为true 就通过jmp指令跳过对第二个操作数的计算 直接使用这个结果 否则使用第二个操作数的结果
                        fi.EmitTestSet(reg, b, 1);  
                    }

                    
                    int JmpPC = fi.EmitJmp(0, 0);  //跳过对第二个操作数的计算

                    //编译第二个操作数的表达式
                    b = fi.AllocReg();
                    CompileExp(fi, exp.Exp2, b, 1);
                    fi.FreeReg();
                    fi.EmitMove(reg, b);

                    fi.FixSbx(JmpPC, fi.PC - JmpPC);

                    break;

                default:
                    //其他二元运算
                    b = fi.AllocReg();
                    CompileExp(fi, exp.Exp1, b, 1);

                    int c = fi.AllocReg();
                    CompileExp(fi, exp.Exp2, c, 1);

                    fi.EmitBinaryOp(exp.Op, reg, b, c);
                    fi.FreeRegs(2);

                    break;
            }
        }

        /// <summary>
        /// 编译ConCat表达式
        /// </summary>
        private static void CompileConcatExp(GenFuncInfo fi, ConcatExp exp, int reg)
        {
            for (int i = 0; i < exp.Exps.Length; i++)
            {
                int a = fi.AllocReg();
                CompileExp(fi, exp.Exps[i], a, 1);
            }

            int c = fi.UsedRegs - 1;  //连接的值的结束位置
            int b = c - exp.Exps.Length + 1;  //连接的值的起始位置
            fi.FreeRegs(c - b + 1); 
            fi.EmitConcat(reg, b, c);
        }

        /// <summary>
        /// 编译名字表达式
        /// </summary>
        private static void CompileNameExp(GenFuncInfo fi, NameExp exp, int reg)
        {
            //可能是局部变量 upvalue 全局变量

            int slot = fi.SlotOfLocalVar(exp.Name);
            if (slot > 0)
            {
                //局部变量
                fi.EmitMove(reg, slot);
            }else
            {
                int index = fi.IndexOfUpvalue(exp.Name);
                if (index > 0)
                {
                    //upvalue
                    fi.EmitGetUpvalue(reg, index);
                }
                else
                {
                    //全局变量

                    ////转换为对Upvalues里的_ENV表的表访问表达式
                    //TableAccessExp taExp = new TableAccessExp(exp.Line, new NameExp(0, "_ENV"), new StringExp(exp.Line, exp.Name));
                    //CompileTableAccessExp(fi, taExp, reg);

                    int b = fi.IndexOfUpvalue("_ENV");
                    int c = fi.IndexOfConstant(new LuaConstantUnion(LuaConstantType.ShortStr, str: exp.Name));
                    fi.EmitGetTabUp(reg,b,c);
                }
            }
        }


        /// <summary>
        /// 编译表访问表达式
        /// </summary>
        private static void CompileTableAccessExp(GenFuncInfo fi, TableAccessExp exp, int reg)
        {
            //分别对table和key的表达式求值
            int b = fi.AllocReg();
            CompileExp(fi, exp.PrefixExp, b, 1);

            int c = fi.AllocReg();
            CompileExp(fi, exp.KeyExp, c, 1);

            fi.EmitGetTable(reg, b, c);
            fi.FreeRegs(2);
        }

        /// <summary>
        /// 编译函数调用表达式
        /// </summary>
        private static void CompileFuncCallExp(GenFuncInfo fi, FuncCallExp exp, int reg, int num)
        {
            //参数数量
            int argsNum = exp.Args.Length;

            //最后一个参数是否为vararg或函数调用
            bool LastArgIsVarargOrFuncCall = false;

            //先编译前缀表达式
            CompileExp(fi, exp.PrefixExp, reg, 1);

            //处理冒号调用
            if (exp.NameExp != null)
            {
                int c = fi.IndexOfConstant(new LuaConstantUnion(LuaConstantType.ShortStr,str: exp.NameExp.Str));
                fi.EmitSelf(reg, reg, c);
            }


            //编译参数
            for (int i = 0; i < argsNum; i++)
            {
                BaseExp argExp = exp.Args[i];

                int temp = fi.AllocReg();

                if (i == argsNum - 1 && IsVarargOrFuncCall(argExp))
                {
                    //最后一个参数是变长的
                    LastArgIsVarargOrFuncCall = true;

                    CompileExp(fi, argExp, temp, -1);
                }
                else
                {
                    CompileExp(fi, argExp, temp, 1);
                }
            }

            fi.FreeRegs(argsNum);

            if (exp.NameExp != null)
            {
                //冒号调用 多一个隐式参数self
                argsNum++;
            }

            if (LastArgIsVarargOrFuncCall)
            {
                //末尾有变长参数
                argsNum = -1;
            }

            //编译调用指令
            //todo:argsNum和num应该还需要+1
            fi.EmitCall(reg, argsNum, num);
        }
    }
}

