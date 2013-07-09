using System;
using System.Collections.Generic;
using System.Text;

namespace G.W.Y.Emit.DynamicProxy
{
    /// <summary>
    ///  用于对截获的目标方法进行拦截处理
    /// </summary>
    public interface IAopInterceptor
    {
        bool PreProcess(InterceptedMethod method);
        void PostProcess(InterceptedMethod method, object returnVal);
        /// <summary>
        /// NewArounder 请注意返回值必不能为null。
        /// </summary>        
        IArounder NewArounder();
    }
}
