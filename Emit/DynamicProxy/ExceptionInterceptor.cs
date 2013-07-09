using System;
using System.Collections.Generic;
using System.Text;
using G.W.Y.IO.Logger;

namespace G.W.Y.Emit.DynamicProxy
{
    public class ExceptionInterceptor : IAopInterceptor, IArounder
    {
        private IExceptionLogger exceptionFilter;
        public ExceptionInterceptor()
        {
        }
        public ExceptionInterceptor(string logFilePath)
            : this(new ExceptionFileLogger(new FileAgileLogger(logFilePath)))
        {
        }
        public ExceptionInterceptor(IAgileLogger logger)
            : this(new ExceptionFileLogger(logger))
        {
        }
        public ExceptionInterceptor(IExceptionLogger logger)
        {
            this.exceptionFilter = logger;
        }
        public bool PreProcess(InterceptedMethod method)
        {
            return true;
        }
        public void PostProcess(InterceptedMethod method, object returnVal)
        {

        }
        public IArounder NewArounder()
        {
            return this;
        }
        public void BeginAround(InterceptedMethod method)
        {
        }
        public void EndAround(object returnVal)
        {
        }
        public void OnException(InterceptedMethod method, Exception ee)
        {
            string methodPath = string.Format("{0}.{1}", method.Target.GetType().ToString(), method.MethodName);
            this.exceptionFilter.Log(ee, methodPath, method.GenericTypes, method.ArgumentNames, method.ArgumentValues);
        }
    }
}
