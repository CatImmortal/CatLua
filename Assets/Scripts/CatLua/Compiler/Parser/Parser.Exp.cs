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
                //case TokenType.KwEnd:
                //case TokenType.KwElse:
                //case TokenType.KwElseif:
                //case TokenType.KwUntil:
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
                        //解析完所有表达式后 有分号要跳过分号
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

            //解析表达式
            BaseExp exp = ParseExp(lexer);
            exps.Add(exp);

            while (lexer.LookNextTokenType() == TokenType.SepComma)
            {
                //跳过逗号
                lexer.GetNextToken(out _, out _, out _);

                //解析表达式
                exp = ParseExp(lexer);
                exps.Add(exp);
            }

            return exps.ToArray();
        }

        /// <summary>
        /// 解析表达式
        /// </summary>
        private static BaseExp ParseExp(Lexer lexer)
        {
            return ParseExpByPriority(lexer, 12);
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

