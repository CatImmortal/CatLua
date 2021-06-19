using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CatLua;
using System;
using UnityEngine.Profiling;
using System.Threading.Tasks;

public class Entry : MonoBehaviour
{
    public TextAsset main;
    private LuaState ls;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 30;

        LuaState ls = new LuaState(100);
        CSFunc.Init(ls);
        ls.LoadChunk(main.bytes, main.name, "b");
        ls.CallFunc(0, 0);

    }

    private Chunk TestUndump()
    {
        Chunk chunk = Chunk.Undump(main.bytes);
        return chunk;
    }

    private void TestInstruction(Chunk chunk)
    {
        for (int i = 0; i < chunk.MainFunc.Code.Length; i++)
        {
            uint code = chunk.MainFunc.Code[i];
            Instructoin instructoin = new Instructoin(code);
            Debug.Log(instructoin.OpType.ToString());
            int a;
            int b;
            int c;
            int ax;
            int bx;
            int sbx;
            switch (instructoin.OpMode)
            {

                case OpMode.IABC:
                    instructoin.GetABC(out a, out b, out c);
                    Debug.Log("a = " + a);
                    if (instructoin.ArgBType != OpArgType.N)
                    {
                        if (b > 0xff)
                        {
                            //常量表索引 按负数打印
                            Debug.Log("b = " + (-1 - (b & 0xff)));
                        }
                        else
                        {
                            Debug.Log("b = " + b);
                        }
                    }
                    if (instructoin.ArgCType != OpArgType.N)
                    {
                        if (c > 0xff)
                        {
                            Debug.Log("c = " + (-1 - (c & 0xff)));
                        }
                        else
                        {
                            Debug.Log("c = " + c);
                        }
                    }
                    break;
                case OpMode.IABx:
                    instructoin.GetABx(out a, out bx);
                    break;
                case OpMode.IAsBx:
                    instructoin.GetAsBx(out a, out sbx);
                    break;
                case OpMode.IAx:
                    instructoin.GetAx(out ax);
                    break;
            }
            Debug.Log("--------------------------------");
        }
    }
    private void TestLuaStateStack(FuncPrototype proto)
    {
        //LuaState ls = new LuaState(20,proto);

        //ls.Push(true);
        //Debug.Log(ls.ToString());


        //ls.Push(10);
        //Debug.Log(ls.ToString());


        //ls.Push();
        //Debug.Log(ls.ToString());


        //ls.Push("hello");
        //Debug.Log(ls.ToString());


        //ls.CopyAndPush(-4);
        //Debug.Log(ls.ToString());


        //ls.PopAndCopy(3);
        //Debug.Log(ls.ToString());

        //ls.SetTop(6);
        //Debug.Log(ls.ToString());

        //ls.Remove(-3);
        //Debug.Log(ls.ToString());

        //ls.SetTop(-5);
        //Debug.Log(ls.ToString());
    }

    private void TestLuaStateOperator(FuncPrototype proto)
    {
        //LuaState ls = new LuaState(20,proto);

        //ls.Push(1);
        //ls.Push("2.0");
        //ls.Push("3.0");
        //ls.Push(4.0);
        //Debug.Log(ls.ToString());

        //ls.Arith(ArithOpType.Add);
        //Debug.Log(ls.ToString());

        //ls.Arith(ArithOpType.BNot);
        //Debug.Log(ls.ToString());

        //ls.Len(2);
        //Debug.Log(ls.ToString());

        //ls.Concat(3);
        //Debug.Log(ls.ToString());

        //bool result = ls.Compare(1, 2, CompareOpType.Eq);
        //ls.Push(result);
        //Debug.Log(ls.ToString());
    }

    private void TestLuaVM(FuncPrototype proto)
    {
        //LuaState ls = new LuaState(proto.MaxStackSize + 8, proto);
        //ls.SetTop(proto.MaxStackSize);

        //while (true)
        //{

        //    int pc = ls.PC;
        //    if (pc + 1 == 8)
        //    {
        //        int i = 1;
        //    }
        //    Instructoin inst = new Instructoin(ls.Fetch());
        //    if (inst.OpCode != (byte)OpCodeType.Return)
        //    {
        //        inst.Execute(ls);
        //       Debug.Log(string.Format("[{0}] {1} {2}", pc + 1, inst.OpType, ls));
        //    }
        //    else
        //    {
        //        break;
        //    }
        //}
    }
}
