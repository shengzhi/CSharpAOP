using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace G.W.Y.Emit.DynamicProxy
{
    public class DynamicProxyFactory
    {
        private static AopProxyEmitter AopProxyEmitter = new AopProxyEmitter(true);

        public static TInterface CreateAopProxy<TInterface>(object origin, IAopInterceptor aopInterceptor)
        {
            TInterface result;
            lock (DynamicProxyFactory.AopProxyEmitter)
            {
                Type type = origin.GetType();
                Type type2 = DynamicProxyFactory.AopProxyEmitter.EmitProxyType<TInterface>(type);
                ConstructorInfo constructor = type2.GetConstructor(new Type[] { type, typeof(IAopInterceptor) });
                result = (TInterface)constructor.Invoke(new object[] { origin, aopInterceptor });
            }
            AopProxyEmitter.Save();
            return result;
        }

        public static TInterface CreateAopProxy<TInterface>(TInterface origin, IAopInterceptor aopInterceptor)
        {
            TInterface result;
            lock (DynamicProxyFactory.AopProxyEmitter)
            {
                Type typeFromHandle = typeof(TInterface);
                Type type = DynamicProxyFactory.AopProxyEmitter.EmitProxyType<TInterface>(typeFromHandle);
                ConstructorInfo constructor = type.GetConstructor(new Type[] { typeFromHandle, typeof(IAopInterceptor) });
                result = (TInterface)constructor.Invoke(new object[] { origin, aopInterceptor });
            }
            AopProxyEmitter.Save();
            return result;
        }

        public static object CreateAopProxy(Type proxyIntfaceType, object origin, IAopInterceptor aopInterceptor)
        {
            object result;
            lock (DynamicProxyFactory.AopProxyEmitter)
            {
                Type type = origin.GetType();
                Type type2 = DynamicProxyFactory.AopProxyEmitter.EmitProxyType(proxyIntfaceType, type);
                ConstructorInfo constructor = type2.GetConstructor(new Type[] { type, typeof(IAopInterceptor) });
                result = constructor.Invoke(new object[] { origin, aopInterceptor });
            }

            return result;
        }
    }
}
