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

            //添加到Break表中 等block结束时修复
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

            //遍历所有if/elseif后接着的表达式
            for (int i = 0; i < stat.Exps.Length; i++)
            {
                BaseExp exp = stat.Exps[i];

                //修复在上一次循环中生成的 跳转到下一个elseif的jmp指令 的偏移量
                if (jmpToNextExpPC >= 0)
                {
                    fi.FixSbx(jmpToNextExpPC, fi.PC - jmpToNextExpPC);
                }

                //分配一个变量 对表达式求值
                int r = fi.AllocReg();
                CompileExp(fi, exp, r, 1);
                fi.FreeReg();

                //生成test和jmp指令 
                fi.EmitTest(r, 0); //r处的值不为false 就跳过接下来 跳转到下一个elseif 的jmp指令 直接进入block执行
                jmpToNextExpPC = fi.EmitJmp(0, 0);  //跳转到下一个elseif 此时还不知道具体pc 跳转偏移量暂时设为0

                //处理块
                fi.EnterScope(true);

                CompileBlock(fi, stat.Blocks[i]);
                fi.CloseOpenUpvalue();

                fi.ExitScope();

                //为每个if的block的生成 跳转到结束位置的jmp指令
                if (i < stat.Exps.Length - 1)
                {
                    jmpToEndPCs[i] = fi.EmitJmp(0, 0);
                }
                else
                {
                    //是最后一条elseif表达式 将to next的jmp指令视为to end的jmp指令处理
                    jmpToEndPCs[i] = jmpToNextExpPC;
                }

            }

            //修复跳转到结束位置的jmp指令的偏移量
            for (int i = 0; i < jmpToEndPCs.Length; i++)
            {
                int pc = jmpToEndPCs[i];
                fi.FixSbx(pc, fi.PC - pc);
            }
        }


        /// <summary>
        /// 编译ForNum语句
        /// </summary>
        public static void CompileForNumStat(GenFuncInfo fi, ForNumStat stat)
        {
            fi.EnterScope(true);

            //声明3个特殊的局部变量 分别存 索引，限制和步长
            LocalVarDeclStat lvds = new LocalVarDeclStat(0, new string[] { "(for index)", "(for limit)", "(for step)" }, new BaseExp[] { stat.InitExp, stat.LimitExp, stat.StepExp });
            CompileLocalVarDeclStat(fi,lvds);

            //声明跟在for后的局部变量 
            fi.AddLocalVar(stat.VarName);

            //生成forprep指令
            int a = fi.UsedRegs - 4;

            int ForPrepPC = fi.EmitForPrep(a, 0); //跳转到循环条件检测 sbx待修复

            CompileBlock(fi,stat.Block);
            fi.CloseOpenUpvalue();

            int ForLoopPC = fi.EmitForLoop(a, 0); //跳转到循环体 sbx待修复

            //修复sbx  todo:
            fi.FixSbx(ForPrepPC, ForLoopPC - ForPrepPC - 1);
            fi.FixSbx(ForLoopPC, ForPrepPC - ForLoopPC);  //sbx是个负数

            fi.ExitScope();
        }


        /// <summary>
        /// 编译ForIn语句
        /// </summary>
        public static void CompileForInStat(GenFuncInfo fi, ForInStat stat)
        {
            fi.EnterScope(true);

            //声明3个特殊的局部变量 分别存 自定义迭代器函数 要遍历的表 key变量
            LocalVarDeclStat lvds = new LocalVarDeclStat(0, new string[] { "(for generator)", "(for state)", "(for control)" }, new BaseExp[] { stat.InitExp, stat.LimitExp, stat.StepExp });
            CompileLocalVarDeclStat(fi, lvds);

            //声明跟在for后的局部变量 
            for (int i = 0; i < stat.NameList.Length; i++)
            {
                fi.AddLocalVar(stat.NameList[i]);
            }

            int jmpToTForCallPC = fi.EmitJmp(0, 0);  //跳转到TFormCall处 开始循环检测

            //编译循环体
            CompileBlock(fi, stat.Block);
            fi.CloseOpenUpvalue();

            fi.FixSbx(jmpToTForCallPC, fi.PC - jmpToTForCallPC);  //修复sbx

            //迭代器函数的寄存器索引
            int GeneratorReg = fi.SlotOfLocalVar("(for generator)");

            fi.EmitTForCall(GeneratorReg, stat.NameList.Length);

            fi.EmitTForLoop(GeneratorReg + 2, jmpToTForCallPC - fi.PC - 1);  //跳回循环体 继续循环

            fi.ExitScope();
        }


        /// <summary>
        /// 编译赋值语句
        /// </summary>
        public static void CompileAssignStat(GenFuncInfo fi, AssignStat stat)
        {
            int expsNum = stat.ExpList.Length;
            int varsNum = stat.VarList.Length;
            int oldUsedRegs = fi.UsedRegs;

            //分别记录为table，key value 分配的临时变量
            int[] tRegs = new int[varsNum];
            int[] kRegs = new int[varsNum];
            int[] vRegs = new int[varsNum];

            //处理=号左侧的索引表达式，分配临时变量，对table和key求值
            for (int i = 0; i < varsNum; i++)
            {
                BaseExp varExp = stat.VarList[i];

                if (varExp is TableAccessExp taExp)
                {
                    //var表达式是表访问表达式

                    //编译前缀表达式
                    tRegs[i] = fi.AllocReg();
                    CompileExp(fi, taExp.PrefixExp, tRegs[i], 1);

                    //编译key表达式
                    kRegs[i] = fi.AllocReg();
                    CompileExp(fi, taExp.KeyExp, kRegs[i], 1);
                }
            }

            //统一对=号右侧的表达式 计算寄存器索引
            for (int i = 0; i < varsNum; i++)
            {
                vRegs[i] = fi.UsedRegs + i;
            }

            if (expsNum >= varsNum)
            {
                //var 不比 表达式 多

                for (int i = 0; i < expsNum; i++)
                {
                    BaseExp exp = stat.ExpList[i];
                    int a = fi.AllocReg();

                    if (i >= varsNum && i == expsNum - 1 && IsVarargOrFuncCall(exp))
                    {
                        //最后一个表达式 是vararg或函数调用
                        CompileExp(fi, exp, a, 0);
                    }
                    else
                    {
                        CompileExp(fi, exp, a, 1);
                    }
                }
            }
            else
            {
                //var 比 表达式 多
                bool multRet = false;
                for (int i = 0; i < expsNum; i++)
                {
                    BaseExp exp = stat.ExpList[i];
                    int a = fi.AllocReg();

                    if (i == expsNum - 1 && IsVarargOrFuncCall(exp))
                    {
                        //最后一个表达式 是vararg或函数调用
                        multRet = true;
                        int num = varsNum - expsNum + 1;
                        CompileExp(fi, exp, a, num);
                        fi.AllocRegs(num - 1);

                    }
                    else
                    {
                        CompileExp(fi, exp, a, 1);
                    }
                }

                if (!multRet)
                {
                    //没有变长返回值表达式 用nil填充不足的
                    int num = varsNum - expsNum;
                    int a = fi.AllocRegs(num);
                    fi.EmitLoadNil(a, num);
                }
            }

            for (int i = 0; i < varsNum; i++)
            {
                BaseExp exp = stat.VarList[i];

                if (exp is NameExp nExp)
                {
                    //var表达式 是名字表达式

                    int a = fi.SlotOfLocalVar(nExp.Name);
                    if (a >= 0)
                    {
                        //var是个局部变量
                        fi.EmitMove(a, vRegs[i]);
                    }
                    else
                    {
                        int b = fi.IndexOfUpvalue(nExp.Name);
                        if (b >= 0)
                        {
                            //var是个upvalue
                            fi.EmitSetUpValue(vRegs[i], b);
                        }
                        else
                        {
                            //var是个全局变量
                            a = fi.IndexOfUpvalue("_ENV");
                            b = fi.IndexOfConstant(new LuaConstantUnion(LuaConstantType.ShorStr,str: nExp.Name));
                            fi.EmitSetTabUp(a, b, vRegs[i]);

                        }

                    }


                }
                else
                {
                    fi.EmitSetTable(tRegs[i], kRegs[i], vRegs[i]);
                }
            }
        }




        /// <summary>
        /// 编译局部变量声明语句
        /// </summary>
        public static void CompileLocalVarDeclStat(GenFuncInfo fi, LocalVarDeclStat stat)
        {
            int oldUsedRegs = fi.UsedRegs;
            if (stat.NameList.Length == stat.ExpList.Length)
            {
                //=号左侧的变量和右侧的表达式数量一样多
                for (int i = 0; i < stat.ExpList.Length; i++)
                {
                    BaseExp exp = stat.ExpList[i];
                    int a = fi.AllocReg();
                    CompileExp(fi, exp, a, 1);
                }
            }
            else if (stat.NameList.Length < stat.ExpList.Length)
            {
                //变量 比 表达式 少
                for (int i = 0; i < stat.ExpList.Length; i++)
                {
                    BaseExp exp = stat.ExpList[i];
                    int a = fi.AllocReg();

                    //检查最后一个表达式 是否为 vararg或函数调用表达式
                    if (i == stat.ExpList.Length - 1 && IsVarargOrFuncCall(exp))
                    {
                        CompileExp(fi, exp, a, 0);
                    }
                    else
                    {
                        CompileExp(fi, exp, a, 1);
                    }
                }
            }
            else
            {
                //表达式 比 变量 少

                bool multRet = false;  //表达式中，是否有包含变长返回值的表达式

                for (int i = 0; i < stat.ExpList.Length; i++)
                {
                    BaseExp exp = stat.ExpList[i];
                    int a = fi.AllocReg();

                    //检查最后一个表达式 是否为 vararg或函数调用表达式
                    if (i == stat.ExpList.Length - 1 && IsVarargOrFuncCall(exp))
                    {
                        //用多重赋值填充
                        multRet = true;

                        int num = stat.NameList.Length - stat.ExpList.Length;  //变量比表达式多出来的数量
                        CompileExp(fi, exp, a, num);

                        fi.AllocRegs(num - 1);
                    }
                    else
                    {
                        CompileExp(fi, exp, a, 1);
                    }
                }

                if (!multRet)
                {
                    //没有变长返回值表达式 用nil填充不足的
                    int num = stat.NameList.Length - stat.ExpList.Length;
                    int a = fi.AllocRegs(num);
                    fi.EmitLoadNil(a, num);
                }
            }

            //释放临时变量
            fi.UsedRegs = oldUsedRegs;

            //声明局部变量
            for (int i = 0; i < stat.NameList.Length; i++)
            {
                fi.AddLocalVar(stat.NameList[i]);
            }

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

