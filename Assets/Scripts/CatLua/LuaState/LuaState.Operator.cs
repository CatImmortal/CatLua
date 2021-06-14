using System;
using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    public partial class LuaState
    {
        /// <summary>
        /// 数学与位运算
        /// </summary>
        public void Arith(ArithOpType type)
        {
            LuaDataUnion b = stack.Pop();
            LuaDataUnion a = b;
            if (type != ArithOpType.Unm && type != ArithOpType.BNot)
            {
                //二元运算
                a = stack.Pop();
            }

            ArithOpConfig operatorConfig = ArithOpConfig.Configs[(int)type];

            LuaDataUnion result = Arith(a, b, operatorConfig);

            if (result.Type == LuaDataType.Nil)
            {
                throw new Exception("数学运算出错，结果为nil");
            }
            stack.Push(result);
        }

        /// <summary>
        /// 数学与位运算
        /// </summary>
        private LuaDataUnion Arith(LuaDataUnion a, LuaDataUnion b, ArithOpConfig operatorConfig)
        {
            long l1;
            long l2;
            bool l1Result = a.TryConvertToInteger(out l1);
            bool l2Result = b.TryConvertToInteger(out l2);

            if (operatorConfig.NumberFunc == null)
            {
                //只支持整数运算
                //主要是位运算一类的
                
                if (l1Result && l2Result)
                {
                    //操作数都可以转换为整数 
                    //调用整数版本函数实现
                    long result = operatorConfig.IntegerFunc(l1, l2);
                    return new LuaDataUnion(LuaDataType.Integer, integer: result);
                }
                else
                {
                    return default;
                }
                
            }

            double d1;
            double d2;
            bool d1Result = a.TryConvertToNumber(out d1);
            bool d2Result = b.TryConvertToNumber(out d2);

            if (operatorConfig.IntegerFunc == null)
            {
                //只支持浮点数运算
                //除法和乘方

                if (d1Result && d2Result)
                {
                    //操作数都可以转换为浮点数
                    //调用浮点版本函数实现
                    double result = operatorConfig.NumberFunc(d1, d2);
                    return new LuaDataUnion(LuaDataType.Number, number: result);
                }
                else
                {
                    return default;
                }

            }

            //浮点数和整数都支持

            //操作数都是整数 直接调用整数版本
            if (a.Type == LuaDataType.Integer && b.Type == LuaDataType.Integer)
            {
                long result = operatorConfig.IntegerFunc(l1, l2);
                return new LuaDataUnion(LuaDataType.Integer, integer: result);
            }

            //否则尝试调用浮点数版本
            if (d1Result && d2Result)
            {
                double result = operatorConfig.NumberFunc(d1, d2);
                return new LuaDataUnion(LuaDataType.Number, number: result);
            }

            return default;

            //if (operatorConfig.NumberFunc == null)
            //{
            //    //操作数只能 是整数 或者 可以转换为整数
            //    long l1;
            //    long l2;
            //    if (a.TryConvertToInteger(out l1) && b.TryConvertToInteger(out l2))
            //    {
            //        long result = operatorConfig.IntegerFunc(l1, l2);
            //        return new LuaDataUnion(LuaDataType.Integer, integer: result);
            //    }
            //}

            ////数学运算

            //if (operatorConfig.IntegerFunc != null)
            //{
            //    // add sub mul mod idiv unm

            //    //整数运算
            //    if (a.Type == LuaDataType.Integer && b.Type == LuaDataType.Integer)
            //    {
            //        long result = operatorConfig.IntegerFunc(a.Integer, b.Integer);
            //        return new LuaDataUnion(LuaDataType.Integer, integer: result);
            //    }


            //}

            ////pow div 或者是其他非整数数学运算
            //double d1;
            //double d2;
            //if (a.TryConvertToNumber(out d1) && b.TryConvertToNumber(out d2))
            //{
            //    double result = operatorConfig.NumberFunc(d1, d2);
            //    return new LuaDataUnion(LuaDataType.Number, number: result);
            //}

            //return default;

        }

        /// <summary>
        /// 比较运算
        /// </summary>
        public bool Compare(int index1, int index2, CompareOpType type)
        {
            LuaDataUnion a = stack.Get(index1);
            LuaDataUnion b = stack.Get(index2);
            CompareOpConfig compareOpConfig = CompareOpConfig.Configs[(int)type];
            return compareOpConfig.CompareFunc(a, b);
        }

        /// <summary>
        /// 将栈中index位置的字符串的长度压入栈顶
        /// </summary>
        public void Len(int index)
        {
            LuaDataUnion value = stack.Get(index);
            long len;
            if (value.Type == LuaDataType.String)
            {
                len = value.Str.Length;
               
            }
            else if (value.Type == LuaDataType.Table)
            {
                len = value.Table.Length;
            }
            else
            {
                throw new Exception("Len方法不能对字符串以外的值使用");
            }

            stack.Push(new LuaDataUnion(LuaDataType.Integer, integer: len));
        }

        /// <summary>
        /// 从栈顶弹出n个值进行拼接，然后将结果压入栈顶
        /// </summary>
        public void Concat(int n)
        {
            if (n == 0)
            {
                stack.Push(new LuaDataUnion(LuaDataType.String, str: string.Empty));
            }
            else if (n >= 2)
            {
                for (int i = 1; i < n; i++)
                {
                    //检查栈顶的2个值是否为stirng或可以转换为string
                    if (IsString(-1) && IsString(-2))
                    {
                        string s2 = stack.Pop().ToString();
                        string s1 = stack.Pop().ToString();
                        string result = s1 + s2;

                        stack.Push(new LuaDataUnion(LuaDataType.String, str: result));

                    }
                    else
                    {
                        throw new Exception(string.Format("Concat错误"));
                    }
                }
            }
        }
    }
}