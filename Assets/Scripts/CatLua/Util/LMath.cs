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
        /// 尝试将表示整数的浮点数转换为对应整数（比如3.0转换为3），
        /// 如果小数部分不为0或者范围超出了会返回flase
        /// </summary>
        public static bool TryNumberToInteger(double d, out long l)
        {
            l = (long)d;
            return l == d;
        }

        /// <summary>
        /// int转换为浮点字节编码
        /// 如果某个字节用二进制写成 eeeeexxx，当eeeee=0时，该字节表示的整数就是xxx
        /// 否则该字节表示的整数是(1xxx) * 2^(eeeee - 1)
        /// </summary>
 
        public static int Int2Fb(int x)
        {
            if (x < 8)
            {
                return x;
            }

            int e = 0;
            while (x >= (8 << 4))
            {
                x = (x + 0xf) >> 4;  //x = ceil(x/16)
                e++;
            }

            while (x >= (8 << 1))
            {
                x = (x + 1) >> 1; //x = ceil(x/2)
                e++;
            }

            return ((e + 1) << 3) | (x - 8);
        }

        /// <summary>
        /// 浮点字节编码转int
        /// </summary>
        public static int Fb2Int(int x)
        {
            if (x < 8)
            {
                return x;
            }

            return (x & 7) + 8;
        }
    }

}


