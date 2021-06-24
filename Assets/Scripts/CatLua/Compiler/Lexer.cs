using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// 词法分析器
    /// </summary>
    public class Lexer
    {
        public Lexer(string chunk, string chunkName)
        {
            Chunk = chunk;
            ChunkName = chunkName;
            Line = 1;
        }

        public static Dictionary<string, TokenType> TokenMap = new Dictionary<string, TokenType>
        {
            { "and",TokenType.OpAnd},
            { "break",TokenType.KwBreak},
            { "do",TokenType.KwDo},
            { "else",TokenType.KwElse},
            { "elseif",TokenType.KwElseif},
            { "end",TokenType.KwEnd},
            { "false",TokenType.KwFalse},
            { "for",TokenType.KwFor},
            { "function",TokenType.KwFunction},
            { "goto",TokenType.KwGoto},
            { "if",TokenType.KwIf},
            { "in",TokenType.KwIn},
            { "local",TokenType.KwLocal},
            { "nil",TokenType.KwNil},
            { "not",TokenType.OpNot},
            { "or",TokenType.OpOr},
            { "repeat",TokenType.KwRepeat},
            { "return",TokenType.KwReturn},
            { "then",TokenType.KwThen},
            { "true",TokenType.KwTrue},
            { "until",TokenType.KwUntil},
            { "while",TokenType.KwWhile},
        };

        /// <summary>
        /// 源代码
        /// </summary>
        public string Chunk;

        /// <summary>
        /// 源文件名
        /// </summary>
        public string ChunkName;

        /// <summary>
        /// 当前行号
        /// </summary>
        public int Line;

      

        /// <summary>
        /// 返回下一个token（会跳过空白字符和注释）
        /// </summary>
        public void NextToken(int kind,int line,string token)
        {

        }
    }
}

