using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    public partial class Parser
    {
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
            lexer.GetNextToken(out int line, out string token, out TokenType type);

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
        /// 解析表达式列表
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
        /// 解析函数定义表达式
        /// </summary>
        private FuncDefExp ParseFuncDefExp(Lexer lexer)
        {

        }

        /// <summary>
        /// 解析前缀表达式
        /// </summary>
        private BaseExp ParsePrefixExp(Lexer lexer)
        {

        }

        /// <summary>
        /// 解析逗号分隔的var表达式列表
        /// </summary>
        private BaseExp[] ParseVarList(Lexer lexer, BaseExp var0)
        {
            List<BaseExp> vars = new List<BaseExp>();
            vars.Add(CheckVar(lexer, var0));

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
        private BaseExp CheckVar(Lexer lexer, BaseExp exp)
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

