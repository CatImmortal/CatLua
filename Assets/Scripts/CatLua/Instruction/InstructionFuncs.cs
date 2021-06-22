using System.Collections;
using System.Collections.Generic;
using System;

namespace CatLua
{
    /// <summary>
    /// 指令的函数实现
    /// </summary>
    public class InstructionFuncs
    {
        public static Action<Instructoin, LuaState> MoveFunc = Move;
        public static Action<Instructoin, LuaState> JmpFunc = Jmp;
        public static Action<Instructoin, LuaState> LoadNilFunc = LoadNil;
        public static Action<Instructoin, LuaState> LoadBoolFunc = LoadBool;
        public static Action<Instructoin, LuaState> LoadKFunc = LoadK;
        public static Action<Instructoin, LuaState> LoadKXFunc = LoadKX;

        public static Action<Instructoin, LuaState> AddFunc = (i, vm) => { BinaryArith(i, vm, ArithOpType.Add); };
        public static Action<Instructoin, LuaState> SubFunc = (i, vm) => { BinaryArith(i, vm, ArithOpType.Sub); };
        public static Action<Instructoin, LuaState> MulFunc = (i, vm) => { BinaryArith(i, vm, ArithOpType.Mul); };
        public static Action<Instructoin, LuaState> ModFunc = (i, vm) => { BinaryArith(i, vm, ArithOpType.Mod); };
        public static Action<Instructoin, LuaState> PowFunc = (i, vm) => { BinaryArith(i, vm, ArithOpType.Pow); };
        public static Action<Instructoin, LuaState> DivFunc = (i, vm) => { BinaryArith(i, vm, ArithOpType.Div); };
        public static Action<Instructoin, LuaState> IDivFunc = (i, vm) => { BinaryArith(i, vm, ArithOpType.IDiv); };
        public static Action<Instructoin, LuaState> BAndFunc = (i, vm) => { BinaryArith(i, vm, ArithOpType.BAnd); };
        public static Action<Instructoin, LuaState> BOrFunc = (i, vm) => { BinaryArith(i, vm, ArithOpType.BOr); };
        public static Action<Instructoin, LuaState> BXOrFunc = (i, vm) => { BinaryArith(i, vm, ArithOpType.BXor); };
        public static Action<Instructoin, LuaState> ShLFunc = (i, vm) => { BinaryArith(i, vm, ArithOpType.ShL); };
        public static Action<Instructoin, LuaState> ShRFunc = (i, vm) => { BinaryArith(i, vm, ArithOpType.ShR); };
        public static Action<Instructoin, LuaState> UnmFunc = (i, vm) => { UnaryArith(i, vm, ArithOpType.Unm); };
        public static Action<Instructoin, LuaState> BNotFunc = (i, vm) => { UnaryArith(i, vm, ArithOpType.BNot); };

        public static Action<Instructoin, LuaState> LenFunc = Len;
        public static Action<Instructoin, LuaState> ConcatFunc = Concat;

        public static Action<Instructoin, LuaState> EqFunc = (i, vm) => { Compare(i, vm, CompareOpType.Eq); };
        public static Action<Instructoin, LuaState> LtFunc = (i, vm) => { Compare(i, vm, CompareOpType.Lt); };
        public static Action<Instructoin, LuaState> LeFunc = (i, vm) => { Compare(i, vm, CompareOpType.Le); };


        public static Action<Instructoin, LuaState> NotFunc = Not;

        public static Action<Instructoin, LuaState> TestSetFunc = TestSet;
        public static Action<Instructoin, LuaState> TestFunc = Test;

        public static Action<Instructoin, LuaState> ForPrepFunc = ForPrep;
        public static Action<Instructoin, LuaState> ForLoopFunc = ForLoop;

        public static Action<Instructoin, LuaState> NewTableFunc = NewTable;
        public static Action<Instructoin, LuaState> GetTableFunc = GetTable;
        public static Action<Instructoin, LuaState> SetTableFunc = SetTable;
        public static Action<Instructoin, LuaState> SetListFunc = SetList;

        public static Action<Instructoin, LuaState> ClosureFunc = Closure;
        public static Action<Instructoin, LuaState> CallFunc = Call;
        public static Action<Instructoin, LuaState> ReturnFunc = Return;
        public static Action<Instructoin, LuaState> VarArgFunc = VarArg;
        public static Action<Instructoin, LuaState> TailCallFunc = TailCall;
        public static Action<Instructoin, LuaState> SelfFunc = Self;

        public static Action<Instructoin, LuaState> GetUpvalueFunc = GetUpvalue;
        public static Action<Instructoin, LuaState> SetUpvalueFunc = SetUpvalue;
        public static Action<Instructoin, LuaState> GetTabUpFunc = GetTabUp;
        public static Action<Instructoin, LuaState> SetTabUpFunc = SetTabUp;

        /// <summary>
        /// 将b位置的栈值复制到a位置
        /// </summary>
        private static void Move(Instructoin i, LuaState vm)
        {
            //Lua的局部变量都在寄存器里
            //用8bit编码目标寄存器索引（参数A），所以理论上Lua局部变量不能超过255个
            //实际上官方Lua编译器里超过200个就无法编译了

            i.GetABC(out int a, out int b, out int c);
            a += vm.CurFrameBottom;
            b += vm.CurFrameBottom;

            vm.Copy(b,a);
        }

        /// <summary>
        /// 为PC加上sbx的值
        /// </summary>
        private static void Jmp(Instructoin i, LuaState vm)
        {
            i.GetAsBx(out int a, out int sbx);

            vm.AddPC(sbx);

            if (a != 0)
            {
                //处理upvalue
                a += vm.CurFrameBottom - 1;  //这里传的栈索引从1开始 需要再-1
                vm.CloseUpvalue(a);
            }
        }

        /// <summary>
        /// 将a到b位置的栈值设为nil
        /// </summary>
        private static void LoadNil(Instructoin i, LuaState vm)
        {
            i.GetABC(out int a, out int b, out int c);
            a += vm.CurFrameBottom;
            b += vm.CurFrameBottom;


            //压入一个nil到栈顶
            vm.Push();

            for (int index = a; index <= a + b; index++)
            {
                //copy栈顶的nil值到index位置上
                vm.Copy(-1,index);
            }

            //弹出栈顶的nil，恢复栈
            vm.Pop();
        }

        /// <summary>
        /// 将a位置的栈值设为bool类型的b，如果c不是0,就跳过下一条指令
        /// </summary>
        private static void LoadBool(Instructoin i, LuaState vm)
        {
            i.GetABC(out int a, out int b, out int c);
            a += vm.CurFrameBottom;

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
        private static void LoadK(Instructoin i, LuaState vm)
        {
            i.GetABx(out int a, out int bx);
            a += vm.CurFrameBottom;

            vm.PushConst(bx);
            vm.PopAndCopy(a);
        }

        /// <summary>
        /// 将常量表ax位置的值复制到栈中a位置，常量数量超过262143时使用
        /// </summary>
        private static void LoadKX(Instructoin i, LuaState vm)
        {
            i.GetABx(out int a, out int bx);
            a += vm.CurFrameBottom;

            Instructoin next = new Instructoin(vm.Fetch());
            next.GetAx(out int ax);

            vm.PushConst(ax);
            vm.PopAndCopy(a);
        }

        /// <summary>
        /// 将b和c位置的值进行二元运算，然后将结果复制到栈的a位置
        /// </summary>
        private static void BinaryArith(Instructoin i,LuaState vm,ArithOpType type)
        {
            i.GetABC(out int a, out int b, out int c);
            a += vm.CurFrameBottom;

            vm.PushRK(b);
            vm.PushRK(c);
            vm.Arith(type);
            vm.PopAndCopy(a);
        }

        /// <summary>
        /// 对b位置的值进行一元运算，然后将结果复制到栈的a位置
        /// </summary>
        private static void UnaryArith(Instructoin i, LuaState vm, ArithOpType type)
        {
            i.GetABC(out int a, out int b, out int c);
            a += vm.CurFrameBottom;

            vm.CopyAndPush(b);
            vm.Arith(type);
            vm.PopAndCopy(a);
        }

        /// <summary>
        /// 将b位置的字符串的长度复制到a位置
        /// </summary>
        private static void Len(Instructoin i, LuaState vm)
        {
            i.GetABC(out int a, out int b, out int c);
            a += vm.CurFrameBottom;
            b += vm.CurFrameBottom;

            vm.Len(b);
            vm.PopAndCopy(a);
        }

        /// <summary>
        /// 将b到c位置的值连接，然后复制到a位置
        /// </summary>
        private static void Concat(Instructoin i, LuaState vm)
        {
            i.GetABC(out int a, out int b, out int c);
            a += vm.CurFrameBottom;
            b += vm.CurFrameBottom;
            c += vm.CurFrameBottom;


            for (int index = b; index <= c; index++)
            {
                vm.CopyAndPush(index);
            }

            vm.Concat((c - b)+ 1);
            vm.PopAndCopy(a);
        }

        /// <summary>
        /// 将b和c位置的栈值或常量进行比较，如果结果和a不匹配，就跳过下一条指令
        /// </summary>
        private static void Compare(Instructoin i, LuaState vm,CompareOpType type)
        {
            i.GetABC(out int a, out int b, out int c);

            vm.PushRK(b);
            vm.PushRK(c);
            bool result = vm.Compare(-2, -1, type);

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
        private static void Not(Instructoin i, LuaState vm)
        {
            i.GetABC(out int a, out int b, out int c);
            a += vm.CurFrameBottom;
            b += vm.CurFrameBottom;

            bool value = vm.GetBoolean(b);
            vm.Push(!value);
            vm.PopAndCopy(a);
        }

        /// <summary>
        /// 将b中的bool值和c作比较，如果一样就将b的值复制到a中，否则跳过下一条指令
        /// </summary>
        private static void TestSet(Instructoin i, LuaState vm)
        {
            i.GetABC(out int a, out int b, out int c);
            a += vm.CurFrameBottom;
            b += vm.CurFrameBottom;

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
        private static void Test(Instructoin i, LuaState vm)
        {
            i.GetABC(out int a, out int b, out int c);
            a += vm.CurFrameBottom;

            bool value = vm.GetBoolean(vm.CurFrameBottom + a);
            bool target = c != 0;

            if (value != target)
            {
                vm.AddPC(1);
            }
        }

        /// <summary>
        /// 将a位置的值(index)减去a+2的值(step)，初始化index,然后让PC增加sbx，指向循环体
        /// </summary>
        private static void ForPrep(Instructoin i, LuaState vm)
        {
            //a+3 i
            //a+2 step
            //a+1 limit
            //a index

            i.GetAsBx(out int a, out int sbx);

            a += vm.CurFrameBottom;

            //R(A) -= R(A + 2)
            vm.CopyAndPush(a);
            vm.CopyAndPush(a + 2);
            vm.Arith(ArithOpType.Sub);
            vm.PopAndCopy(a);

            //PC += sBx
            vm.AddPC(sbx);


        }

        /// <summary>
        /// 将a位置的值(index)加上a+2位置的值(step)，增加循环的index
        /// 然后判断index是否达到了limit，如果未达到就让PC增加sbx，指向循环体,并把index值复制给用户的自定义局部变量
        /// </summary>
        private static void ForLoop(Instructoin i, LuaState vm)
        {
            i.GetAsBx(out int a, out int sbx);
            a += vm.CurFrameBottom;

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
        private static void NewTable(Instructoin i, LuaState vm)
        {
            i.GetABC(out int a, out int b, out int c);
            a += vm.CurFrameBottom;

            //为了用9bit表示大于521的数
            //NewTable指令使用了浮点字节编码B和C 需要进行转换
            vm.CreateTable(LMath.Fb2Int(b), LMath.Fb2Int(c));
            vm.PopAndCopy(a);
        }

        /// <summary>
        /// 从b位置获取table,从c位置获取key，将table[key]放入a位置
        /// </summary>
        private static void GetTable(Instructoin i, LuaState vm)
        {
            i.GetABC(out int a, out int b, out int c);
            a += vm.CurFrameBottom;
            b += vm.CurFrameBottom;

            vm.PushRK(c); 
            vm.PushTableValue(b);
            vm.PopAndCopy(a);
        }

        /// <summary>
        /// 从a位置获取table,从b位置获取key，从c位置获取value,table[key] = value
        /// </summary>
        private static void SetTable(Instructoin i, LuaState vm)
        {
            i.GetABC(out int a, out int b, out int c);
            a += vm.CurFrameBottom;

            vm.PushRK(b);
            vm.PushRK(c);
            vm.SetTableValue(a);
        }

        /// <summary>
        /// 将从a+1位置开始的b个值放入a位置table的数组部分，c表示批次数，起始索引=批次数*批大小
        /// 如果c>0，那么c表示批次数+1，否则批次数放在了下一条指令的ax里
        /// </summary>
        private static void SetList(Instructoin i, LuaState vm)
        {
            i.GetABC(out int a, out int b, out int c);
            a += vm.CurFrameBottom;

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

            if (b == 0)
            {
                //b为0 说明表构造器最后一个元素是变长参数这种不确定参数数量的
                //从a+1到top都是表的value
                b = vm.Top - a;
            }

            //设置表的数据
            for (int index = 1; index <= b; index++)
            {

                key++;
                vm.CopyAndPush(a + index);
                vm.SetTableValue(a, key);
            }
        }

        /// <summary>
        /// 将bx位置的子函数原型实例化为闭包，放入a中
        /// </summary>
        private static void Closure(Instructoin i, LuaState vm)
        {
            i.GetABx(out int a, out int bx);
            a += vm.CurFrameBottom;

            vm.PushProto(bx);
            vm.PopAndCopy(a);
        }


        /// <summary>
        /// 调用a位置的函数，参数有b-1个，返回值有c-1个，b或c为0的话，表示有不确定长度的参数或返回值
        /// </summary>
        private static void Call(Instructoin i, LuaState vm)
        {
            i.GetABC(out int a, out int b, out int c);
            a += vm.CurFrameBottom;

            //从a位置开始，复制并压入函数和参数到主调栈帧的栈顶
            int ArgsNum = vm.PushFuncAndArgs(a, b);

            //弹出函数和参数，用函数创建被调栈帧并压入，然后压入参数，执行函数内的指令
            vm.CallFunc(ArgsNum, c - 1);
            //遇到return指令则调用结束，将返回值复制并压入栈顶
            //然后弹出栈顶的返回值，弹出被调栈帧，再将这些返回值压入主调栈帧

            //最后弹出主调栈帧栈顶的返回值并复制到a开始的位置
            vm.PopResults(a, c - 1);

        }

        /// <summary>
        /// 被调函数的所有指令都执行完毕后，将a开始的b - 1个数据压入栈顶，以作为返回值压入主调函数栈帧
        /// </summary>
        private static void Return(Instructoin i, LuaState vm)
        {
            i.GetABC(out int a, out int b, out int c);
            a += vm.CurFrameBottom;

            int resultNum = b - 1;

            if (resultNum == 0)
            {
                return;
            }

            if (resultNum == -1)
            {
                //将a到top的所有值都压入栈顶
                resultNum = vm.Top - a + 1;
            }

            for (int index = 0; index < resultNum; index++)
            {
                vm.CopyAndPush(a + index);
            }

            vm.CallFrameReturnResultNum = resultNum;
        }

        /// <summary>
        /// 若b>1，就将b-1个vararg参数复制到a开始的位置，若b==0，就复制全部vararg参数到寄存器
        /// </summary>
        private static void VarArg(Instructoin i, LuaState vm)
        {
            i.GetABC(out int a, out int b, out int c);
            a += vm.CurFrameBottom;

            int argsNum = b - 1;

            if (argsNum == 0)
            {
                return;
            }

            vm.PushVarArg(argsNum);

            if (argsNum == -1)
            {
                //要复制所有变长参数
                argsNum = vm.CurFrameVarArgsNum;
            }

            //取出变长参数
            LuaDataUnion[] datas = vm.PopN(argsNum);

            
            int targetIndex = (a - 1) + argsNum;
            if (targetIndex > vm.Top)
            {
                //扩充栈顶
                vm.SetTop(targetIndex);
            }
           

            //复制变长参数到指定位置
            for (int index = 0; index < datas.Length; index++)
            {
                vm.Push(datas[index]);
                vm.PopAndCopy(a + index);
            }
        }

        private static void TailCall(Instructoin i, LuaState vm)
        {
            Call(i, vm);
        }

        /// <summary>
        /// 将b位置的table和table的函数(函数key来自c位置的函数名)复制到a + 1和a里
        /// </summary>
        private static void Self(Instructoin i, LuaState vm)
        {
            i.GetABC(out int a, out int b, out int c);
            a += vm.CurFrameBottom;
            b += vm.CurFrameBottom;

            vm.Copy(b, a + 1);

            vm.PushRK(c);  //push table function key
            vm.PushTableValue(b);  //push function(table[key])
            vm.PopAndCopy(a);
        }

        /// <summary>
        /// 将当前函数upvalues表中b位置的upvalue值复制到栈的a位置
        /// </summary>
        private static void GetUpvalue(Instructoin i, LuaState vm)
        {
            i.GetABC(out int a, out int b, out int c);
            a += vm.CurFrameBottom;

            vm.Push(vm.GetCurFrameUpvalue(b).Value);
            vm.PopAndCopy(a);

        }

        /// <summary>
        /// 将栈中a位置的值赋值给当前函数upvalues表中b位置的upvalue
        /// </summary>
        private static void SetUpvalue(Instructoin i, LuaState vm)
        {
            i.GetABC(out int a, out int b, out int c);
            a += vm.CurFrameBottom;
            Upvalue upvalue = vm.GetCurFrameUpvalue(b);

            vm.CopyAndPush(a);
            LuaDataUnion data = vm.Pop();

            upvalue.SetValue(data,vm);
        }

        /// <summary>
        /// 从当前函数upvalues表中获取b位置的table upvalue，将table[RK(c)]复制到栈中a位置
        /// </summary>
        private static void GetTabUp(Instructoin i, LuaState vm)
        {
            i.GetABC(out int a, out int b, out int c);
            a += vm.CurFrameBottom;
            Upvalue upvalue = vm.GetCurFrameUpvalue(b);

            vm.Push(upvalue.Value.Table);
            vm.PushRK(c);
            vm.PushTableValue(-2);
            vm.PopAndCopy(a);
            vm.Pop();
        }

        /// <summary>
        /// 从当前函数upvalues表中获取a位置的table upvalue，table[RK(b)] = RK(c)
        /// </summary>
        private static void SetTabUp(Instructoin i, LuaState vm)
        {
            i.GetABC(out int a, out int b, out int c);

            Upvalue upvalue = vm.GetCurFrameUpvalue(a);

            vm.Push(upvalue.Value.Table);
            vm.PushRK(b);
            vm.PushRK(c);
            vm.SetTableValue(-3);
            vm.Pop();
        }
    }

}

