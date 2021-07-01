using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    public partial class Parser
    {
        /// <summary>
        /// 解析语句序列
        /// </summary>
        private static BaseStat[] ParseStats(Lexer lexer)
        {
            List<BaseStat> stats = new List<BaseStat>();

            while (!IsReturnOrBlockEnd(lexer.LookNextTokenType()))
            {
                //下一个token 不是 return或block结束 就解析一条语句
                BaseStat stat = ParseStat(lexer);
                UnityEngine.Debug.Log("Parse结束，语句类型为" + stat.GetType());
                if (!(stat is EmptyStat))
                {
                    //不是空语句 放入列表
                    stats.Add(stat);
                }
            }

            return stats.ToArray();
        }

        /// <summary>
        /// 解析语句
        /// </summary>
        private static BaseStat ParseStat(Lexer lexer)
        {
            //前瞻一个token 根据类型调用对应的解析语句
            switch (lexer.LookNextTokenType())
            {
                case TokenType.SepSemi:
                    return ParseEmptyStat(lexer);
                case TokenType.KwBreak:
                    return ParseBreakStat(lexer);
                case TokenType.KwDo:
                    return ParseDoStat(lexer);
                case TokenType.KwWhile:
                    return ParseWhileStat(lexer);
                case TokenType.KwRepeat:
                    return ParseRepeatStat(lexer);
                case TokenType.KwIf:
                    return ParseIfStat(lexer);
                case TokenType.KwFor:
                    return ParseForStat(lexer);
                case TokenType.KwFunction:
                    return ParseFuncDefStat(lexer);
                case TokenType.KwLocal:
                    return ParseLocalStat(lexer);
                default:
                    return ParseAssignOrFuncCallStat(lexer);
            }
        }



        /// <summary>
        /// 解析逗号分隔的名字列表
        /// </summary>
        private static string[] ParseNameList(Lexer lexer, string name0)
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

