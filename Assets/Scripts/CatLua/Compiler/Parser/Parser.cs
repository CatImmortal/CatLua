using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    /// <summary>
    /// 语法解析器（递归下降解析）
    /// </summary>
    public partial class Parser
    {
        /// <summary>
        /// 解析Block
        /// </summary>
        public Block ParseBlock(Lexer lexer)
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
        private bool IsReturnOrBlockEnd(TokenType type)
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


        /// <summary>
        /// 解析逗号分隔的变量名列表
        /// </summary>
        private string[] ParseNameList(Lexer lexer,string name0)
        {
            List<string> nameList = new List<string>();
            nameList.Add(name0);

            while (lexer.LookNextTokenType() == TokenType.SepComma)
            {
                //跳过逗号
                lexer.GetNextToken(out _, out _, out _);

                //提取标识符
                lexer.GetNextIdentifier(out _, out string name);

                nameList.Add(name);
            }

            return nameList.ToArray();
        }




       
    }

}
