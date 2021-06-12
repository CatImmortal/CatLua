using System.Collections;
using System.Collections.Generic;
using System;

namespace CatLua
{
    /// <summary>
    /// Lua的特殊数学运算
    /// </summary>
    public static class LMath
    {

        /// <summary>
        /// 左移运算，左移-n位相当于右移n位
        /// </summary>
       public static long BitLeftMove(long a,long n)
        {
            int i = (int)n;

            if (n >= 0)
            {
                return a << i;
            }
            else
            {
                return BitRightMove(a, -n);
            }
        }

        /// <summary>
        /// 无符号右移运算，右移-n位相当于左移n位
        /// </summary>
        public static long BitRightMove(long a, long n)
        {
            int i = (int)n;

            if (n >= 0)
            {
                if (a >= 0)
                {
                    return a >> i;
                }
                else
                {
                    //希望使用无符号右移 所以先转成ulong 右移后再转回long
                    return (long)((ulong)a >> i);
                }

              
            }
            else
            {
                return BitLeftMove(a, -n);
            }
        }

        /// <summary>
        /// 尝试将Lu numbner转换为lua integer（比如3.0转换为3），
        /// 如果小数部分不为0或者范围超出了会返回flase
        /// </summary>
        public static bool TryNumberToInteger(double d, out long l)
        {
            l = (long)d;
            return l == d;
        }


    }

}


