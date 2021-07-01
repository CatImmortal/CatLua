namespace CatLua
{
    public partial class Parser
    {
        /// <summary>
        /// 解析空语句
        /// </summary>>
        private static EmptyStat ParseEmptyStat(Lexer lexer)
        {
            //空语句直接跳过分号
            lexer.GetNextTokenOfType(TokenType.SepSemi, out _, out _);
            return new EmptyStat();
        }

        /// <summary>
        /// 解析do语句
        /// </summary>>
        private static DoStat ParseDoStat(Lexer lexer)
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
        /// 解析赋值语句或函数调用语句
        /// </summary>
        private static BaseStat ParseAssignOrFuncCallStat(Lexer lexer)
        {
            //赋值语句和函数调用语句都以前缀表达式开始

            //先解析前缀表达式
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
    }

}
