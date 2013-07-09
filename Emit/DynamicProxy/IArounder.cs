using System;
using System.Collections.Generic;
using System.Text;

namespace G.W.Y.Emit.DynamicProxy
{
    /// <summary>
    ///  用于对截获的目标方法进行Around处理
    /// </summary>
    public interface IArounder
    {
        void BeginAround(InterceptedMethod method);
        void EndAround(object returnVal);
        /// <summary>
        /// OnException 目标方法抛出异常。
        /// </summary>       
        void OnException(InterceptedMethod method, Exception ee);
    }
}
