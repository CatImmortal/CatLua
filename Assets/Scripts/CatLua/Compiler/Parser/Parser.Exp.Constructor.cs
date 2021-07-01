using System.Collections.Generic;

namespace CatLua
{
    public partial class Parser
    {
        /// <summary>
        /// 解析函数定义表达式
        /// </summary>
        private static FuncDefExp ParseFuncDefExp(Lexer lexer)
        {
            //关键字function已经跳过

            int line = lexer.Line;

            //跳过左括号
            lexer.GetNextTokenOfType(TokenType.SepLparen, out _, out _);

            //解析参数列表
            bool isVararg = ParseFuncParamList(lexer, out string[] paramList);

            //跳过右括号
            lexer.GetNextTokenOfType(TokenType.SepRparen, out _, out _);

            //解析函数体
            Block block = ParseBlock(lexer);

            //跳过end
            lexer.GetNextTokenOfType(TokenType.KwEnd, out int lastLine, out _);

            return new FuncDefExp(line, lastLine, paramList, isVararg, block);
        }

        /// <summary>
        /// 解析函数参数列表
        /// </summary>
        private static bool ParseFuncParamList(Lexer lexer, out string[] paramList)
        {
            paramList = null;
          

            //左括号后接的是 右括号或...
            //直接返回了
            switch (lexer.LookNextTokenType())
            {
                case TokenType.SepRparen:
                    return false;

                case TokenType.Vararg:
                    //跳过...
                    lexer.GetNextToken(out _, out _, out _);
                    return true;
            }

            bool isVararg = false;
            List<string> list = new List<string>();

            //解析第一个参数的变量名
            lexer.GetNextIdentifier(out _, out string name);
            list.Add(name);

            //解析逗号后跟着的其他变量名
            while (lexer.LookNextTokenType() == TokenType.SepComma)
            {
                //跳过逗号
                lexer.GetNextToken(out _, out _, out _);

                if (lexer.LookNextTokenType() == TokenType.Identifier)
                {
                    //解析变量名
                    lexer.GetNextIdentifier(out _, out name);
                    list.Add(name);
                }
                else
                {
                    //跳过vararg
                    lexer.GetNextTokenOfType(TokenType.Vararg, out _, out _);
                    isVararg = true;

                    //vararg只能放在参数末尾 所以break了
                    break;
                }
            }

            paramList = list.ToArray();
            return isVararg;

        }

        /// <summary>
        /// 解析表构造器表达式
        /// </summary>
        private static BaseExp ParseTableConstructorExp(Lexer lexer)
        {
            int line = lexer.Line;

            //跳过 {
            lexer.GetNextTokenOfType(TokenType.SepLcurly, out _, out _);

            //解析key value列表
            ParseKeyValueList(lexer, out BaseExp[] keys, out BaseExp[] values);

            //跳过 }
            lexer.GetNextTokenOfType(TokenType.SepRcurly, out _, out _);

            int lastLine = lexer.Line;

            return new TableConstructorExp(line, lastLine, keys, values);
        }

        /// <summary>
        /// 解析表构造器中的key value列表
        /// </summary>
        private static void ParseKeyValueList(Lexer lexer, out BaseExp[] keys, out BaseExp[] values)
        {
            List<BaseExp> keyList = new List<BaseExp>();
            List<BaseExp> valueList = new List<BaseExp>();

            if (lexer.LookNextTokenType() != TokenType.SepRcurly)
            {
                //{ 后接的不是 }
                //继续解析key value
                ParseKeyValue(lexer, out BaseExp key, out BaseExp value);
                keyList.Add(key);
                valueList.Add(value);

                while (IsKeyValueSep(lexer.LookNextTokenType()))
                {
                    //处理分隔符以及后面的key value

                    //跳过分隔符
                    lexer.GetNextToken(out _, out _, out _);

                    if (lexer.LookNextTokenType() != TokenType.SepRcurly)
                    {
                        //没遇到 }

                        //解析key value
                        ParseKeyValue(lexer, out key, out value);
                        keyList.Add(key);
                        valueList.Add(value);
                    }
                    else
                    {
                        //分隔符后是 { 结束循环
                        break;
                    }
                }
            }

            keys = keyList.ToArray();
            values = valueList.ToArray();
        }

        /// <summary>
        /// 解析表构造器中的key value
        /// </summary>
        private static void ParseKeyValue(Lexer lexer, out BaseExp key, out BaseExp value)
        {


            if (lexer.LookNextTokenType() == TokenType.SepLbrack)
            {
                //[key] = value的形式

                //跳过[
                lexer.GetNextToken(out _, out _, out _);

                //解析key的表达式
                key = ParseExp(lexer);

                //跳过]和=
                lexer.GetNextTokenOfType(TokenType.SepRbrack, out _, out _);
                lexer.GetNextTokenOfType(TokenType.OpAsssign, out _, out _);

                //解析value的表达式
                value = ParseExp(lexer);
                return;
            }

            //key = value的形式

            BaseExp exp = ParseExp(lexer);

            key = null;
            value = exp;

            if (exp is NameExp nameExp)
            {
                if (lexer.LookNextTokenType() == TokenType.OpAsssign)
                {
                    //跳过=
                    lexer.GetNextToken(out _, out _, out _);

                    //将key从NameExp转换为StringExp
                    //key = value 等于 ["key"] = value
                    key = new StringExp(nameExp.Line, nameExp.Name);

                    //解析value的表达式
                    value = ParseExp(lexer);
                }
            }
        }

        /// <summary>
        /// 是否是表构造器key value的分隔符
        /// </summary>
        private static bool IsKeyValueSep(TokenType type)
        {
            //逗号或分号都是分隔符
            return type == TokenType.SepComma || type == TokenType.SepSemi;
        }
    }

}
