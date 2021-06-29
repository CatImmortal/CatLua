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


        /// <summary>
        /// 解析函数名
        /// </summary>
        private bool ParseFuncName(Lexer lexer,out BaseExp exp)
        {
            //是否有冒号
            bool hasColon = false;  

            lexer.GetNextIdentifier(out int line, out string name);
            exp = new NameExp(line,name);

            while (lexer.LookNextTokenType() == TokenType.SepDot)
            {
                //跳过点号
                lexer.GetNextToken(out _, out _, out _);

                //获取点号后的key名 如 t.a
                lexer.GetNextIdentifier(out line, out name);

                //构造表访问表达式
                StringExp key = new StringExp(line, name);
                exp = new TableAccessExp(line, exp, key);
            }

            if (lexer.LookNextTokenType() == TokenType.SepColon)
            {
                //跳过冒号
                lexer.GetNextToken(out _, out _, out _);

                //获取冒号后访问的函数名
                lexer.GetNextIdentifier(out line, out name);

                //构造表访问表达式
                StringExp key = new StringExp(line,name);
                exp = new TableAccessExp(line, exp, key);
                hasColon = true;
            }

            return hasColon;
        }


       
    }

}
