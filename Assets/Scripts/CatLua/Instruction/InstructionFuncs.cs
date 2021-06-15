﻿using System.Collections;
using System.Collections.Generic;
using System;

namespace CatLua
{
    /// <summary>
    /// 指令的函数实现
    /// </summary>
    public class InstructionFuncs
    {
        public static Action<Instructoin, LuaState> Move = MoveFunc;
        public static Action<Instructoin, LuaState> Jmp = JmpFunc;
        public static Action<Instructoin, LuaState> LoadNil = LoadNilFunc;
        public static Action<Instructoin, LuaState> LoadBool = LoadBoolFunc;
        public static Action<Instructoin, LuaState> LoadK = LoadKFunc;
        public static Action<Instructoin, LuaState> LoadKX = LoadKXFunc;

        public static Action<Instructoin, LuaState> Add = (i, vm) => { BinaryArithFunc(i, vm, ArithOpType.Add); };
        public static Action<Instructoin, LuaState> Sub = (i, vm) => { BinaryArithFunc(i, vm, ArithOpType.Sub); };
        public static Action<Instructoin, LuaState> Mul = (i, vm) => { BinaryArithFunc(i, vm, ArithOpType.Mul); };
        public static Action<Instructoin, LuaState> Mod = (i, vm) => { BinaryArithFunc(i, vm, ArithOpType.Mod); };
        public static Action<Instructoin, LuaState> Pow = (i, vm) => { BinaryArithFunc(i, vm, ArithOpType.Pow); };
        public static Action<Instructoin, LuaState> Div = (i, vm) => { BinaryArithFunc(i, vm, ArithOpType.Div); };
        public static Action<Instructoin, LuaState> IDiv = (i, vm) => { BinaryArithFunc(i, vm, ArithOpType.IDiv); };
        public static Action<Instructoin, LuaState> BAnd = (i, vm) => { BinaryArithFunc(i, vm, ArithOpType.BAnd); };
        public static Action<Instructoin, LuaState> BOr = (i, vm) => { BinaryArithFunc(i, vm, ArithOpType.BOr); };
        public static Action<Instructoin, LuaState> BXOr = (i, vm) => { BinaryArithFunc(i, vm, ArithOpType.BXor); };
        public static Action<Instructoin, LuaState> ShL = (i, vm) => { BinaryArithFunc(i, vm, ArithOpType.ShL); };
        public static Action<Instructoin, LuaState> ShR = (i, vm) => { BinaryArithFunc(i, vm, ArithOpType.ShR); };
        public static Action<Instructoin, LuaState> Unm = (i, vm) => { UnaryArithFunc(i, vm, ArithOpType.Unm); };
        public static Action<Instructoin, LuaState> BNot = (i, vm) => { UnaryArithFunc(i, vm, ArithOpType.BNot); };

        public static Action<Instructoin, LuaState> Len = LenFunc;
        public static Action<Instructoin, LuaState> Concat = ConcatFunc;

        public static Action<Instructoin, LuaState> Eq = (i, vm) => { CompareFunc(i, vm, CompareOpType.Eq); };
        public static Action<Instructoin, LuaState> Lt = (i, vm) => { CompareFunc(i, vm, CompareOpType.Lt); };
        public static Action<Instructoin, LuaState> Le = (i, vm) => { CompareFunc(i, vm, CompareOpType.Le); };


        public static Action<Instructoin, LuaState> Not = NotFunc;

        public static Action<Instructoin, LuaState> TestSet = TestSetFunc;
        public static Action<Instructoin, LuaState> Test = TestFunc;

        public static Action<Instructoin, LuaState> ForPrep = ForPrepFunc;
        public static Action<Instructoin, LuaState> ForLoop = ForLoopFunc;

        public static Action<Instructoin, LuaState> NewTable = NewTableFunc;
        public static Action<Instructoin, LuaState> GetTable = GetTableFunc;
        public static Action<Instructoin, LuaState> SetTable = SetTableFunc;
        public static Action<Instructoin, LuaState> SetList = SetListFunc;

        /// <summary>
        /// 将b位置的栈值复制到a位置
        /// </summary>
        private static void MoveFunc(Instructoin i, LuaState vm)
        {
            //Lua的局部变量都在寄存器里
            //用8bit编码目标寄存器索引（参数A），所以理论上Lua局部变量不能超过255个
            //实际上官方Lua编译器里超过200个就无法编译了

            i.GetABC(out int a, out int b, out int c);
            a++;
            b++;
            vm.Copy(b, a);
        }

        /// <summary>
        /// 为PC加上sbx的值
        /// </summary>
        private static void JmpFunc(Instructoin i, LuaState vm)
        {


            i.GetAsBx(out int a, out int sbx);
            vm.AddPC(sbx);
            if (a != 0)
            {
                //处理upvalue
                //todo
            }
        }

        /// <summary>
        /// 将a到b位置的栈值设为nil
        /// </summary>
        private static void LoadNilFunc(Instructoin i, LuaState vm)
        {
            i.GetABC(out int a, out int b, out int c);
            a++;

            //压入一个nil到栈顶
            vm.Push();

            for (int index = a; index <= a + b; index++)
            {
                //copy栈顶的nil值到index位置上
                vm.Copy(-1, index);
            }

            //弹出栈顶的nil，恢复栈
            vm.Pop(1);
        }

        /// <summary>
        /// 将a位置的栈值设为bool类型的b，如果c不是0,就跳过下一条指令
        /// </summary>
        private static void LoadBoolFunc(Instructoin i, LuaState vm)
        {
            i.GetABC(out int a, out int b, out int c);
            a++;

            vm.Push(b != 0);
            vm.PopAndCopy(a);

            if (c != 0)
            {
                vm.AddPC(1);
            }
        }

        /// <summary>
        /// 将常量表bx位置的值复制到栈中a位置
        /// </summary>
        private static void LoadKFunc(Instructoin i, LuaState vm)
        {
            i.GetABx(out int a, out int bx);
            a++;

            vm.PushConst(bx);
            vm.PopAndCopy(a);
        }

        /// <summary>
        /// 将常量表ax位置的值复制到栈中a位置，常量数量超过262143时使用
        /// </summary>
        private static void LoadKXFunc(Instructoin i, LuaState vm)
        {
            i.GetABx(out int a, out int bx);
            a++;

            Instructoin next = new Instructoin(vm.Fetch());
            next.GetAx(out int ax);

            vm.PushConst(ax);
            vm.PopAndCopy(a);
        }

        /// <summary>
        /// 将b和c位置的值进行二元运算，然后将结果复制到栈的a位置
        /// </summary>
        private static void BinaryArithFunc(Instructoin i,LuaState vm,ArithOpType type)
        {
            i.GetABC(out int a, out int b, out int c);
            a++;

            vm.PushRK(b);
            vm.PushRK(c);
            vm.Arith(type);
            vm.PopAndCopy(a);
        }

        /// <summary>
        /// 对b位置的值进行一元运算，然后将结果复制到栈的a位置
        /// </summary>
        private static void UnaryArithFunc(Instructoin i, LuaState vm, ArithOpType type)
        {
            i.GetABC(out int a, out int b, out int c);
            a++;
            b++;

            vm.CopyAndPush(b);
            vm.Arith(type);
            vm.PopAndCopy(a);
        }

        /// <summary>
        /// 将b位置的字符串的长度复制到a位置
        /// </summary>
        private static void LenFunc(Instructoin i, LuaState vm)
        {
            i.GetABC(out int a, out int b, out int c);
            a++;
            b++;

            vm.Len(b);
            vm.PopAndCopy(a);
        }

        /// <summary>
        /// 将b到c位置的值连接，然后复制到a位置
        /// </summary>
        private static void ConcatFunc(Instructoin i, LuaState vm)
        {
            i.GetABC(out int a, out int b, out int c);
            a++;
            b++;
            c++;

            for (int index = b; index <= c; index++)
            {
                vm.CopyAndPush(index);
            }

            vm.Concat((c - b)+ 1);
            vm.PopAndCopy(a);
        }

        /// <summary>
        /// 将b和c位置的值进行比较，如果结果和a不匹配，就跳过下一条指令
        /// </summary>
        private static void CompareFunc(Instructoin i, LuaState vm,CompareOpType type)
        {
            i.GetABC(out int a, out int b, out int c);

            vm.PushRK(b);
            vm.PushRK(c);
            bool result = vm.Compare(-1, -2, type);

            bool target = a != 0;

            if (result != target)
            {
                vm.AddPC(1);
            }

            vm.Pop(2);
        }

        /// <summary>
        /// 将b位置的bool值取反，然后复制到a位置
        /// </summary>
        private static void NotFunc(Instructoin i, LuaState vm)
        {
            i.GetABC(out int a, out int b, out int c);
            a++;
            b++;

            bool value = vm.GetBoolean(b);
            vm.Push(!value);
            vm.PopAndCopy(a);
        }

        /// <summary>
        /// 将b中的bool值和c作比较，如果一样就将b的值复制到a中，否则跳过下一条指令
        /// </summary>
        private static void TestSetFunc(Instructoin i, LuaState vm)
        {
            i.GetABC(out int a, out int b, out int c);
            a++;
            b++;

            bool value = vm.GetBoolean(b);
            bool target = c != 0;

            if (value == target)
            {
                vm.Copy(b, a);
            }
            else
            {
                vm.AddPC(1);
            }
        }

        /// <summary>
        /// 将a中的bool值和c作比较，如果不一样，就跳过下一条指令
        /// </summary>
        private static void TestFunc(Instructoin i, LuaState vm)
        {
            i.GetABC(out int a, out int b, out int c);
            a++;

            bool value = vm.GetBoolean(a);
            bool target = c != 0;

            if (value != target)
            {
                vm.AddPC(1);
            }
        }

        /// <summary>
        /// 将a位置的值(index)减去a+2的值(step)，初始化index,然后让PC增加sbx，指向循环体
        /// </summary>
        private static void ForPrepFunc(Instructoin i, LuaState vm)
        {
            //a index
            //a+1 limit
            //a+2 step
            //a+3 i

            i.GetAsBx(out int a, out int sbx);
            a++;

            //R(A) -= R(A + 2)
            vm.CopyAndPush(a);
            vm.CopyAndPush(a + 2);
            vm.Arith(ArithOpType.Sub);
            vm.PopAndCopy(a);

            //PC += sBx
            vm.AddPC(sbx);
        }

        /// <summary>
        /// 将a位置的值(index)加上2(step)，增加循环的index
        /// 然后判断index是否达到了limit，如果未达到就让PC增加sbx，指向循环体,并把index值复制给用户的自定义局部变量
        /// </summary>
        private static void ForLoopFunc(Instructoin i, LuaState vm)
        {
            i.GetAsBx(out int a, out int sbx);
            a++;

            //R(A) += R(A + 2)
            vm.CopyAndPush(a);
            vm.CopyAndPush(a + 2);
            vm.Arith(ArithOpType.Add);
            vm.PopAndCopy(a);

            //R(A) <?= R(A + 1)
            
            //step是否为正数
            bool isPositiveStep = vm.GetNumber(a + 2) >= 0;

            if (
                (isPositiveStep && vm.Compare(a, a + 1, CompareOpType.Le))
                ||(!isPositiveStep && vm.Compare(a + 1, a, CompareOpType.Le))
              
                )
            {
                //step是正数，并且index<=limit
                //或者  step是负数，并且index >= limit

                vm.AddPC(sbx);  // PC += sBx
                vm.Copy(a, a + 3);  //R(A+3)=R(A)
            }

        }

        /// <summary>
        /// 创建空表，放入a位置，空表的数组大小=b，字典大小=c
        /// </summary>
        private static void NewTableFunc(Instructoin i, LuaState vm)
        {
            i.GetABC(out int a, out int b, out int c);
            a++;

            //为了用9bit表示大于521的数
            //NewTable指令使用了浮点字节编码B和C 需要进行转换
            vm.CreateTable(LMath.Fb2Int(b), LMath.Fb2Int(c));
            vm.PopAndCopy(a);
        }

        /// <summary>
        /// 从b位置获取table,从c位置获取key，将table[key]放入a位置
        /// </summary>
        private static void GetTableFunc(Instructoin i, LuaState vm)
        {
            i.GetABC(out int a, out int b, out int c);
            a++;
            b++;

            vm.PushRK(c); 
            vm.PushTableValue(b);
            vm.PopAndCopy(a);
        }

        /// <summary>
        /// 从a位置获取table,从b位置获取key，从c位置获取value,table[key] = value
        /// </summary>
        private static void SetTableFunc(Instructoin i, LuaState vm)
        {
            i.GetABC(out int a, out int b, out int c);
            a++;

            vm.PushRK(b);
            vm.PushRK(c);
            vm.SetTableValue(a);
        }

        /// <summary>
        /// 将从a+1位置开始的b个值放入a位置table的数组部分，c表示批次数，起始索引=批次数*批大小
        /// 如果c>0，那么c表示批次数+1，否则批次数放在了下一条指令的ax里
        /// </summary>
        private static void SetListFunc(Instructoin i, LuaState vm)
        {
            i.GetABC(out int a, out int b, out int c);
            a++;

            if (c > 0)
            {
                c = c - 1;
            }
            else
            {
                new Instructoin(vm.Fetch()).GetAx(out int ax);
                c = ax;
            }

            long key = c * Constants.SetListDefaultBatch;
            for (int num  = 1; num <= b; num ++)
            {
                key++;
                vm.CopyAndPush(a + num);
                vm.SetTableValue(a, key);
            }
        }
    }

}
