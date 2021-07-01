using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// 语法解析器（递归下降解析）
    /// </summary>
    public static partial class Parser
    {
        /// <summary>
        /// 解析chunk
        /// </summary>
        public static Block Parse(string chunk,string chunkName)
        {
            Lexer lexer = new Lexer(chunk,chunkName);
            Block block = ParseBlock(lexer);
            lexer.GetNextTokenOfType(TokenType.Eof, out _, out _);
            return block;
        }

        /// <summary>
        /// 解析Block
        /// </summary>
        private static Block ParseBlock(Lexer lexer)
        {
            Block block = new Block();
            block.LastLine = lexer.Line;
            block.Stats = ParseStats(lexer);
            block.ReturnExps = ParseReturnExps(lexer);

            return block;
        }

        /// <summary>
        /// 是否为Return或Block结束
        /// </summary>
        private static bool IsReturnOrBlockEnd(TokenType type)
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





   


    }

}
