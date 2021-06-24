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
                //不断弹出栈帧 直到到了初始的调用栈帧
                while (curFrame != frame)
                {
                    CloseUpvalue(curFrame.Bottom);
                    SetTop(curFrame.Bottom - 1);
                    PopFuncCallFrame();
                }

                //将异常信息作为pcall的返回值压入调用栈帧的栈顶
                CallFrameReturnResultNum = 1;
                Push(e.Message);
                return FuncCallState.ErrRun;
            }

            return FuncCallState.Ok;
        }
    }

}
