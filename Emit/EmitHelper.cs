using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace G.W.Y.Emit
{
   public class EmitHelper
    {
        private static MethodInfo GetTypeFromHandleMethodInfo;

        private static MethodInfo MakeByRefTypeMethodInfo;

        static EmitHelper()
        {
            EmitHelper.GetTypeFromHandleMethodInfo = typeof(Type).GetMethod("GetTypeFromHandle", new Type[]{typeof(RuntimeTypeHandle)});

            EmitHelper.MakeByRefTypeMethodInfo = typeof(Type).GetMethod("MakeByRefType");
        }
        /// <summary>
        /// GetParametersType 获取方法的参数类型
        /// </summary>       
        public static Type[] GetParametersType(MethodInfo method)
        {
            ParameterInfo[] parameters = method.GetParameters();
            Type[] array = new Type[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                array[i] = parameters[i].ParameterType;
            }
            return array;
        }
        /// <summary>
        /// LoadArg 加载方法的参数
        /// </summary>      
        public static void LoadArg(ILGenerator gen, int index)
        {
            switch (index)
            {
                case 0:
                    {
                        gen.Emit(OpCodes.Ldarg_0);
                        break;
                    }
                case 1:
                    {
                        gen.Emit(OpCodes.Ldarg_1);
                        break;
                    }
                case 2:
                    {
                        gen.Emit(OpCodes.Ldarg_2);
                        break;
                    }
                case 3:
                    {
                        gen.Emit(OpCodes.Ldarg_3);
                        break;
                    }
                default:
                    {
                        if (index < 128)
                        {
                            gen.Emit(OpCodes.Ldarg_S, index);
                        }
                        else
                        {
                            gen.Emit(OpCodes.Ldarg, index);
                        }
                        break;
                    }
            }
        }
        /// <summary>
        /// ConvertTopArgType 发射类型转换的代码，将堆栈顶部的参数转换为目标类型。
        /// </summary>        
        public static void ConvertTopArgType(ILGenerator ilGenerator, Type source, Type dest)
        {
            if (!dest.IsAssignableFrom(source))
            {
                if (dest.IsClass)
                {
                    if (source.IsValueType)
                    {
                        ilGenerator.Emit(OpCodes.Box, dest);
                    }
                    else
                    {
                        ilGenerator.Emit(OpCodes.Castclass, dest);
                    }
                }
                else
                {
                    if (source.IsValueType)
                    {
                        ilGenerator.Emit(OpCodes.Castclass, dest);
                    }
                    else
                    {
                        ilGenerator.Emit(OpCodes.Unbox_Any, dest);
                    }
                }
            }
        }
        /// <summary>
        /// GetGenericParameterNames 获取目标方法的泛型参数的名称。
        /// </summary>        
        public static string[] GetGenericParameterNames(MethodInfo method)
        {
            Type[] genericArguments = method.GetGenericArguments();
            string[] array = new string[genericArguments.Length];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = genericArguments[i].Name;
            }
            return array;
        }
        /// <summary>
        /// DefineDerivedMethodSignature 定义动态类中方法的签名，支持泛型方法。
        /// </summary>      
        public static MethodBuilder DefineDerivedMethodSignature(TypeBuilder typeBuilder, MethodInfo baseMethod)
        {
            Type[] parametersType = EmitHelper.GetParametersType(baseMethod);
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(baseMethod.Name, baseMethod.Attributes & ~MethodAttributes.Abstract, baseMethod.ReturnType, parametersType);
            if (baseMethod.IsGenericMethod)
            {
                Type[] genericArguments = baseMethod.GetGenericArguments();
                string[] genericParameterNames = EmitHelper.GetGenericParameterNames(baseMethod);
                GenericTypeParameterBuilder[] array = methodBuilder.DefineGenericParameters(genericParameterNames);
                for (int i = 0; i < array.Length; i++)
                {
                    array[i].SetInterfaceConstraints(genericArguments[i].GetGenericParameterConstraints());
                }
            }
            return methodBuilder;
        }
        /// <summary>
        /// Ldind 间接加载（即从地址加载[type类型]的对象）。不支持decimal类型
        /// </summary>       
        public static void Ldind(ILGenerator ilGenerator, Type type)
        {
            if (!type.IsValueType)
            {
                ilGenerator.Emit(OpCodes.Ldind_Ref);
            }
            else
            {
                if (type.IsEnum)
                {
                    Type underlyingType = Enum.GetUnderlyingType(type);
                    EmitHelper.Ldind(ilGenerator, underlyingType);
                }
                else
                {
                    if (type == typeof(long))
                    {
                        ilGenerator.Emit(OpCodes.Ldind_I8);
                    }
                    else
                    {
                        if (type == typeof(int))
                        {
                            ilGenerator.Emit(OpCodes.Ldind_I4);
                        }
                        else
                        {
                            if (type == typeof(short))
                            {
                                ilGenerator.Emit(OpCodes.Ldind_I2);
                            }
                            else
                            {
                                if (type == typeof(byte))
                                {
                                    ilGenerator.Emit(OpCodes.Ldind_U1);
                                }
                                else
                                {
                                    if (type == typeof(sbyte))
                                    {
                                        ilGenerator.Emit(OpCodes.Ldind_I1);
                                    }
                                    else
                                    {
                                        if (type == typeof(bool))
                                        {
                                            ilGenerator.Emit(OpCodes.Ldind_I1);
                                        }
                                        else
                                        {
                                            if (type == typeof(ulong))
                                            {
                                                ilGenerator.Emit(OpCodes.Ldind_I8);
                                            }
                                            else
                                            {
                                                if (type == typeof(uint))
                                                {
                                                    ilGenerator.Emit(OpCodes.Ldind_U4);
                                                }
                                                else
                                                {
                                                    if (type == typeof(ushort))
                                                    {
                                                        ilGenerator.Emit(OpCodes.Ldind_U2);
                                                    }
                                                    else
                                                    {
                                                        if (type == typeof(float))
                                                        {
                                                            ilGenerator.Emit(OpCodes.Ldind_R4);
                                                        }
                                                        else
                                                        {
                                                            if (type == typeof(double))
                                                            {
                                                                ilGenerator.Emit(OpCodes.Ldind_R8);
                                                            }
                                                            else
                                                            {
                                                                if (type == typeof(IntPtr))
                                                                {
                                                                    ilGenerator.Emit(OpCodes.Ldind_I4);
                                                                }
                                                                else
                                                                {
                                                                    if (type != typeof(UIntPtr))
                                                                    {
                                                                        throw new Exception(string.Format("The target type:{0} is not supported by EmitHelper.Ldind()", type));
                                                                    }
                                                                    ilGenerator.Emit(OpCodes.Ldind_I4);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Stind 间接存储（即存储[type类型]的对象地址）。将间接存储。不支持decimal类型
        /// </summary>      
        public static void Stind(ILGenerator ilGenerator, Type type)
        {
            if (!type.IsValueType)
            {
                ilGenerator.Emit(OpCodes.Stind_Ref);
            }
            else
            {
                if (type.IsEnum)
                {
                    Type underlyingType = Enum.GetUnderlyingType(type);
                    EmitHelper.Stind(ilGenerator, underlyingType);
                }
                else
                {
                    if (type == typeof(long))
                    {
                        ilGenerator.Emit(OpCodes.Stind_I8);
                    }
                    else
                    {
                        if (type == typeof(int))
                        {
                            ilGenerator.Emit(OpCodes.Stind_I4);
                        }
                        else
                        {
                            if (type == typeof(short))
                            {
                                ilGenerator.Emit(OpCodes.Stind_I2);
                            }
                            else
                            {
                                if (type == typeof(byte))
                                {
                                    ilGenerator.Emit(OpCodes.Stind_I1);
                                }
                                else
                                {
                                    if (type == typeof(sbyte))
                                    {
                                        ilGenerator.Emit(OpCodes.Stind_I1);
                                    }
                                    else
                                    {
                                        if (type == typeof(bool))
                                        {
                                            ilGenerator.Emit(OpCodes.Stind_I1);
                                        }
                                        else
                                        {
                                            if (type == typeof(ulong))
                                            {
                                                ilGenerator.Emit(OpCodes.Stind_I8);
                                            }
                                            else
                                            {
                                                if (type == typeof(uint))
                                                {
                                                    ilGenerator.Emit(OpCodes.Stind_I4);
                                                }
                                                else
                                                {
                                                    if (type == typeof(ushort))
                                                    {
                                                        ilGenerator.Emit(OpCodes.Stind_I2);
                                                    }
                                                    else
                                                    {
                                                        if (type == typeof(float))
                                                        {
                                                            ilGenerator.Emit(OpCodes.Stind_R4);
                                                        }
                                                        else
                                                        {
                                                            if (type == typeof(double))
                                                            {
                                                                ilGenerator.Emit(OpCodes.Stind_R8);
                                                            }
                                                            else
                                                            {
                                                                if (type == typeof(IntPtr))
                                                                {
                                                                    ilGenerator.Emit(OpCodes.Stind_I4);
                                                                }
                                                                else
                                                                {
                                                                    if (type != typeof(UIntPtr))
                                                                    {
                                                                        throw new Exception(string.Format("The target type:{0} is not supported by EmitHelper.Stind_ForValueType()", type));
                                                                    }
                                                                    ilGenerator.Emit(OpCodes.Stind_I4);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// LoadType 加载一个Type对象到堆栈。
        /// </summary>       
        public static void LoadType(ILGenerator gen, Type targetType)
        {
            if (targetType.IsByRef)
            {
                gen.Emit(OpCodes.Ldtoken, targetType.GetElementType());
                gen.Emit(OpCodes.Call, EmitHelper.GetTypeFromHandleMethodInfo);
                gen.Emit(OpCodes.Callvirt, EmitHelper.MakeByRefTypeMethodInfo);
            }
            else
            {
                gen.Emit(OpCodes.Ldtoken, targetType);
                gen.Emit(OpCodes.Call, EmitHelper.GetTypeFromHandleMethodInfo);
            }
        }
    }
}
