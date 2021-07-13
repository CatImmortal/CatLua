using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

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
            Line = 1;
            curIndex = 0;
        }

        public static Dictionary<string, TokenType> KeyWordTokenMap = new Dictionary<string, TokenType>
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
        /// 匹配十进制整数或浮点数的正则表达式
        /// </summary>
        private static Regex numberRegex = new Regex(@"^-?\d+$|^(-?\d+)(\.\d+)?", RegexOptions.Compiled);

        /// <summary>
        /// 匹配标识符和关键字的正则表达式
        /// </summary>
        private static Regex identifierRegex = new Regex(@"^[_\d\w]+", RegexOptions.Compiled);

        /// <summary>
        /// 匹配短字符串的正则表达式（不支持转义字符
        /// </summary>
        private static Regex shortStrRegex = new Regex("^\"[^\"]*\"", RegexOptions.Compiled);

        /// <summary>
        /// 源代码
        /// </summary>
        private string chunk;

        /// <summary>
        /// 源文件名
        /// </summary>
        private string chunkName;

        /// <summary>
        /// 当前索引
        /// </summary>
        private int curIndex;



        private string nextToken;
        private TokenType nextTokenType;
        private int nextTokenLine;

        /// <summary>
        /// 当前行号
        /// </summary>
        public int Line
        {
            get;
            private set;
        }

        public void Error(string err)
        {
            err = $"{chunkName}:{Line}: {err}";
            throw new System.Exception(err);
        }


        /// <summary>
        /// 查看下一个token的类型
        /// </summary>
        public TokenType LookNextTokenType()
        {
            if (nextTokenLine > 0)
            {
                //已有下一个token的信息了
                return nextTokenType;
            }

            //记录行号
            int curLine = this.Line;

            GetNextToken(out int line, out string token, out TokenType type);

            //还原行号
            this.Line = curLine;

            //下一个token的缓存信息
            nextTokenLine = line;
            nextTokenType = type;
            nextToken = token;
            return type;


        }

        /// <summary>
        /// 提取下一个标识符
        /// </summary>
        public void GetNextIdentifier(out int line,out string token)
        {
            GetNextTokenOfType(TokenType.Identifier, out line, out token);
        }


        /// <summary>
        /// 获取指定类型的下一个token
        /// </summary>
        public void GetNextTokenOfType(TokenType type,out int line,out string token)
        {
            GetNextToken(out  line, out token, out TokenType typeResult);
            if (type != typeResult)
            {
                Error($"NextTokenOfType调用失败，需求{type}但提取到的是{token}");
            }

        }

        /// <summary>
        /// 返回下一个token，对应行号和token类型，并将其跳过
        /// </summary>
        public void GetNextToken(out int line, out string token, out TokenType type)
        {
            if (nextTokenLine > 0)
            {
                //有缓存下一个token的信息 直接返回
                line = nextTokenLine;
                token = nextToken;
                type = nextTokenType;

                this.Line = nextTokenLine;

                //重置
                nextTokenLine = 0;
                return;
            }

            //跳过空白字符，回车，换行与注释
            SkipWhiteSpaces();

            line = Line;

            token = default;
            type = default;

            if (curIndex >= chunk.Length)
            {
                type = TokenType.Eof;
                token = "Eof";
                return;
            }

            char c = chunk[curIndex];

            //分隔符,运算符和字符串
            switch (c)
            {
                case ';':
                    Next(1);
                    type = TokenType.SepSemi;
                    token = ";";
                    return;

                case ',':
                    Next(1);
                    type = TokenType.SepComma;
                    token = ",";
                    return;

                case '(':
                    Next(1);
                    type = TokenType.SepLparen;
                    token = "(";
                    return;

                case ')':
                    Next(1);
                    type = TokenType.SepRparen;
                    token = ")";
                    return;

                case ']':
                    Next(1);
                    type = TokenType.SepRbrack;
                    token = "]";
                    return;

                case '{':
                    Next(1);
                    type = TokenType.SepLcurly;
                    token = "{";
                    return;

                case '}':
                    Next(1);
                    type = TokenType.SepRcurly;
                    token = "}";
                    return;

                case '+':
                    Next(1);
                    type = TokenType.OpAdd;
                    token = "+";
                    return;

                case '-':
                    Next(1);
                    type = TokenType.OpMinus;
                    token = "-";
                    return;
                case '*':
                    Next(1);
                    type = TokenType.OpMul;
                    token = "*";
                    return;
                case '^':
                    Next(1);
                    type = TokenType.OpPow;
                    token = "^";
                    return;
                case '%':
                    Next(1);
                    type = TokenType.OpMod;
                    token = "&";
                    return;
                case '&':
                    Next(1);
                    type = TokenType.OpBAnd;
                    token = "&";
                    return;
                case '|':
                    Next(1);
                    type = TokenType.OpBOr;
                    token = "|";
                    return;
                case '#':
                    Next(1);
                    type = TokenType.OpLen;
                    token = "#";
                    return;

                case ':':
                    if (Test("::"))
                    {
                        Next(2);
                        type = TokenType.SepLable;
                        token = "::";
                    }
                    else
                    {
                        Next(1);
                        type = TokenType.SepColon;
                        token = ":";
                    }
                    return;

                case '/':
                    if (Test("//"))
                    {
                        Next(2);
                        type = TokenType.OpIDiv;
                        token = "//";
                    }
                    else
                    {
                        Next(1);
                        type = TokenType.OpDiv;
                        token = "/";
                    }
                    return;

                case '~':
                    if (Test("~="))
                    {
                        Next(2);
                        type = TokenType.OpNe;
                        token = "~=";
                    }
                    else
                    {
                        Next(1);
                        type = TokenType.OpWave;
                        token = "~";
                    }
                    return;

                case '=':
                    if (Test("=="))
                    {
                        Next(2);
                        type = TokenType.OpEq;
                        token = "==";
                    }
                    else
                    {
                        Next(1);
                        type = TokenType.OpAsssign;
                        token = "=";
                    }
                    return;

                case '<':
                    if (Test("<<"))
                    {
                        Next(2);
                        type = TokenType.OpShL;
                        token = "<<";
                    }
                    else if (Test("<="))
                    {
                        Next(2);
                        type = TokenType.OpLe;
                        token = "<=";
                    }
                    else
                    {
                        Next(1);
                        type = TokenType.OpDiv;
                        token = "<";
                    }
                    return;

                case '>':
                    if (Test(">>"))
                    {
                        Next(2);
                        type = TokenType.OpShR;
                        token = ">>";
                    }
                    else if (Test("<="))
                    {
                        Next(2);
                        type = TokenType.OpGe;
                        token = ">=";
                    }
                    else
                    {
                        Next(1);
                        type = TokenType.OpGt;
                        token = ">";
                    }
                    return;

                case '.':
                    if (Test("..."))
                    {
                        Next(3);
                        type = TokenType.Vararg;
                        token = "...";
                        return;
                    }
                    else if (Test(".."))
                    {
                        Next(2);
                        type = TokenType.OpConcat;
                        token = "..";
                        return;
                    }
                    else if (curIndex == chunk.Length - 1 || !char.IsDigit(chunk[curIndex + 1]))
                    {
                        Next(1);
                        type = TokenType.SepDot;
                        token = ".";
                        return;
                    }
                    break;
                case '[':
                    if (Test("[[") || Test("[="))
                    {
                        type = TokenType.String;
                        token = ScanLongString();//todo 扫描长字符串
                        return;
                    }
                    else
                    {
                        Next(1);
                        type = TokenType.SepLbrack;
                        token = "[";
                        return;
                    }

                case '\'':  //单引号
                case '"':  //双引号
                    Next(1);
                    type = TokenType.String;
                    token = ScanShortString(); //todo 扫描短字符串
                    return;
            }

            //数字字面量
            if (c == '.' || char.IsDigit(c))
            {
                token = ScanNumber();
                type = TokenType.Number;
                return;
            }

            //标识符或关键字
            if (c == '_' || char.IsLetter(c))
            {
                token = ScanIdentifier();
                if (KeyWordTokenMap.TryGetValue(token,out type))
                {
                    //关键字
                    return;
                }

                //标识符
                type = TokenType.Identifier;
                return;
            }

            Error("语法错误，当前字符为:" + c);
        }

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
            return chunk.Substring(curIndex).StartsWith(s);
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
                    Line++;
                    continue;
                }

                if (IsNewLine(chunk[curIndex]))
                {
                    //跳过回车或换行
                    Next(1);
                    Line++;
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

        /// <summary>
        /// 跳过注释
        /// </summary>
        private  void SkipComment()
        {
            Next(2); //跳过--

            //长注释检查 长注释就是--跟一个长字符串
            if (Test("["))
            {
                //扫描长字符串
                //todo:
                
               
            }

            //短注释检查
            while (curIndex < chunk.Length && !IsNewLine(chunk[curIndex]))
            {
                //不断跳过--后的注释字符 直到遇到新行
                Next(1);
            }
        }

        /// <summary>
        /// 扫描长字符串
        /// </summary>
        private string ScanLongString()
        {
            //todo
            return string.Empty;
            
        }

        /// <summary>
        /// 扫描短字符串
        /// </summary>
        private string ScanShortString()
        {
            string matchResult = shortStrRegex.Match(chunk.Substring(curIndex-1)).Value;
            if (string.IsNullOrEmpty(matchResult))
            {
                Error("提取短字符串失败");
            }

            Next(matchResult.Length - 1);
            string result = matchResult.Substring(1, matchResult.Length - 2);

            return result;
        }

        /// <summary>
        /// 扫描数字字面量
        /// </summary>
        private string ScanNumber()
        {
            return Scan(numberRegex);
        }

        /// <summary>
        /// 扫描标识符和关键字
        /// </summary>
        private string ScanIdentifier()
        {
            return Scan(identifierRegex);
        }

        /// <summary>
        /// 使用指定的正则表达式，从剩余源代码中进行匹配，返回第一个匹配结果
        /// </summary>
        private string Scan(Regex regex)
        {
            string matchResult = regex.Match(chunk.Substring(curIndex)).Value;
            if (string.IsNullOrEmpty(matchResult))
            {
                Error("Scan扫描失败");
            }

            Next(matchResult.Length);
            return matchResult;
        }

        public override string ToString()
        {
            return chunk.Substring(curIndex);
        }
    }
}

