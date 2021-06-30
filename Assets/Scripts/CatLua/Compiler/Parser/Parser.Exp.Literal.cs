namespace CatLua
{
    public partial class Parser
    {
        /// <summary>
        /// 解析数字字面量表达式
        /// </summary>
        private static BaseExp ParseNumberExp(Lexer lexer)
        {
            lexer.GetNextToken(out int line, out string token, out _);
            if (long.TryParse(token, out long l))
            {
                return new IntegerExp(line, l);
            }
            else if (double.TryParse(token, out double d))
            {
                return new FloatExp(line, d);
            }

            throw new System.Exception("ParseNumberExp方法解析失败");

        }
    }

}
