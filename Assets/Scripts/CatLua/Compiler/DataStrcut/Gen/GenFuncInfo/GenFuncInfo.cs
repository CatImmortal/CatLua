using System.Collections;
using System.Collections.Generic;
using System;

namespace CatLua
{

    /// <summary>
    /// 函数信息
    /// </summary>
    public partial class GenFuncInfo
    {
        public GenFuncInfo(GenFuncInfo parent,FuncDefExp exp)
        {
            Parent = parent;
            IsVararg = exp.IsVararg;
            NumParams = exp.ParamList.Length;
        }

        

        /// <summary>
        /// 直接外围函数
        /// </summary>
        public GenFuncInfo Parent;

        /// <summary>
        /// 子函数
        /// </summary>
        public List<GenFuncInfo> Children = new List<GenFuncInfo>();

        /// <summary>
        /// 固定参数数量
        /// </summary>
        public int NumParams;

        /// <summary>
        /// 是否有变长参数
        /// </summary>
        public bool IsVararg;


      
    }
}

