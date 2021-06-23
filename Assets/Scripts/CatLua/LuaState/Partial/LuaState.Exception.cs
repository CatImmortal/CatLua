using System.Collections;
using System.Collections.Generic;
using System;

namespace CatLua
{
    public partial class LuaState
    {
        public int Error()
        {
            LuaDataUnion data = Pop();
            throw new Exception(data.ToString());
        }

        public FuncCallState PCall(int argsNum,int resultsNum,int msg)
        {
            FuncCallFrame frame = curFrame;

            try
            {
                CallFunc(argsNum, resultsNum);
            }
            catch (Exception e)
            {
                //不断弹出栈帧 直到到了调用栈帧
                while (curFrame != frame)
                {
                    SetTop(curFrame.Bottom - 1);
                    PopFuncCallFrame();
                }

                //将异常信息压入调用栈帧的栈顶
                Push(e.Message);
                return FuncCallState.ErrRun;
            }

            return FuncCallState.Ok;
        }
    }

}
