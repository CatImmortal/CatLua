namespace CatLua
{
    public partial class Parser
    {
        /// <summary>
        /// 解析前缀表达式
        /// </summary>
        private static BaseExp ParsePrefixExp(Lexer lexer)
        {
            BaseExp exp;

            //前缀表达式只能以标识符或 ( 开始
            if (lexer.LookNextTokenType() == TokenType.Identifier)
            {
                lexer.GetNextIdentifier(out int line, out string name);
                exp = new NameExp(line, name);
            }
            else
            {
                //(exp)

                exp = ParseParensExp(lexer);
            }

            while (true)
            {
                switch (lexer.LookNextTokenType())
                {
                    case TokenType.SepLbrack:
                        //跳过 [
                        lexer.GetNextToken(out _, out _, out _);

                        //解析key表达式
                        BaseExp keyExp = ParseExp(lexer);

                        //跳过 [
                        lexer.GetNextTokenOfType(TokenType.SepRbrack, out _, out _);
                        exp = new TableAccessExp(lexer.Line, exp, keyExp);

                        break;

                    case TokenType.SepDot:
                        //跳过 .
                        lexer.GetNextToken(out _, out _, out _);

                        //解析点后面的key
                        lexer.GetNextIdentifier(out int line, out string name);
                        keyExp = new StringExp(line, name);

                        exp = new TableAccessExp(line, exp, keyExp);

                        break;

                    case TokenType.SepColon:
                    case TokenType.SepLparen:
                    case TokenType.SepLcurly:
                    case TokenType.String:
                        //: ( { string
                        //解析为函数调用表达式
                        exp = ParseFuncCallExp(lexer, exp);
                        break;

                    default:
                        return exp;

                }

                return exp;
            }
        }

        /// <summary>
        /// 解析圆括号表达式
        /// </summary>
        private static BaseExp ParseParensExp(Lexer lexer)
        {
            //跳过 (
            lexer.GetNextTokenOfType(TokenType.SepLparen, out _, out _);

            //解析括号里的表达式
            BaseExp exp = ParseExp(lexer);

            //跳过 )
            lexer.GetNextTokenOfType(TokenType.SepRparen, out _, out _);

            if (exp is VarargExp || exp is FuncCallExp || exp is NameExp || exp is TableAccessExp)
            {
                //圆括号里的表达式是vararg 函数调用 和var表达式(名字和表访问)时，需要保留圆括号
                return new ParensExp(exp);
            }

            return exp;
        }

        /// <summary>
        /// 解析函数调用表达式
        /// </summary>
        private static FuncCallExp ParseFuncCallExp(Lexer lexer, BaseExp prefixExp)
        {
            //解析要调用的函数名字
            StringExp name = ParseFuncCallNameExp(lexer);

            int line = lexer.Line;

            //解析参数
            ParseArgs(lexer, out BaseExp[] args);

            int lastLine = lexer.Line;

            return new FuncCallExp(line, lastLine, prefixExp, name, args);

        }

        /// <summary>
        /// 解析函数调用时可选的名字表达式  如t:f()
        /// </summary>
        private static StringExp ParseFuncCallNameExp(Lexer lexer)
        {
            if (lexer.LookNextTokenType() == TokenType.SepColon)
            {
                //跳过 :
                lexer.GetNextToken(out _, out _, out _);

                lexer.GetNextIdentifier(out int line, out string name);
                return new StringExp(line, name);
            }
            return null;
        }

        /// <summary>
        /// 解析函数调用时的参数列表
        /// </summary>
        private static void ParseArgs(Lexer lexer, out BaseExp[] args)
        {
            args = null;

            switch (lexer.LookNextTokenType())
            {
                case TokenType.SepLparen:

                    //跳过 (
                    lexer.GetNextToken(out _, out _, out _);
                    if (lexer.LookNextTokenType() != TokenType.SepRparen)
                    {
                        //接下来不是 )

                        //解析参数
                        args = ParseExpList(lexer);
                    }
                    //跳过 )
                    lexer.GetNextTokenOfType(TokenType.SepRparen, out _, out _);
                    break;

                case TokenType.SepLcurly:

                    //只有一个表构造器参数的函数调用 不需要用圆括号把参数包起来
                    //func {a,b,c}
                    args = new BaseExp[] { ParseTableConstructorExp(lexer) };
                    break;

                default:
                    //只有一个字符串参数的函数调用 不需要用圆括号把参数包起来
                    //func "a"
                    lexer.GetNextTokenOfType(TokenType.String, out int line, out string str);
                    args = new BaseExp[] { new StringExp(line, str) };
                    break;

            }
        }
    }

}
