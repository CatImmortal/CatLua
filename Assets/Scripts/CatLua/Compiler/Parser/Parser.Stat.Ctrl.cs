using System.Collections.Generic;

namespace CatLua
{
    public partial class Parser
    {

        /// <summary>
        /// 解析break语句
        /// </summary>>
        private static BreakStat ParseBreakStat(Lexer lexer)
        {
            //跳过break
            lexer.GetNextTokenOfType(TokenType.KwBreak, out _, out _);
            return new BreakStat();
        }

        /// <summary>
        /// 解析while语句
        /// </summary>>
        private static WhileStat ParseWhileStat(Lexer lexer)
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
        private static RepeatStat ParseRepeatStat(Lexer lexer)
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
        private static IfStat ParseIfStat(Lexer lexer)
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
        private static BaseStat ParseForStat(Lexer lexer)
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
        private static ForNumStat ParseForNumStat(Lexer lexer, int lineOfFor, string varName)
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
        private static ForInStat ParseForInStat(Lexer lexer, string name0)
        {
            //解析in前面的变量列表
            string[] nameList = ParseNameList(lexer, name0);

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

            return new ForInStat(lineOfDo, nameList, exps, block);
        }
    }

}
