﻿using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    public partial class GenFuncInfo
    {
        /// <summary>
        /// 该函数的所有upvalue
        /// </summary>
        public Dictionary<string, GenUpvalueInfo> UpvalueDict = new Dictionary<string, GenUpvalueInfo>();

        /// <summary>
        /// 返回name绑定的upvalue索引
        /// </summary>
        public int IndexOfUpvalue(string name)
        {
            //name已经和upvalue绑定了 直接返回
            if (UpvalueDict.TryGetValue(name, out GenUpvalueInfo upvalue))
            {
                return upvalue.Index;
            }

            //name没和upvalue绑定 尝试绑定

            if (Parent != null)
            {
                //尝试从直接外围函数的局部变量进行绑定
                if (Parent.activeLocalVarDict.TryGetValue(name, out GenLocalVarInfo localVar))
                {
                    int index = UpvalueDict.Count;
                    GenUpvalueInfo info = new GenUpvalueInfo(localVar.Slot, -1, index);
                    UpvalueDict.Add(name, info);

                    localVar.Captured = true;
                    return index;
                }

                //尝试从直接外围函数的Upvalue表进行绑定
                int upvalueIndex = Parent.IndexOfUpvalue(name);
                if (upvalueIndex >= 0)
                {
                    int index = UpvalueDict.Count;
                    GenUpvalueInfo info = new GenUpvalueInfo(-1, upvalueIndex, index);
                    UpvalueDict.Add(name, info);
                    return index;
                }
            }

            //绑定失败
            return -1;
        }
   
        /// <summary>
        /// 闭合处于开放状态的upvalue
        /// </summary>
        public void CloseOpenUpvalue()
        {
            int a = GetJmpArgA();
            if (a > 0)
            {
                //栈上有upvalue 用jump指令的a参数来处理upvalue的关闭
                EmitJmp(a, 0);
            }
        }
    }

}
