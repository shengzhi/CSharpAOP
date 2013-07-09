using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using G.W.Y.Helper;

namespace G.W.Y.Emit.DynamicProxy
{
    public class AopProxyEmitter
    {
        #region Field
        private bool saveFile = false;
        private string assemblyName = "DynamicProxyAssembly";
        private string theFileName;
        private AssemblyBuilder dynamicAssembly;
        protected ModuleBuilder moduleBuilder;
        private IDictionary<string, Type> proxyTypeDictionary = new Dictionary<string, Type>();
        #endregion

        public AopProxyEmitter(bool save)
        {
            this.saveFile = save;
            this.theFileName = this.assemblyName + ".dll";
            AssemblyBuilderAccess access = this.saveFile ? AssemblyBuilderAccess.RunAndSave : AssemblyBuilderAccess.Run;
            this.dynamicAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(this.assemblyName), access);
            if (this.saveFile)
            {
                this.moduleBuilder = this.dynamicAssembly.DefineDynamicModule("MainModule", this.theFileName);
            }
            else
            {
                this.moduleBuilder = this.dynamicAssembly.DefineDynamicModule("MainModule");
            }
        }

        public Type EmitProxyType(Type interfaceType, Type originType)
        {
            if (!interfaceType.IsInterface)
            {
                throw new Exception("TInterface must be interface type !");
            }
            if (interfaceType.ContainsGenericParameters)
            {
                throw new Exception("TInterface can't be generic !");
            }
            string dynamicTypeName = this.GetDynamicTypeName(interfaceType, originType);
            Type result;
            if (this.proxyTypeDictionary.ContainsKey(dynamicTypeName))
            {
                result = this.proxyTypeDictionary[dynamicTypeName];
            }
            else
            {
                Type type = this.DoEmitProxyType(interfaceType, originType);
                this.proxyTypeDictionary.Add(dynamicTypeName, type);
                result = type;
            }
            return result;
        }

        public Type EmitProxyType<TInterface>(Type originType)
        {
            Type typeFromHandle = typeof(TInterface);
            return this.EmitProxyType(typeFromHandle, originType);
        }
        /// <summary>
        /// 获取要动态生成的类型的名称。注意，子类一定要使用本方法来得到动态类型的名称。
        /// </summary>    
        private string GetDynamicTypeName(Type interfaceType, Type originType)
        {
            return string.Format("{0}.{1}_{2}_AopProxy", this.assemblyName, originType.ToString(), interfaceType.ToString());
        }

        public void Save()
        {
            if (this.saveFile)
            {
                this.dynamicAssembly.Save(this.theFileName);
            }
        }

        private Type DoEmitProxyType(Type interfaceType, Type originType)
        {
            string dynamicTypeName = this.GetDynamicTypeName(interfaceType, originType);
            TypeBuilder typeBuilder = this.moduleBuilder.DefineType(dynamicTypeName, TypeAttributes.Public);
            typeBuilder.SetParent(typeof(MarshalByRefObject));
            typeBuilder.AddInterfaceImplementation(interfaceType);
            FieldBuilder fieldBuilder = typeBuilder.DefineField("target", originType, FieldAttributes.Private);
            FieldBuilder fieldBuilder2 = typeBuilder.DefineField("aopInterceptor", typeof(IAopInterceptor), FieldAttributes.Private);
            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { originType, typeof(IAopInterceptor) });
            ILGenerator iLGenerator = constructorBuilder.GetILGenerator();
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));//调用构造函数，此行和上一行为构造函数实现的基本格式
            iLGenerator.Emit(OpCodes.Ldarg_0);//this
            iLGenerator.Emit(OpCodes.Ldarg_1);//构造函数第一个参数值
            iLGenerator.Emit(OpCodes.Stfld, fieldBuilder);
            iLGenerator.Emit(OpCodes.Ldarg_0);//this
            iLGenerator.Emit(OpCodes.Ldarg_2);//构造函数第二个参数值
            iLGenerator.Emit(OpCodes.Stfld, fieldBuilder2);
            iLGenerator.Emit(OpCodes.Ret);
            this.EmitInitializeLifetimeServiceMethod(typeBuilder);
            foreach (MethodInfo current in ReflectionHelper.GetAllMethods(new Type[] { interfaceType }))
            {
                this.EmitMethod(originType, typeBuilder, current, fieldBuilder, fieldBuilder2);
            }
            return typeBuilder.CreateType();
        }

        private void EmitInitializeLifetimeServiceMethod(TypeBuilder typeBuilder)
        {
            MethodInfo methodInfo = ReflectionHelper.SearchMethod(typeof(MarshalByRefObject), "InitializeLifetimeService", Type.EmptyTypes);
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(methodInfo.Name, MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Virtual, methodInfo.ReturnType, Type.EmptyTypes);
            ILGenerator iLGenerator = methodBuilder.GetILGenerator();
            iLGenerator.Emit(OpCodes.Ldnull);
            iLGenerator.Emit(OpCodes.Ret);
        }

        private void EmitMethod(Type originType, TypeBuilder typeBuilder, MethodInfo baseMethod, FieldBuilder targetField, FieldBuilder aopInterceptorField)
        {
            Type[] parametersType = EmitHelper.GetParametersType(baseMethod);
            MethodInfo methodInfo = ReflectionHelper.SearchMethod(originType, baseMethod.Name, parametersType);
            MethodBuilder methodBuilder = EmitHelper.DefineDerivedMethodSignature(typeBuilder, baseMethod);
            ILGenerator iLGenerator = methodBuilder.GetILGenerator();
            LocalBuilder local = iLGenerator.DeclareLocal(typeof(Type[]));//定义方法内部临时变量

            //定义变量并给其赋值
            //LocalBuilder temp = iLGenerator.DeclareLocal(typeof(string));
            //iLGenerator.Emit(OpCodes.Ldstr, methodInfo.Name);
            //iLGenerator.Emit(OpCodes.Stloc, temp);

            if (methodInfo.IsGenericMethod)
            {
                Type[] genericArguments = methodInfo.GetGenericArguments();
                iLGenerator.Emit(OpCodes.Ldc_I4, genericArguments.Length);
                iLGenerator.Emit(OpCodes.Newarr, typeof(Type));
                iLGenerator.Emit(OpCodes.Stloc, local);
                for (int i = 0; i < genericArguments.Length; i++)
                {
                    iLGenerator.Emit(OpCodes.Ldloc, local);
                    iLGenerator.Emit(OpCodes.Ldc_I4, i);
                    EmitHelper.LoadType(iLGenerator, genericArguments[i]);
                    iLGenerator.Emit(OpCodes.Stelem_Ref);
                }
            }
            ParameterInfo[] parameters = methodInfo.GetParameters();
            LocalBuilder local2 = iLGenerator.DeclareLocal(typeof(string[]));
            LocalBuilder local3 = iLGenerator.DeclareLocal(typeof(object[]));
            if (parameters.Length > 0)
            {
                iLGenerator.Emit(OpCodes.Ldc_I4, parameters.Length);
                iLGenerator.Emit(OpCodes.Newarr, typeof(string));
                iLGenerator.Emit(OpCodes.Stloc, local2);
                for (int i = 0; i < parameters.Length; i++)
                {
                    iLGenerator.Emit(OpCodes.Ldloc, local2);
                    iLGenerator.Emit(OpCodes.Ldc_I4, i);
                    iLGenerator.Emit(OpCodes.Ldstr, parameters[i].Name);
                    iLGenerator.Emit(OpCodes.Stelem_Ref);
                }
                iLGenerator.Emit(OpCodes.Ldc_I4, parameters.Length);
                iLGenerator.Emit(OpCodes.Newarr, typeof(object));
                iLGenerator.Emit(OpCodes.Stloc, local3);
                for (int i = 0; i < parameters.Length; i++)
                {
                    iLGenerator.Emit(OpCodes.Ldloc, local3);
                    iLGenerator.Emit(OpCodes.Ldc_I4, i);
                    iLGenerator.Emit(OpCodes.Ldarg, i + 1);
                    if (parameters[i].ParameterType.IsByRef)
                    {
                        EmitHelper.Ldind(iLGenerator, parameters[i].ParameterType);
                        iLGenerator.Emit(OpCodes.Box, parameters[i].ParameterType.GetElementType());
                    }
                    else
                    {
                        if (parameters[i].ParameterType.IsValueType)
                        {
                            iLGenerator.Emit(OpCodes.Box, parameters[i].ParameterType);
                        }
                    }
                    iLGenerator.Emit(OpCodes.Stelem_Ref);
                }
            }

            /*Newobj 堆栈转换行为依次为：
            将从 arg1 到 argn 的参数按顺序推送到堆栈上。
            从堆栈中弹出从 argn 到 arg1 的参数并将它们传递到 ctor 以用于对象创建。
            将对新对象的引用推送到堆栈上。

            //从构造函数的角度，未初始化的对象是参数 0 和其他参数一样遵循一定的顺序被传递到 newobj*/

            /*Callvirt堆栈转换行为依次为：
             将对象引用 obj 推送到堆栈上
             将从 arg1 到 argN 的方法参数推送到堆栈上。
             从堆栈中弹出从 arg1 到 argN 的方法参数和对象引用 obj；通过这些参数执行方法调用并将控制转移到由方法元数据标记引用的 obj 中的方法。 
             完成后，被调用方方法生成返回值并将其发送给调用方。
             将返回值推送到堆栈上。*/

            LocalBuilder local4 = iLGenerator.DeclareLocal(typeof(InterceptedMethod));
            ConstructorInfo ctor = typeof(InterceptedMethod).GetConstructor(new Type[] { typeof(object), typeof(string), typeof(Type[]), typeof(string[]), typeof(object[]) });
            MethodInfo method = typeof(IAopInterceptor).GetMethod("PreProcess", new Type[] { typeof(InterceptedMethod) });
            iLGenerator.Emit(OpCodes.Ldarg_0);//将当前对象引用放到堆栈上（this） 除静态函数外,其他所有的函数的实际的参数都会在最左边多一位,就是this.所以这里实际的参数应该是从1开始的.
            iLGenerator.Emit(OpCodes.Ldfld, aopInterceptorField);//找到对应字段，并作为第1个参数放到堆栈上
            iLGenerator.Emit(OpCodes.Ldstr, methodInfo.Name);//作为第2个参数放到堆栈上
            iLGenerator.Emit(OpCodes.Ldloc, local);//作为第3个参数放到堆栈上
            iLGenerator.Emit(OpCodes.Ldloc, local2);//作为第4个参数放到堆栈上
            iLGenerator.Emit(OpCodes.Ldloc, local3);//作为第5个参数放到堆栈上
            iLGenerator.Emit(OpCodes.Newobj, ctor);//new InterceptedMethod 对象出来
            iLGenerator.Emit(OpCodes.Stloc, local4);//将对象引用指向local4

            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Ldfld, aopInterceptorField);
            iLGenerator.Emit(OpCodes.Ldloc, local4);
            iLGenerator.Emit(OpCodes.Callvirt, method);

            LocalBuilder local5 = iLGenerator.DeclareLocal(typeof(IArounder));
            MethodInfo method2 = typeof(IAopInterceptor).GetMethod("NewArounder");
            MethodInfo method3 = typeof(IArounder).GetMethod("BeginAround");

            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Ldfld, aopInterceptorField);
            iLGenerator.Emit(OpCodes.Callvirt, method2);
            iLGenerator.Emit(OpCodes.Stloc, local5);
            iLGenerator.Emit(OpCodes.Ldloc, local5);
            iLGenerator.Emit(OpCodes.Ldloc, local4);
            iLGenerator.Emit(OpCodes.Callvirt, method3);

            LocalBuilder localBuilder = null;
            if (methodInfo.ReturnType != typeof(void))
            {
                localBuilder = iLGenerator.DeclareLocal(methodInfo.ReturnType);
            }
            LocalBuilder local6 = iLGenerator.DeclareLocal(typeof(Exception));
            Label label = iLGenerator.BeginExceptionBlock();
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Ldfld, targetField);
            int num = 0;
            ParameterInfo[] parameters2 = methodInfo.GetParameters();
            for (int j = 0; j < parameters2.Length; j++)
            {
                ParameterInfo parameterInfo = parameters2[j];
                EmitHelper.LoadArg(iLGenerator, num + 1);
                EmitHelper.ConvertTopArgType(iLGenerator, parametersType[num], parameterInfo.ParameterType);
                num++;
            }
            iLGenerator.Emit(OpCodes.Callvirt, methodInfo);
            if (localBuilder != null)
            {
                iLGenerator.Emit(OpCodes.Stloc, localBuilder);
            }
            iLGenerator.BeginCatchBlock(typeof(Exception));
            MethodInfo method4 = typeof(IArounder).GetMethod("OnException");

            iLGenerator.Emit(OpCodes.Stloc, local6);
            iLGenerator.Emit(OpCodes.Ldloc, local5);
            iLGenerator.Emit(OpCodes.Ldloc, local4);
            iLGenerator.Emit(OpCodes.Ldloc, local6);
            iLGenerator.Emit(OpCodes.Callvirt, method4);
            iLGenerator.Emit(OpCodes.Nop);
            iLGenerator.Emit(OpCodes.Rethrow);
            iLGenerator.Emit(OpCodes.Nop);
            iLGenerator.EndExceptionBlock();
            MethodInfo method5 = typeof(IArounder).GetMethod("EndAround");
            iLGenerator.Emit(OpCodes.Ldloc, local5);
            if (localBuilder != null)
            {
                if (methodInfo.ReturnType.IsValueType)
                {
                    iLGenerator.Emit(OpCodes.Ldloc, localBuilder);
                    iLGenerator.Emit(OpCodes.Box, methodInfo.ReturnType);
                }
                else
                {
                    iLGenerator.Emit(OpCodes.Ldloc, localBuilder);
                }
            }
            else
            {
                iLGenerator.Emit(OpCodes.Ldnull);
            }
            iLGenerator.Emit(OpCodes.Callvirt, method5);
            iLGenerator.Emit(OpCodes.Nop);
            MethodInfo method6 = typeof(IAopInterceptor).GetMethod("PostProcess", new Type[] { typeof(InterceptedMethod), typeof(object) });
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Ldfld, aopInterceptorField);
            iLGenerator.Emit(OpCodes.Ldloc, local4);
            if (localBuilder != null)
            {
                if (methodInfo.ReturnType.IsValueType)
                {
                    iLGenerator.Emit(OpCodes.Ldloc, localBuilder);
                    iLGenerator.Emit(OpCodes.Box, methodInfo.ReturnType);
                }
                else
                {
                    iLGenerator.Emit(OpCodes.Ldloc, localBuilder);
                }
            }
            else
            {
                iLGenerator.Emit(OpCodes.Ldnull);
            }
            iLGenerator.Emit(OpCodes.Callvirt, method6);
            iLGenerator.Emit(OpCodes.Nop);
            if (localBuilder != null)
            {
                iLGenerator.Emit(OpCodes.Ldloc, localBuilder);
            }
            iLGenerator.Emit(OpCodes.Ret);
            typeBuilder.DefineMethodOverride(methodBuilder, baseMethod);
        }



    }
}


