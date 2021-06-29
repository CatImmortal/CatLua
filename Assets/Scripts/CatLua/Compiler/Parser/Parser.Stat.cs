using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    public partial class Parser
    {
        /// <summary>
        /// 解析语句序列
        /// </summary>
        private BaseStat[] ParseStats(Lexer lexer)
        {
            List<BaseStat> stats = new List<BaseStat>();

            while (!IsReturnOrBlockEnd(lexer.LookNextTokenType()))
            {
                //下一个token 不是 return或block结束 就解析一条语句
                BaseStat stat = ParseStat(lexer);

                if (!(stat is EmptyStat))
                {
                    //不是空语句 放入列表
                    stats.Add(stat);
                }
            }

            return stats.ToArray();
        }

        /// <summary>
        /// 解析语句
        /// </summary>
        private BaseStat ParseStat(Lexer lexer)
        {
            //前瞻一个token 根据类型调用对应的解析语句
            switch (lexer.LookNextTokenType())
            {
                case TokenType.SepSemi:
                    return ParseEmptyStat(lexer);
                case TokenType.KwBreak:
                    return ParseBreakStat(lexer);
                case TokenType.KwDo:
                    return ParseDoStat(lexer);
                case TokenType.KwWhile:
                    return ParseWhileStat(lexer);
                case TokenType.KwRepeat:
                    return ParseRepeatStat(lexer);
                case TokenType.KwIf:
                    return ParseIfStat(lexer);
                case TokenType.KwFor:
                    return ParseForStat(lexer);
                case TokenType.KwFunction:
                    return ParseFuncDefStat(lexer);
                case TokenType.KwLocal:
                    return ParseLocalStat(lexer);
                default:
                    return ParseAssignOrFuncCallStat(lexer);
            }
        }

        /// <summary>
        /// 解析空语句
        /// </summary>>
        private EmptyStat ParseEmptyStat(Lexer lexer)
        {
            //空语句直接跳过分号
            lexer.GetNextTokenOfType(TokenType.SepSemi, out int line, out string token);
            return new EmptyStat();
        }

        /// <summary>
        /// 解析break语句
        /// </summary>>
        private BreakStat ParseBreakStat(Lexer lexer)
        {
            //跳过break关键字
            lexer.GetNextTokenOfType(TokenType.KwBreak, out int line, out string token);
            return new BreakStat();
        }

        /// <summary>
        /// 解析do语句
        /// </summary>>
        private DoStat ParseDoStat(Lexer lexer)
        {
            //跳过do
            lexer.GetNextTokenOfType(TokenType.KwDo, out _, out _);

            //解析block
            Block block = ParseBlock(lexer);

            //跳过end
            lexer.GetNextTokenOfType(TokenType.KwEnd, out _, out _);

            return new DoStat(block);
        }

        /// <summary>
        /// 解析while语句
        /// </summary>>
        private WhileStat ParseWhileStat(Lexer lexer)
        {
            //跳过while
            lexer.GetNextTokenOfType(TokenType.KwWhile, out _, out _);

            //解析exp
            BaseExp exp = ParseExp(lexer);

            //跳过do
            lexer.GetNextTokenOfType(TokenType.KwDo, out _, out _);

            //解析block
            Block block = ParseBlock(lexer);

            //跳过end
            lexer.GetNextTokenOfType(TokenType.KwEnd, out _, out _);

            return new WhileStat(exp, block);
        }


        /// <summary>
        /// 解析repeat语句
        /// </summary>>
        private RepeatStat ParseRepeatStat(Lexer lexer)
        {
            //跳过repeat
            lexer.GetNextTokenOfType(TokenType.KwRepeat, out _, out _);

            //解析block
            Block block = ParseBlock(lexer);

            //跳过until
            lexer.GetNextTokenOfType(TokenType.KwUntil, out _, out _);

            //解析exp
            BaseExp exp = ParseExp(lexer);

            return new RepeatStat(block, exp);
        }

        /// <summary>
        /// 解析if语句
        /// </summary>>
        private IfStat ParseIfStat(Lexer lexer)
        {
            List<BaseExp> exps = new List<BaseExp>();
            List<Block> blocks = new List<Block>();

            //跳过if
            lexer.GetNextTokenOfType(TokenType.KwIf, out _, out _);

            //解析exp
            BaseExp exp = ParseExp(lexer);
            exps.Add(exp);

            //跳过then
            lexer.GetNextTokenOfType(TokenType.KwThen, out _, out _);

            //解析block
            Block block = ParseBlock(lexer);
            blocks.Add(block);

            while (lexer.LookNextTokenType() == TokenType.KwElseif)
            {
                //解析elseif 部分

                //跳过elseif
                lexer.GetNextToken(out _, out _, out _);

                //解析exp
                exp = ParseExp(lexer);
                exps.Add(exp);

                //跳过then
                lexer.GetNextTokenOfType(TokenType.KwThen, out _, out _);

                //解析block
                block = ParseBlock(lexer);
                blocks.Add(block);

            }

            if (lexer.LookNextTokenType() == TokenType.KwElse)
            {
                //解析else 部分


                //跳过else
                lexer.GetNextToken(out _, out _, out _);

                //else 等于elseif true
                TrueExp trueExp = new TrueExp(lexer.Line);
                exps.Add(trueExp);

                //解析block
                block = ParseBlock(lexer);
                blocks.Add(block);
            }



            //跳过end
            lexer.GetNextTokenOfType(TokenType.KwEnd, out _, out _);

            return new IfStat(exps.ToArray(), blocks.ToArray());
        }

        /// <summary>
        /// 解析for语句
        /// </summary>
        private BaseStat ParseForStat(Lexer lexer)
        {
            //跳过for
            lexer.GetNextTokenOfType(TokenType.KwFor, out int lineOfFor, out _);

            //提取1个标识符
            lexer.GetNextIdentifier(out _, out string name);

            //前瞻1个token
            if (lexer.LookNextTokenType() == TokenType.OpAsssign)
            {
                //是赋值符号 数值for
                return ParseForNumStat(lexer, lineOfFor, name);
            }
            else
            {
                //否则是通用for
                return ParseForInStat(lexer, name);
            }

        }

        /// <summary>
        /// 解析数值for语句
        /// </summary>
        private ForNumStat ParseForNumStat(Lexer lexer, int lineOfFor, string varName)
        {
            //跳过赋值符号
            lexer.GetNextTokenOfType(TokenType.OpAsssign, out _, out _);

            //解析初始化表达式
            BaseExp initExp = ParseExp(lexer);

            //跳过逗号
            lexer.GetNextTokenOfType(TokenType.SepComma, out _, out _);

            //解析限制表达式
            BaseExp limitExp = ParseExp(lexer);

            //解析步长表达式
            BaseExp stepExp;
            if (lexer.LookNextTokenType() == TokenType.SepComma)
            {
                lexer.GetNextToken(out _, out _, out _);
                stepExp = ParseExp(lexer);
            }
            else
            {
                //没显式指定步长 默认为1
                stepExp = new IntegerExp(lexer.Line, 1);
            }

            //跳过do
            lexer.GetNextTokenOfType(TokenType.KwDo, out int lineOfDo, out _);

            //解析block
            Block block = ParseBlock(lexer);

            //跳过end
            lexer.GetNextTokenOfType(TokenType.KwEnd, out _, out _);

            return new ForNumStat(lineOfFor, lineOfDo, varName, initExp, limitExp, stepExp, block);
        }

        /// <summary>
        /// 解析通用for语句
        /// </summary>
        private ForInStat ParseForInStat(Lexer lexer, string name0)
        {
            string[] names = ParseNameList(lexer, name0);

            //跳过in
            lexer.GetNextTokenOfType(TokenType.KwIn, out _, out _);

            //解析in后跟着的表达式列表
            BaseExp[] exps = ParseExpList(lexer);

            //跳过do
            lexer.GetNextTokenOfType(TokenType.KwDo, out int lineOfDo, out _);

            //解析block
            Block block = ParseBlock(lexer);

            //跳过end
            lexer.GetNextTokenOfType(TokenType.KwEnd, out _, out _);

            return new ForInStat(lineOfDo, names, exps, block);
        }

        /// <summary>
        /// 解析局部函数定义语句或局部变量声明语句
        /// </summary>
        private BaseStat ParseLocalStat(Lexer lexer)
        {
            //跳过local
            lexer.GetNextTokenOfType(TokenType.KwLocal, out _, out _);

            //前瞻1个token
            if (lexer.LookNextTokenType() == TokenType.KwFunction)
            {
                //local后接function 是局部函数定义
                return ParseLocalFuncDefStat(lexer);
            }
            else
            {
                //否则是局部变量声明
                return ParseLocalVarDeclStat(lexer);
            }
        }

        /// <summary>
        /// 解析局部函数定义语句
        /// </summary>
        private LocalFuncDefStat ParseLocalFuncDefStat(Lexer lexer)
        {
            //跳过function
            lexer.GetNextTokenOfType(TokenType.KwFunction, out _, out _);

            //解析函数名
            lexer.GetNextIdentifier(out _, out string name);

            //解析函数定义表达式
            FuncDefExp exp = ParseFuncDefExp(lexer);

            return new LocalFuncDefStat(name, exp);
        }

        /// <summary>
        /// 解析局部变量声明语句
        /// </summary>
        private LocalVarDeclStat ParseLocalVarDeclStat(Lexer lexer)
        {
            //提取一个变量名
            lexer.GetNextIdentifier(out _, out string name0);

            //解析逗号后跟着的其他变量名
            string[] nameList = ParseNameList(lexer, name0);

            BaseExp[] expList = null;
            if (lexer.LookNextTokenType() == TokenType.OpAsssign)
            {
                //有赋值符号

                //跳过赋值符号
                lexer.GetNextToken(out _, out _, out _);

                //解析赋值符号后面的表达式
                expList = ParseExpList(lexer);
            }

            return new LocalVarDeclStat(lexer.Line, nameList, expList);
        }

        /// <summary>
        /// 解析赋值语句或函数调用语句
        /// </summary>
        private BaseStat ParseAssignOrFuncCallStat(Lexer lexer)
        {
            //解析前缀表达式
            BaseExp prefixExp = ParsePrefixExp(lexer);

            if (prefixExp is FuncCallExp exp)
            {
                //这个前缀表达式是一个函数调用表达式，那就是函数调用语句
                return new FuncCallStat(exp);
            }
            else
            {
                //否则是赋值语句

                return ParseAssignStat(lexer, prefixExp);
            }
        }

        /// <summary>
        /// 解析赋值语句
        /// </summary>
        private AssignStat ParseAssignStat(Lexer lexer, BaseExp var0)
        {
            //解析赋值号左侧的var表达式列表
            BaseExp[] varList = ParseVarList(lexer, var0);

            //跳过赋值符号
            lexer.GetNextTokenOfType(TokenType.OpAsssign, out _, out _);

            //解析赋值号右侧的表达式列表
            BaseExp[] expList = ParseExpList(lexer);

            return new AssignStat(lexer.Line, varList, expList);
        }

        /// <summary>
        /// 解析非局部函数定义语句（赋值语句的语法糖）
        /// </summary>
        private AssignStat ParseFuncDefStat(Lexer lexer)
        {
            //跳过function
            lexer.GetNextTokenOfType(TokenType.KwFunction, out _, out _);

            //解析函数名
            bool hasColon = ParseFuncName(lexer, out BaseExp funcNameExp);

            //解析函数体
            FuncDefExp funcDefExp = ParseFuncDefExp(lexer);

            if (hasColon)
            {
                //有冒号 将隐式参数self放入参数列表首位
                List<string> paramList = new List<string>(funcDefExp.ParamList);
                paramList.Insert(0, "self");
                funcDefExp.ParamList = paramList.ToArray();
            }

            return new AssignStat(funcDefExp.Line, new BaseExp[] { funcNameExp }, new BaseExp[] { funcDefExp });
        }
    }
}

