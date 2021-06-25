using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CatLua
{
    /// <summary>
    /// 词法分析器
    /// </summary>
    public class Lexer
    {
        public Lexer(string chunk, string chunkName)
        {
            this.chunk = chunk;
            this.chunkName = chunkName;
            line = 1;
            curIndex = 0;
            ReOpeningLongBracket = new Regex(@"^\[=*\[", RegexOptions.Compiled);
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
        private string chunk;

        /// <summary>
        /// 源文件名
        /// </summary>
        private string chunkName;

        /// <summary>
        /// 当前行号
        /// </summary>
        private int line;

        /// <summary>
        /// 当前索引
        /// </summary>
        private int curIndex;

        private Regex ReOpeningLongBracket;

        /// <summary>
        /// 跳过n个字符
        /// </summary>
        private void Next(int n)
        {
            curIndex += n;
        }

        /// <summary>
        /// 剩余的源代码是否以s开头
        /// </summary>
        private bool Test(string s)
        {
            return chunk.StartsWith(s);
        }

        /// <summary>
        /// 返回下一个token，对应行号和token类型
        /// </summary>
        private void NextToken(out int line, out TokenType kind, out string token)
        {
            SkipWhiteSpaces();

            if (curIndex >= chunk.Length)
            {
                line = this.line;
                kind = TokenType.Eof;
                token = "Eof";
                return;
            }

            switch (chunk[curIndex])
            {
                case ';':
                    Next(1);
                    kind = TokenType.SepSemi;
                    token = ";";
                    break;

                case ',':
                    Next(1);
                    kind = TokenType.SepComma;
                    token = ";";
                    break;

                case '(':
                    Next(1);
                    kind = TokenType.SepLparen;
                    token = "(";
                    break;

                case ')':
                    Next(1);
                    kind = TokenType.SepRparen;
                    token = ")";
                    break;

                case ']':
                    Next(1);
                    kind = TokenType.SepRbrack;
                    token = "]";
                    break;

                case '{':
                    Next(1);
                    kind = TokenType.SepLcurly;
                    token = "{";
                    break;

                case '}':
                    Next(1);
                    kind = TokenType.SepRcurly;
                    token = "}";
                    break;

                case '+':
                    Next(1);
                    kind = TokenType.OpAdd;
                    token = "+";
                    break;

                case '-':
                    Next(1);
                    kind = TokenType.OpMinus;
                    token = "-";
                    break;
                case '*':
                    Next(1);
                    kind = TokenType.OpMul;
                    token = "*";
                    break;
                case '^':
                    Next(1);
                    kind = TokenType.OpPow;
                    token = "^";
                    break;
                case '%':
                    Next(1);
                    kind = TokenType.OpMod;
                    token = "&";
                    break;
                case '&':
                    Next(1);
                    kind = TokenType.OpBAnd;
                    token = "&";
                    break;
                case '|':
                    Next(1);
                    kind = TokenType.OpBOr;
                    token = "|";
                    break;
                case '#':
                    Next(1);
                    kind = TokenType.OpLen;
                    token = "#";
                    break;

                case ':':
                    if (Test("::"))
                    {
                        Next(2);
                        kind = TokenType.SepLable;
                        token = "::";
                    }
                    else
                    {
                        Next(1);
                        kind = TokenType.SepColon;
                        token = ":";
                    }
                    break;

                case '/':
                    if (Test("//"))
                    {
                        Next(2);
                        kind = TokenType.OpIDiv;
                        token = "//";
                    }
                    else
                    {
                        Next(1);
                        kind = TokenType.OpDiv;
                        token = "/";
                    }
                    break;

                case '~':
                    if (Test("~="))
                    {
                        Next(2);
                        kind = TokenType.OpNe;
                        token = "~=";
                    }
                    else
                    {
                        Next(1);
                        kind = TokenType.OpWave;
                        token = "~";
                    }
                    break;

                case '=':
                    if (Test("=="))
                    {
                        Next(2);
                        kind = TokenType.OpEq;
                        token = "==";
                    }
                    else
                    {
                        Next(1);
                        kind = TokenType.OpAsssign;
                        token = "=";
                    }
                    break;

                case '<':
                    if (Test("<<"))
                    {
                        Next(2);
                        kind = TokenType.OpShL;
                        token = "<<";
                    }
                    else if (Test("<="))
                    {
                        Next(2);
                        kind = TokenType.OpLe;
                        token = "<=";
                    }
                    else
                    {
                        Next(1);
                        kind = TokenType.OpDiv;
                        token = "<";
                    }
                    break;

                case '>':
                    if (Test(">>"))
                    {
                        Next(2);
                        kind = TokenType.OpShR;
                        token = ">>";
                    }
                    else if (Test("<="))
                    {
                        Next(2);
                        kind = TokenType.OpGe;
                        token = ">=";
                    }
                    else
                    {
                        Next(1);
                        kind = TokenType.OpGt;
                        token = ">";
                    }
                    break;

                case '.':
                    if (Test("..."))
                    {
                        Next(3);
                        kind = TokenType.Vararg;
                        token = "...";
                    }
                    else if (Test(".."))
                    {
                        Next(2);
                        kind = TokenType.OpConcat;
                        token = "..";
                    }
                    else if (curIndex == chunk.Length - 1 || !char.IsDigit(chunk[curIndex + 1]))
                    {
                        Next(1);
                        kind = TokenType.SepDot;
                        token = ".";
                    }
                    break;

                case '[':
                    if (Test("[[") || Test("[="))
                    {
                        

                        kind = TokenType.String;
                        token = "";//todo 扫描长字符串
                    }
                   
                    else
                    {
                        Next(1);
                        kind = TokenType.SepLbrack;
                        token = "[";
                    }
                    break;

                case '\\':
                case '"':
                    Next(1);
                    kind = TokenType.String;
                    token = ""; //todo 扫描短字符串
                    break;
            }
        }

        /// <summary>
        /// 跳过空白字符，回车，换行与注释
        /// </summary>
        private void SkipWhiteSpaces()
        {
            while (curIndex < chunk.Length)
            {
                if (Test("--"))
                {
                    //跳过注释
                    SkipComment();
                    continue;
                }

                if (Test("\r\n") || Test("\n\r"))
                {
                    //跳过回车与换行
                    Next(2);
                    line++;
                    continue;
                }

                if (IsNewLine(chunk[curIndex]))
                {
                    //跳过回车或换行
                    Next(1);
                    line++;
                    continue;
                }

                if (IsWhiteSpace(chunk[curIndex]))
                {
                    //跳过空白字符
                    Next(1);
                    continue;
                }

                return;
            }
        }


        /// <summary>
        /// 是否为空白字符
        /// </summary>
        private bool IsWhiteSpace(char c)
        {

            switch (c)
            {
                case '\t':
                case '\n':
                case '\v':
                case '\f':
                case '\r':
                case ' ':
                    return true;

            }

            return false;
        }

        /// <summary>
        /// 是否是回车或换行符
        /// </summary>
        private bool IsNewLine(char c)
        {
            return c == '\r' || c == '\n';
        }

        private  void SkipComment()
        {
            Next(2); //跳过--

            //长注释检查
            if (Test("["))
            {
                //扫描长字符串

                if (ReOpeningLongBracket.IsMatch(chunk.Substring(curIndex)))
                {
                    //提取长字符串扔掉
                    scanShortString();
                    return;
                }
               
            }

            //短注释检查
            while (curIndex < chunk.Length && !IsNewLine(chunk[curIndex]))
            {
                //跳过短注释字符 直到遇到新行
                Next(1);
            }
        }

        /// <summary>
        /// 扫描长字符串
        /// </summary>
        /// <returns></returns>
        private string ScanLongString()
        {
            Match m = ReOpeningLongBracket.Match(chunk.Substring(curIndex));
            if (m.Value == string.Empty)
            {
                Error("长字符串提取失败");
            }


        }

        private string scanShortString()
        {

        }

        private void Error(string err)
        {
            err = $"{chunkName}:{line}: {err}";
            throw new System.Exception(err);
        }


    }
}

