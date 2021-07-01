using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    public partial class Parser
    {
        /// <summary>
        /// 解析赋值语句
        /// </summary>
        private static AssignStat ParseAssignStat(Lexer lexer, BaseExp var0)
        {
            //解析赋值号左侧的var列表
            BaseExp[] varList = ParseVarList(lexer, var0);

            //跳过赋值符号
            lexer.GetNextTokenOfType(TokenType.OpAsssign, out _, out _);

            //解析赋值号右侧的表达式列表
            BaseExp[] expList = ParseExpList(lexer);

            return new AssignStat(lexer.Line, varList, expList);
        }

        /// <summary>
        /// 解析局部函数定义语句或局部变量声明语句
        /// </summary>
        private static BaseStat ParseLocalStat(Lexer lexer)
        {
            //跳过local
            lexer.GetNextTokenOfType(TokenType.KwLocal, out _, out _);

            //前瞻1个token
            if (lexer.LookNextTokenType() == TokenType.KwFunction)
            {
                //local后接function 是局部函数定义
                return ParseLocalFuncDefStat(lexer);
            }
            else
            {
                //否则是局部变量声明
                return ParseLocalVarDeclStat(lexer);
            }
        }

        /// <summary>
        /// 解析局部函数定义语句
        /// </summary>
        private static LocalFuncDefStat ParseLocalFuncDefStat(Lexer lexer)
        {
            //跳过function
            lexer.GetNextTokenOfType(TokenType.KwFunction, out _, out _);

            //解析函数名
            lexer.GetNextIdentifier(out _, out string name);

            //解析函数定义表达式
            FuncDefExp exp = ParseFuncDefExp(lexer);

            return new LocalFuncDefStat(name, exp);
        }

        /// <summary>
        /// 解析局部变量声明语句
        /// </summary>
        private static LocalVarDeclStat ParseLocalVarDeclStat(Lexer lexer)
        {
            //提取一个变量名
            lexer.GetNextIdentifier(out _, out string name0);

            //解析逗号后跟着的其他变量名
            string[] nameList = ParseNameList(lexer, name0);

            BaseExp[] expList = null;
            if (lexer.LookNextTokenType() == TokenType.OpAsssign)
            {
                //有赋值符号

                //跳过赋值符号
                lexer.GetNextToken(out _, out _, out _);

                //解析赋值符号后面的表达式
                expList = ParseExpList(lexer);
            }

            return new LocalVarDeclStat(lexer.Line, nameList, expList);
        }

        /// <summary>
        /// 解析非局部函数定义语句（赋值语句的语法糖）
        /// </summary>
        private static AssignStat ParseFuncDefStat(Lexer lexer)
        {
            //跳过function
            lexer.GetNextTokenOfType(TokenType.KwFunction, out _, out _);

            //解析函数名
            bool hasColon = ParseFuncName(lexer, out BaseExp funcNameExp);

            //解析函数定义表达式
            FuncDefExp funcDefExp = ParseFuncDefExp(lexer);

            if (hasColon)
            {
                //有冒号 将隐式参数self放入参数列表首位
                List<string> paramList = new List<string>(funcDefExp.ParamList);
                paramList.Insert(0, "self");
                funcDefExp.ParamList = paramList.ToArray();
            }

            return new AssignStat(funcDefExp.Line, new BaseExp[] { funcNameExp }, new BaseExp[] { funcDefExp });
        }

        /// <summary>
        /// 解析函数名的表达式
        /// </summary>
        private static bool ParseFuncName(Lexer lexer, out BaseExp exp)
        {
            //是否有冒号
            bool hasColon = false;

            lexer.GetNextIdentifier(out int line, out string name);
            exp = new NameExp(line, name);

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
                StringExp key = new StringExp(line, name);
                exp = new TableAccessExp(line, exp, key);
                hasColon = true;
            }

            return hasColon;
        }
    }

}
