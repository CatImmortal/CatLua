using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    public partial class Parser
    {
        /// <summary>
        /// 解析返回值表达式
        /// </summary>
        private static BaseExp[] ParseReturnExps(Lexer lexer)
        {
            if (lexer.LookNextTokenType() != TokenType.KwReturn)
            {
                //没有返回语句 返回null
                return null;
            }

            //跳过return
            lexer.GetNextToken(out _, out _, out _);

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
                    lexer.GetNextToken(out _, out _, out _);
                    return new BaseExp[0];

                default:
                    //解析return后的所有表达式
                    BaseExp[] exps = ParseExpList(lexer);

                    if (lexer.LookNextTokenType() == TokenType.SepSemi)
                    {
                        //跳过分号
                        lexer.GetNextToken(out _, out _, out _);
                    }

                    return exps;
            }
        }

        /// <summary>
        /// 解析由逗号分隔的表达式列表
        /// </summary>
        private static BaseExp[] ParseExpList(Lexer lexer)
        {
            List<BaseExp> exps = new List<BaseExp>();

            while (lexer.LookNextTokenType() == TokenType.SepComma)
            {
                //跳过逗号
                lexer.GetNextToken(out _, out _, out _);

                //解析表达式
                BaseExp exp = ParseExp(lexer);
                exps.Add(exp);
            }

            return exps.ToArray();
        }

        /// <summary>
        /// 解析表达式
        /// </summary>
        private static BaseExp ParseExp(Lexer lexer)
        {
            return ParseExp12(lexer);
        }

        /// <summary>
        /// 解析表达式12 (or)
        /// </summary>
        private static BaseExp ParseExp12(Lexer lexer)
        {
            //解析左侧表达式
            BaseExp leftExp = ParseExp11(lexer);

            while (lexer.LookNextTokenType() == TokenType.OpOr)
            {
                lexer.GetNextToken(out int line, out _, out TokenType type);

                //解析右侧表达式
                BaseExp rightExp = ParseExp11(lexer);

                //将左侧表达式和右侧表达式 合为一个新的左侧表达式 实现运算符的左结合性
                //如： a or b or c = (a or b) or c
                leftExp = new BinopExp(line, type, leftExp, rightExp);
            }

            return leftExp;
        }

        /// <summary>
        /// 解析表达式11 (and)
        /// </summary>
        private static BaseExp ParseExp11(Lexer lexer)
        {

        }

        /// <summary>
        /// 解析表达式5 (..)
        /// </summary>
        private static BaseExp ParseExp5(Lexer lexer)
        {
            BaseExp exp = ParseExp4(lexer);
            if (lexer.LookNextTokenType() != TokenType.OpConcat)
            {
                return exp;
            }

            int line = 0;
            List<BaseExp> exps = new List<BaseExp>();
            exps.Add(exp);

            while (lexer.LookNextTokenType() == TokenType.OpConcat)
            {
                //生成多叉树
                lexer.GetNextToken(out line, out _, out _);
                exp = ParseExp4(lexer);
                exps.Add(exp);
            }

            return new ConcatExp(line, exps.ToArray());
        }

        /// <summary>
        /// 解析表达式4 ( + - * / // %)
        /// </summary>
        private static BaseExp ParseExp4(Lexer lexer)
        {

        }

        /// <summary>
        /// 解析表达式2 (not # - ~)
        /// </summary>
        private static BaseExp ParseExp2(Lexer lexer)
        {
            switch (lexer.LookNextTokenType())
            {
                case TokenType.OpNot:
                case TokenType.OpLen:
                case TokenType.OpUnm:
                case TokenType.OpBNot:
                    lexer.GetNextToken(out int line, out _, out TokenType type);
                    //递归调用 实现一元运算符的右结合性
                    BaseExp rightExp = ParseExp2(lexer);
                    return new UnopExp(line, type, rightExp);
                    

            }

            return ParseExp1(lexer);
        }

        /// <summary>
        /// 解析表达式1 (^)
        /// </summary>
        private static BaseExp ParseExp1(Lexer lexer)
        {
            //解析左侧表达式
            BaseExp leftExp = ParseExp(lexer);

            if (lexer.LookNextTokenType() == TokenType.OpPow)
            {
                lexer.GetNextToken(out int line, out _, out TokenType type);

                //解析右侧表达式 递归调用 实现乘方的右结合性
                BaseExp rightExp = ParseExp1(lexer);

                leftExp = new BinopExp(line, type, leftExp, rightExp);
            }

            return leftExp;
        }

        /// <summary>
        /// 解析表达式0 (nil false true numeral LiteralString vararg functiondef tableconstructor prefixexp)
        /// </summary>
        private static BaseExp ParseExp0(Lexer lexer)
        {
            switch (lexer.LookNextTokenType())
            {
                case TokenType.KwNil:
                    lexer.GetNextToken(out int line, out _, out _);
                    return new NilExp(line);

                case TokenType.KwFalse:
                    lexer.GetNextToken(out line, out _, out _);
                    return new FalseExp(line);

                case TokenType.KwTrue:
                    lexer.GetNextToken(out line, out _, out _);
                    return new TrueExp(line);

                case TokenType.Number:
                    return ParseNumberExp(lexer);

                case TokenType.String:
                    lexer.GetNextToken(out line, out string token, out _);
                    return new StringExp(line, token);

                case TokenType.Vararg:
                    lexer.GetNextToken(out line, out _, out _);
                    return new VarargExp(line);

                case TokenType.KwFunction:
                    lexer.GetNextToken(out _, out _, out _);
                    return ParseFuncDefExp(lexer);

                case TokenType.SepLcurly:
                    return ParseTableConstructorExp(lexer);

                default:
                    return ParsePrefixExp(lexer);

            }
        }


        /// <summary>
        /// 解析逗号分隔的var列表 var包括名字表达式和表访问表达式(都是前缀表达式)
        /// </summary>
        private static BaseExp[] ParseVarList(Lexer lexer, BaseExp var0)
        {
            List<BaseExp> vars = new List<BaseExp>();
            vars.Add(CheckVar(lexer, var0));

            //解析逗号后跟着的var

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
        /// 检查exp是否为var(名字表达式或表访问表达式)
        /// </summary>
        private static BaseExp CheckVar(Lexer lexer, BaseExp exp)
        {
            if (exp is NameExp || exp is TableAccessExp)
            {
                return exp;
            }

            lexer.Error("目标exp不是var表达式");
            return null;
        }



     
    }

   
}

