using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CatLua;
public class Entry : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //TextAsset main = Resources.Load<TextAsset>("Main");
        //Chunk chunk = Chunk.Undump(main.bytes);


        //for (int i = 0; i < chunk.MainFunc.Code.Length; i++)
        //{
        //    uint code = chunk.MainFunc.Code[i];
        //    Instructoin instructoin = new Instructoin(code);
        //    Debug.Log(instructoin.OpType.ToString());
        //    int a;
        //    int b;
        //    int c;
        //    int ax;
        //    int bx;
        //    int sbx;
        //    switch (instructoin.OpMode)
        //    {

        //        case OpMode.IABC:
        //            instructoin.GetABC(out a,out b,out c);
        //            Debug.Log("a = " + a);
        //            if (instructoin.ArgBType != OpArgType.N)
        //            {
        //                if (b > 0xff)
        //                {
        //                    //常量表索引 按负数打印
        //                    Debug.Log("b = " + (-1 - (b & 0xff)));
        //                }
        //                else
        //                {
        //                    Debug.Log("b = " + b);
        //                }
        //            }
        //            if (instructoin.ArgCType != OpArgType.N)
        //            {
        //                if (c > 0xff)
        //                {
        //                    Debug.Log("c = " + (-1 - (c & 0xff)));
        //                }
        //                else
        //                {
        //                    Debug.Log("c = " + c);
        //                }
        //            }
        //            break;
        //        case OpMode.IABx:
        //            instructoin.GetABx(out a, out bx);
        //            break;
        //        case OpMode.IAsBx:
        //            instructoin.GetAsBx(out a,out sbx);
        //            break;
        //        case OpMode.IAx:
        //            instructoin.GetAx(out ax);
        //            break;
        //    }
        //    Debug.Log("--------------------------------");
        //}

        TestLuaState();
    }

    private void TestLuaState()
    {
        LuaState ls = new LuaState();

        ls.Push(true);
        Debug.Log(ls.ToString());


        ls.Push(10);
        Debug.Log(ls.ToString());


        ls.Push();
        Debug.Log(ls.ToString());


        ls.Push("hello");
        Debug.Log(ls.ToString());


        ls.CopyToTop(-4);
        Debug.Log(ls.ToString());


        ls.PopAndCopy(3);
        Debug.Log(ls.ToString());

        ls.SetTop(6);
        Debug.Log(ls.ToString());

        ls.Remove(-3);
        Debug.Log(ls.ToString());

        ls.SetTop(-5);
        Debug.Log(ls.ToString());
    }


}
