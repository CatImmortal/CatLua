using System.Collections;
using System.Collections.Generic;

namespace CatLua
{
    public enum FuncCallState
    {
        Ok,
        Yield,
        ErrRun,
        ErrSyntax,
        ErrMen,
        ErrGcmm,
        ErrErr,
        ErrFile,
    }

}
