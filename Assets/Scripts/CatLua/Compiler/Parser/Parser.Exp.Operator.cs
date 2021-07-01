using System.Collections.Generic;

namespace CatLua
{
    public partial class Parser
    {
        /// <summary>
        /// 解析表达式12 (or)
        /// </summary>
        private static BaseExp ParseExp12(Lexer lexer)
        {
            //解析左侧表达式
            BaseExp leftExp = ParseExp11(lexer);

            while (lexer.LookNextTokenType() == TokenType.OpOr)
            {
                lexer.GetNextToken(out int line, out _, out TokenType type);

                //解析右侧表达式
                BaseExp rightExp = ParseExp11(lexer);

                //将左侧表达式和右侧表达式 合为一个新的左侧表达式 实现运算符的左结合性
                //如： a or b or c = (a or b) or c
                leftExp = new BinopExp(line, type, leftExp, rightExp);
            }

            return leftExp;
        }

        /// <summary>
        /// 解析表达式11 (and)
        /// </summary>
        private static BaseExp ParseExp11(Lexer lexer)
        {
            BaseExp leftExp = ParseExp10(lexer);

            while (lexer.LookNextTokenType() == TokenType.OpAnd)
            {
                lexer.GetNextToken(out int line, out _, out TokenType type);
                BaseExp rightExp = ParseExp10(lexer);
                leftExp = new BinopExp(line, type, leftExp, rightExp);
            }

            return leftExp;

        }

        /// <summary>
        /// 解析表达式10 (> < >= <= == ~=)
        /// </summary>
        private static BaseExp ParseExp10(Lexer lexer)
        {
            BaseExp leftExp = ParseExp9(lexer);

            while (true)
            {

                switch (lexer.LookNextTokenType())
                {
                    case TokenType.OpLt:
                    case TokenType.OpLe:
                    case TokenType.OpGt:
                    case TokenType.OpGe:
                    case TokenType.OpEq:
                    case TokenType.OpNe:
                        lexer.GetNextToken(out int line, out _, out TokenType type);
                        BaseExp rightExp = ParseExp9(lexer);
                        leftExp = new BinopExp(line, type, leftExp, rightExp);
                        break;
                    default:
                        return leftExp;
                }
            }
        }

        /// <summary>
        /// 解析表达式9 (|)
        /// </summary>
        private static BaseExp ParseExp9(Lexer lexer)
        {
            BaseExp leftExp = ParseExp8(lexer);

            while (lexer.LookNextTokenType() == TokenType.OpBOr)
            {
                lexer.GetNextToken(out int line, out _, out TokenType type);
                BaseExp rightExp = ParseExp8(lexer);
                leftExp = new BinopExp(line, type, leftExp, rightExp);
            }

            return leftExp;

        }

        /// <summary>
        /// 解析表达式8 (~:xor)
        /// </summary>
        private static BaseExp ParseExp8(Lexer lexer)
        {
            BaseExp leftExp = ParseExp7(lexer);

            while (lexer.LookNextTokenType() == TokenType.OpBXor)
            {
                lexer.GetNextToken(out int line, out _, out TokenType type);
                BaseExp rightExp = ParseExp7(lexer);
                leftExp = new BinopExp(line, type, leftExp, rightExp);
            }
            return leftExp;
           
        }

        /// <summary>
        /// 解析表达式7 (&)
        /// </summary>
        private static BaseExp ParseExp7(Lexer lexer)
        {
            BaseExp leftExp = ParseExp6(lexer);

            while (lexer.LookNextTokenType() == TokenType.OpBAnd)
            {
                lexer.GetNextToken(out int line, out _, out TokenType type);
                BaseExp rightExp = ParseExp6(lexer);
                leftExp = new BinopExp(line, type, leftExp, rightExp);
            }
            return leftExp;

        }

        /// <summary>
        /// 解析表达式6 (<< >>)
        /// </summary>
        private static BaseExp ParseExp6(Lexer lexer)
        {
            BaseExp leftExp = ParseExp5(lexer);

            while (true)
            {
                switch (lexer.LookNextTokenType())
                {
                    case TokenType.OpShL:
                    case TokenType.OpShR:
                        lexer.GetNextToken(out int line, out _, out TokenType type);
                        BaseExp rightExp = ParseExp5(lexer);
                        leftExp = new BinopExp(line, type, leftExp, rightExp);
                        break;
                    default:
                        return leftExp;
                }
            }

        }

        /// <summary>
        /// 解析表达式5 (..)
        /// </summary>
        private static BaseExp ParseExp5(Lexer lexer)
        {
            BaseExp exp = ParseExp4(lexer);
            if (lexer.LookNextTokenType() != TokenType.OpConcat)
            {
                return exp;
            }

            int line = 0;
            List<BaseExp> exps = new List<BaseExp>();
            exps.Add(exp);

            while (lexer.LookNextTokenType() == TokenType.OpConcat)
            {
                //生成多叉树
                lexer.GetNextToken(out line, out _, out _);
                exp = ParseExp4(lexer);
                exps.Add(exp);
            }

            return new ConcatExp(line, exps.ToArray());
        }

        /// <summary>
        /// 解析表达式4 (+ -)
        /// </summary>
        private static BaseExp ParseExp4(Lexer lexer)
        {
            BaseExp leftExp = ParseExp3(lexer);

            while (true)
            {
                switch (lexer.LookNextTokenType())
                {
                    case TokenType.OpAdd:
                    case TokenType.OpSub:
                        lexer.GetNextToken(out int line, out _, out TokenType type);
                        BaseExp rightExp = ParseExp3(lexer);
                        leftExp = new BinopExp(line, type, leftExp, rightExp);
                        break;
                    default:
                        return leftExp;
                }
            }
        }

        /// <summary>
        /// 解析表达式3 (* / // %)
        /// </summary>
        private static BaseExp ParseExp3(Lexer lexer)
        {
            BaseExp leftExp = ParseExp2(lexer);

            while (true)
            {
                switch (lexer.LookNextTokenType())
                {
                    case TokenType.OpMul:
                    case TokenType.OpDiv:
                    case TokenType.OpIDiv:
                    case TokenType.OpMod:
                        lexer.GetNextToken(out int line, out _, out TokenType type);
                        BaseExp rightExp = ParseExp2(lexer);
                        leftExp = new BinopExp(line, type, leftExp, rightExp);
                        break;
                    default:
                        return leftExp;
                }
            }
           
        }

       

        /// <summary>
        /// 解析表达式2 (not # - ~)
        /// </summary>
        private static BaseExp ParseExp2(Lexer lexer)
        {
            switch (lexer.LookNextTokenType())
            {
                case TokenType.OpNot:
                case TokenType.OpLen:
                case TokenType.OpUnm:
                case TokenType.OpBNot:
                    lexer.GetNextToken(out int line, out _, out TokenType type);
                    //递归调用 实现一元运算符的右结合性
                    BaseExp rightExp = ParseExp2(lexer);
                    return new UnopExp(line, type, rightExp);
            }

            return ParseExp1(lexer);
        }

        /// <summary>
        /// 解析表达式1 (^)
        /// </summary>
        private static BaseExp ParseExp1(Lexer lexer)
        {
            //解析左侧表达式
            BaseExp leftExp = ParseExp0(lexer);

            if (lexer.LookNextTokenType() == TokenType.OpPow)
            {
                lexer.GetNextToken(out int line, out _, out TokenType type);

                //解析右侧表达式 递归调用 实现乘方的右结合性
                BaseExp rightExp = ParseExp2(lexer);

                leftExp = new BinopExp(line, type, leftExp, rightExp);
            }

            return leftExp;
        }
    }

}
