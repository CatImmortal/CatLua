namespace CatLua
{
    public partial class Parser
    {
        /// <summary>
        /// 解析前缀表达式（可以作为函数调用表达式，表访问表达式，记录访问表达式的前缀）
        /// 可能返回名字表达式，圆括号表达式，表访问表达式，函数调用表达式
        /// </summary>
        private static BaseExp ParsePrefixExp(Lexer lexer)
        {
            BaseExp exp;

            //前缀表达式只能以标识符或 ( 开始
            //先前瞻一个token
            if (lexer.LookNextTokenType() == TokenType.Identifier)
            {
                //名字表达式
                lexer.GetNextIdentifier(out int line, out string name);
                exp = new NameExp(line, name);
            }
            else
            {

                //圆括号表达式 (exp)

                exp = ParseParensExp(lexer);
            }

            //后续解析

            while (true)
            {
                switch (lexer.LookNextTokenType())
                {
                    case TokenType.SepLbrack:

                        //解析表访问表达式

                        //跳过 [
                        lexer.GetNextToken(out _, out _, out _);

                        //解析key表达式
                        BaseExp keyExp = ParseExp(lexer);

                        //跳过 [
                        lexer.GetNextTokenOfType(TokenType.SepRbrack, out _, out _);
                        exp = new TableAccessExp(lexer.Line, exp, keyExp);

                        break;

                    case TokenType.SepDot:

                        //解析记录访问表达式

                        //跳过 .
                        lexer.GetNextToken(out _, out _, out _);

                        //解析点后面的key
                        lexer.GetNextIdentifier(out int line, out string name);
                        keyExp = new StringExp(line, name);

                        //记录访问表达式是表访问表达式的语法糖
                        exp = new TableAccessExp(line, exp, keyExp);

                        break;

                    //以下4种都是函数调用表达式
                    case TokenType.SepColon:    //:  用冒号调用函数的情况
                    case TokenType.SepLparen:   //(  用括号把参数括起来的情况
                    case TokenType.SepLcurly:   //{  不用括号 直接用表构造器作为参数的情况
                    case TokenType.String:  //string  不用括号 直接用字符串字面量作为参数的情况
                        
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
                //圆括号里的表达式是vararg 函数调用 var表达式(名字和表访问)时，需要保留圆括号
                //以圆括号表达式的形式返回
                return new ParensExp(exp);
            }

            //其他的丢弃圆括号，直接返回表达式即可
            return exp;
        }

        /// <summary>
        /// 解析函数调用表达式
        /// </summary>
        private static FuncCallExp ParseFuncCallExp(Lexer lexer, BaseExp prefixExp)
        {
            //尝试解析可选的要调用的函数名字 处理掉冒号调用的情况
            //如果有冒号 name就是函数名
            //没有冒号的话name就是null
            StringExp name = ParseFuncCallNameExp(lexer);

            int line = lexer.Line;

            //解析参数 会处理有圆括号和没圆括号的情况
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

                        //解析参数的表达式列表
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
                    //只有一个字符串字面量参数的函数调用 不需要用圆括号把参数包起来
                    //func "a"
                    lexer.GetNextTokenOfType(TokenType.String, out int line, out string str);
                    args = new BaseExp[] { new StringExp(line, str) };
                    break;

            }
        }
    }

}
