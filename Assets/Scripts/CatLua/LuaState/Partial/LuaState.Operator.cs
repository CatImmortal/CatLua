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
            LuaDataUnion b = Pop();
            LuaDataUnion a = b;
            if (type != ArithOpType.Unm && type != ArithOpType.BNot)
            {
                //二元运算
                a = Pop();
            }

            ArithOpConfig operatorConfig = ArithOpConfig.Configs[(int)type];

            LuaDataUnion result = Arith(a, b, operatorConfig);

            if (result.Type == LuaDataType.Nil)
            {
                //无法进行运算 尝试调用元方法进行运算
                if (!TryCallMetaMethod(a, b, operatorConfig.MetaMethodName, out result))
                {
                    throw new Exception("运算失败,type == " + type);
                }
            }



            globalStack.Push(result);
            return;
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
                    return Factory.NewInteger(result);
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
                    return Factory.NewNumber(result);
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
                return Factory.NewInteger(result);
            }

            //否则尝试调用浮点数版本
            if (d1Result && d2Result)
            {
                double result = operatorConfig.NumberFunc(d1, d2);
                return Factory.NewNumber(result);
            }

            return default;
        }

        /// <summary>
        /// 比较运算
        /// </summary>
        public bool Compare(int index1, int index2, CompareOpType type)
        {
            LuaDataUnion a = globalStack.Get(index1);
            LuaDataUnion b = globalStack.Get(index2);
            CompareOpConfig compareOpConfig = CompareOpConfig.Configs[(int)type];
            return compareOpConfig.CompareFunc(a, b,this);
        }

        /// <summary>
        /// 将栈中index位置的字符串的长度压入栈顶
        /// </summary>
        public void Len(int index)
        {
            LuaDataUnion value = globalStack.Get(index);
  
            if (value.Type == LuaDataType.String)
            {
                globalStack.Push(Factory.NewInteger(value.Str.Length));
                return;

            }

            //不是string 尝试调用元方法
            if (!TryCallMetaMethod(value,value,"__len",out LuaDataUnion result))
            {
                //没有__len关联的元方法 但是是个table 返回数组部分长度
                if (value.Type == LuaDataType.Table)
                {
                    globalStack.Push(Factory.NewInteger(value.Table.Length));
                    return;
                }
            }

            globalStack.Push(result);
        }

        /// <summary>
        /// 从栈顶弹出n个值进行拼接，然后将结果压入栈顶
        /// </summary>
        public void Concat(int n)
        {
            if (n == 0)
            {
                globalStack.Push(Factory.NewString(string.Empty));
            }
            else if (n >= 2)
            {
                for (int i = 1; i < n; i++)
                {
                    
                    if (IsString(-1) && IsString(-2))
                    {
                        //检查栈顶的2个值是否为stirng或可以转换为string
                        string s2 = Pop().ToString();
                        string s1 = Pop().ToString();
                        string result = s1 + s2;

                        Push(result);

                    }
                    else
                    {
                        //有无法转换string的 尝试调用元方法进行拼接
                        LuaDataUnion b = Pop();
                        LuaDataUnion a = Pop();
                        if (TryCallMetaMethod(a,b,"__concat",out LuaDataUnion result))
                        {
                            Push(result.Str);
                        }
                        else
                        {
                            throw new Exception("Concat调用失败");
                        }
                    }
                }
            }
        }
    }
}