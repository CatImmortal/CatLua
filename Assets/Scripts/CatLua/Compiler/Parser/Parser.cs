using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// 语法解析器（递归下降解析）
    /// </summary>
    public class Parser
    {
        /// <summary>
        /// 解析Block
        /// </summary>
        public Block ParseBlock(Lexer lexer)
        {
            Block block = new Block();
            block.LastLine = lexer.Line;
            block.Stats = ParseStats(lexer);
            block.ReturnExps = ParseReturnExps(lexer);

            return block;
        }

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
        /// 是否为Return或Block结束
        /// </summary>
        private bool IsReturnOrBlockEnd(TokenType type)
        {
            switch (type)
            {
                case TokenType.Eof:
                case TokenType.KwReturn:
                case TokenType.KwEnd:
                case TokenType.KwElse:
                case TokenType.KwElseif:
                case TokenType.KwUntil:
                    return true;
          
            }

            return false;
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
                    break;
                case TokenType.KwBreak:
                    break;
                case TokenType.KwDo:
                    break;
                case TokenType.KwWhile:
                    break;
                case TokenType.KwRepeat:
                    break;
                case TokenType.KwIf:
                    break;
                case TokenType.KwFor:
                    break;
                case TokenType.KwFunction:
                    break;
                case TokenType.KwLocal:
                    break;
                default:
                    break;
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

            return new WhileStat(exp,block);
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
                lexer.GetNextToken(out _, out _,out _);

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
                TrueExp trueExp = new TrueExp(lexer.Line, -1);
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
        private ForNumStat ParseForNumStat(Lexer lexer,int lineOfFor,string varName)
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
                stepExp = new IntegerExp(lexer.Line, -1, 1);
            }

            //跳过do
            lexer.GetNextTokenOfType(TokenType.KwDo, out int lineOfDo, out _);

            //解析block
            Block block = ParseBlock(lexer);

            //跳过end
            lexer.GetNextTokenOfType(TokenType.KwEnd, out _, out _);

            return new ForNumStat(lineOfFor, lineOfDo,varName, initExp, limitExp, stepExp, block);
        }

        /// <summary>
        /// 解析通用for语句
        /// </summary>
        private ForInStat ParseForInStat(Lexer lexer,string name0)
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
        /// 解析名字列表
        /// </summary>
        private string[] ParseNameList(Lexer lexer,string name0)
        {
            List<string> names = new List<string>();
            names.Add(name0);

            while (lexer.LookNextTokenType() == TokenType.SepComma)
            {
                //跳过逗号
                lexer.GetNextToken(out _, out _, out _);

                //提取标识符
                lexer.GetNextIdentifier(out _, out string name);

                names.Add(name);
            }

            return names.ToArray();
        }

        /// <summary>
        /// 解析局部变量声明或函数定义语句
        /// </summary>
        private BaseStat ParseLocalAssignOrFuncDefStat(Lexer lexer)
        {
            //跳过local
            lexer.GetNextTokenOfType(TokenType.KwLocal, out _, out _);

            //前瞻1个token
            if (lexer.LookNextTokenType() == TokenType.KwFunction)
            {
                //是function 函数定义
            }
            else
            {
                //否则是局部变量声明
            }
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

            if (prefixExp is FuncCallExp)
            {
                //这个前缀表达式是一个函数调用表达式，那就是函数调用语句
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
        private AssignStat ParseAssignStat(Lexer lexer,BaseExp var0)
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
        /// 解析var表达式列表
        /// </summary>
        private BaseExp[] ParseVarList(Lexer lexer,BaseExp var0)
        {
            List<BaseExp> vars = new List<BaseExp>();
            vars.Add(CheckVar(lexer,var0));

            while (lexer.LookNextTokenType() == TokenType.SepComma)
            {
                //跳过逗号
                lexer.GetNextToken(out _, out _, out _);

                //解析var表达式
                BaseExp exp = ParsePrefixExp(lexer);
                vars.Add(CheckVar(lexer, exp));
            }

            return vars.ToArray();
        }

        /// <summary>
        /// 检查exp是否为var表达式
        /// </summary>
        private BaseExp CheckVar(Lexer lexer,BaseExp exp)
        {
            if (exp is NameExp || exp is TableAccessExp)
            {
                return exp;
            }

            lexer.Error("目标exp不是var表达式");
            return null;
        }

        /// <summary>
        /// 解析返回值表达式
        /// </summary>
        private BaseExp[] ParseReturnExps(Lexer lexer)
        {
            if (lexer.LookNextTokenType() != TokenType.KwReturn)
            {
                //没有返回语句 返回null
                return null;
            }

            //跳过return
            lexer.GetNextToken(out int line,out string token ,out TokenType type);

            switch (lexer.LookNextTokenType())
            {
                case TokenType.Eof:
                case TokenType.KwEnd:
                case TokenType.KwElse:
                case TokenType.KwElseif:
                case TokenType.KwUntil:
                    //block结束了 返回空数组
                    return new BaseExp[0];

                case TokenType.SepSemi:
                    //跳过分号 返回空数组
                    lexer.GetNextToken(out line, out token, out type);
                    return new BaseExp[0];

                default:
                    //解析return后的所有表达式
                    BaseExp[] exps = ParseExpList(lexer);

                    if (lexer.LookNextTokenType() == TokenType.SepSemi)
                    {
                        //跳过分号
                        lexer.GetNextToken(out line, out token, out type);
                    }

                    return exps;
            }
        }

        /// <summary>
        /// 解析表达式序列
        /// </summary>
        private BaseExp[] ParseExpList(Lexer lexer)
        {
            List<BaseExp> exps = new List<BaseExp>();

            while (lexer.LookNextTokenType() == TokenType.SepComma)
            {
                //跳过逗号
                lexer.GetNextToken(out int line, out string token, out TokenType type);

                //解析表达式
                BaseExp exp = ParseExp(lexer);
                exps.Add(exp);
            }

            return exps.ToArray();
        }

        /// <summary>
        /// 解析表达式
        /// </summary>
        private BaseExp ParseExp(Lexer lexer)
        {

        }

        /// <summary>
        /// 解析前缀表达式
        /// </summary>
        private BaseExp ParsePrefixExp(Lexer lexer)
        {

        }

    }

}
