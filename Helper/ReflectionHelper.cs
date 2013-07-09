using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using G.W.Y.Helper;

namespace G.W.Y.Helper
{
    public static class ReflectionHelper
    {
        public class TypeLoadConfig
        {
            private bool copyToMemory = false;
            private bool loadAbstractType = false;
            private string targetFilePostfix = ".dll";
            /// <summary>
            /// CopyToMem 是否将程序集拷贝到内存后加载
            /// </summary>
            public bool CopyToMemory
            {
                get
                {
                    return this.copyToMemory;
                }
                set
                {
                    this.copyToMemory = value;
                }
            }
            /// <summary>
            /// LoadAbstractType 是否加载抽象类型
            /// </summary>
            public bool LoadAbstractType
            {
                get
                {
                    return this.loadAbstractType;
                }
                set
                {
                    this.loadAbstractType = value;
                }
            }
            /// <summary>
            /// TargetFilePostfix 搜索的目标程序集的后缀名
            /// </summary>
            public string TargetFilePostfix
            {
                get
                {
                    return this.targetFilePostfix;
                }
                set
                {
                    this.targetFilePostfix = value;
                }
            }

            public TypeLoadConfig()
            { 
            }

            public TypeLoadConfig(bool copyToMem, bool loadAbstract, string postfix)
            {
                this.copyToMemory = copyToMem;
                this.loadAbstractType = loadAbstract;
                this.targetFilePostfix = postfix;
            }
        }

        /// <summary>
        /// GetType  通过完全限定的类型名来加载对应的类型。typeAndAssName如"ESBasic.Filters.SourceFilter,ESBasic"。
        /// 如果为系统简单类型，则可以不带程序集名称。
        /// </summary>       
        public static Type GetType(string typeAndAssName)
        {
            string[] array = typeAndAssName.Split(new char[] { ',' });

            Type type;
            if (array.Length < 2)
            {
                type = Type.GetType(typeAndAssName);
            }
            else
            {
                type = ReflectionHelper.GetType(array[0].Trim(), array[1].Trim());
            }
            return type;
        }

        /// <summary>
        /// GetType 加载assemblyName程序集中的名为typeFullName的类型。assemblyName不用带扩展名，如果目标类型在当前程序集中，assemblyName传入null	
        /// </summary>		
        public static Type GetType(string typeFullName, string assemblyName)
        {
            Type result;
            if (assemblyName == null)
            {
                result = Type.GetType(typeFullName);
            }
            else
            {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                Assembly[] array = assemblies;
                for (int i = 0; i < array.Length; i++)
                {
                    Assembly assembly = array[i];
                    string[] array2 = assembly.FullName.Split(new char[]{','});
                    if (array2[0].Trim() == assemblyName.Trim())
                    {
                        result = assembly.GetType(typeFullName);
                        return result;
                    }
                }
                Assembly assembly2 = Assembly.Load(assemblyName);
                if (assembly2 != null)
                {
                    result = assembly2.GetType(typeFullName);
                }
                else
                {
                    result = null;
                }
            }
            return result;
        }
        public static string GetTypeFullName(Type t)
        {
            return t.FullName + "," + t.Assembly.FullName.Split(new char[]
			{
				','
			})[0];
        }
        /// <summary>
        /// LoadDerivedInstance 将程序集中所有继承自TBase的类型实例化
        /// </summary>
        /// <typeparam name="TBase">基础类型（或接口类型）</typeparam>
        /// <param name="asm">目标程序集</param>
        /// <returns>TBase实例列表</returns>
        public static IList<TBase> LoadDerivedInstance<TBase>(Assembly asm)
        {
            IList<TBase> list = new List<TBase>();
            Type typeFromHandle = typeof(TBase);
            Type[] types = asm.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                Type type = types[i];
                if (typeFromHandle.IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
                {
                    TBase item = (TBase)Activator.CreateInstance(type);
                    list.Add(item);
                }
            }
            return list;
        }
        /// <summary>
        /// LoadDerivedType 加载directorySearched目录下所有程序集中的所有派生自baseType的类型
        /// </summary>
        /// <typeparam name="baseType">基类（或接口）类型</typeparam>
        /// <param name="directorySearched">搜索的目录</param>
        /// <param name="searchChildFolder">是否搜索子目录中的程序集</param>
        /// <param name="config">高级配置，可以传入null采用默认配置</param>        
        /// <returns>所有从BaseType派生的类型列表</returns>
        public static IList<Type> LoadDerivedType(Type baseType, string directorySearched, bool searchChildFolder, ReflectionHelper.TypeLoadConfig config)
        {
            if (config == null)
            {
                config = new ReflectionHelper.TypeLoadConfig();
            }
            IList<Type> list = new List<Type>();
            if (searchChildFolder)
            {
                ReflectionHelper.LoadDerivedTypeInAllFolder(baseType, list, directorySearched, config);
            }
            else
            {
                ReflectionHelper.LoadDerivedTypeInOneFolder(baseType, list, directorySearched, config);
            }
            return list;
        }
        private static void LoadDerivedTypeInAllFolder(Type baseType, IList<Type> derivedTypeList, string folderPath, ReflectionHelper.TypeLoadConfig config)
        {
            ReflectionHelper.LoadDerivedTypeInOneFolder(baseType, derivedTypeList, folderPath, config);
            string[] directories = Directory.GetDirectories(folderPath);
            if (directories != null)
            {
                string[] array = directories;
                for (int i = 0; i < array.Length; i++)
                {
                    string folderPath2 = array[i];
                    ReflectionHelper.LoadDerivedTypeInAllFolder(baseType, derivedTypeList, folderPath2, config);
                }
            }
        }
        private static void LoadDerivedTypeInOneFolder(Type baseType, IList<Type> derivedTypeList, string folderPath, ReflectionHelper.TypeLoadConfig config)
        {
            string[] files = Directory.GetFiles(folderPath);
            string[] array = files;
            int i = 0;
            while (i < array.Length)
            {
                string text = array[i];
                if (config.TargetFilePostfix == null)
                {
                    goto IL_44;
                }
                if (text.EndsWith(config.TargetFilePostfix))
                {
                    goto IL_44;
                }
            IL_10C:
                i++;
                continue;
            IL_44:
                Assembly assembly = null;
                try
                {
                    if (config.CopyToMemory)
                    {
                        byte[] rawAssembly = FileHelper.ReadFileReturnBytes(text);
                        assembly = Assembly.Load(rawAssembly);
                    }
                    else
                    {
                        assembly = Assembly.LoadFrom(text);
                    }
                }
                catch (Exception var_4_74)
                {
                }
                if (assembly == null)
                {
                    goto IL_10C;
                }
                Type[] types = assembly.GetTypes();
                Type[] array2 = types;
                for (int j = 0; j < array2.Length; j++)
                {
                    Type type = array2[j];
                    if (type.IsSubclassOf(baseType) || baseType.IsAssignableFrom(type))
                    {
                        bool flag = config.LoadAbstractType || !type.IsAbstract;
                        if (flag)
                        {
                            derivedTypeList.Add(type);
                        }
                    }
                }
                goto IL_10C;
            }
        }
        /// <summary>
        /// SetProperty 如果list中的object具有指定的propertyName属性，则设置该属性的值为proValue
        /// </summary>		
        public static void SetProperty(IList<object> objs, string propertyName, object proValue)
        {
            object[] array = new object[]
			{
				proValue
			};
            foreach (object current in objs)
            {
                ReflectionHelper.SetProperty(current, propertyName, proValue);
            }
        }
        public static void SetProperty(object obj, string propertyName, object proValue)
        {
            ReflectionHelper.SetProperty(obj, propertyName, proValue, true);
        }
        /// <summary>
        /// SetProperty 如果object具有指定的propertyName属性，则设置该属性的值为proValue
        /// </summary>		
        public static void SetProperty(object obj, string propertyName, object proValue, bool ignoreError)
        {
            Type type = obj.GetType();
            PropertyInfo property = type.GetProperty(propertyName);
            if (property == null || !property.CanWrite)
            {
                if (!ignoreError)
                {
                    string message = string.Format("The setter of property named '{0}' not found in '{1}'.", propertyName, type);
                    throw new Exception(message);
                }
            }
            else
            {
                try
                {
                    proValue = TypeHelper.ChangeType(property.PropertyType, proValue);
                }
                catch
                {
                }
                object[] args = new object[]
				{
					proValue
				};
                type.InvokeMember(propertyName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty, null, obj, args);
            }
        }
        /// <summary>
        /// GetProperty 根据指定的属性名获取目标对象该属性的值
        /// </summary>
        public static object GetProperty(object obj, string propertyName)
        {
            Type type = obj.GetType();
            return type.InvokeMember(propertyName, BindingFlags.GetProperty, null, obj, null);
        }
        /// <summary>
        /// GetFieldValue 取得目标对象的指定field的值，field可以是private
        /// </summary>      
        public static object GetFieldValue(object obj, string fieldName)
        {
            Type type = obj.GetType();
            FieldInfo field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField);
            if (field == null)
            {
                string message = string.Format("The field named '{0}' not found in '{1}'.", fieldName, type);
                throw new Exception(message);
            }
            return field.GetValue(obj);
        }
        /// <summary>
        /// SetFieldValue 设置目标对象的指定field的值，field可以是private
        /// </summary>      
        public static void SetFieldValue(object obj, string fieldName, object val)
        {
            Type type = obj.GetType();
            FieldInfo field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetField);
            if (field == null)
            {
                string message = string.Format("The field named '{0}' not found in '{1}'.", fieldName, type);
                throw new Exception(message);
            }
            field.SetValue(obj, val);
        }
        /// <summary>
        /// CopyProperty 将source中的属性的值赋给target上同名的属性
        /// 使用CopyProperty可以方便的实现拷贝构造函数
        /// </summary>        
        public static void CopyProperty(object source, object target)
        {
            ReflectionHelper.CopyProperty(source, target, null);
        }
        /// <summary>
        /// CopyProperty 将source中的属性的值赋给target上想匹配的属性，匹配关系通过propertyMapItemList确定
        /// </summary>        
        public static void CopyProperty(object source, object target, IList<MapItem> propertyMapItemList)
        {
            Type type = source.GetType();
            Type type2 = target.GetType();
            PropertyInfo[] properties = type.GetProperties();
            if (propertyMapItemList != null)
            {
                foreach (MapItem current in propertyMapItemList)
                {
                    object property = ReflectionHelper.GetProperty(source, current.Source);
                    ReflectionHelper.SetProperty(target, current.Target, property);
                }
            }
            else
            {
                PropertyInfo[] array = properties;
                for (int i = 0; i < array.Length; i++)
                {
                    PropertyInfo propertyInfo = array[i];
                    if (propertyInfo.CanRead)
                    {
                        object property = ReflectionHelper.GetProperty(source, propertyInfo.Name);
                        ReflectionHelper.SetProperty(target, propertyInfo.Name, property);
                    }
                }
            }
        }
        /// <summary>
        /// GetAllMethods 获取接口的所有方法信息，包括继承的
        /// </summary>       
        public static IList<MethodInfo> GetAllMethods(params Type[] interfaceTypes)
        {
            for (int i = 0; i < interfaceTypes.Length; i++)
            {
                Type type = interfaceTypes[i];
                if (!type.IsInterface)
                {
                    throw new Exception("Target Type must be interface!");
                }
            }
            IList<MethodInfo> result = new List<MethodInfo>();
            for (int i = 0; i < interfaceTypes.Length; i++)
            {
                Type type = interfaceTypes[i];
                ReflectionHelper.DistillMethods(type, ref result);
            }
            return result;
        }
        private static void DistillMethods(Type interfaceType, ref IList<MethodInfo> methodList)
        {
            MethodInfo[] methods = interfaceType.GetMethods();
            for (int i = 0; i < methods.Length; i++)
            {
                MethodInfo methodInfo = methods[i];
                bool flag = false;
                foreach (MethodInfo current in methodList)
                {
                    if (current.Name == methodInfo.Name && current.ReturnType == methodInfo.ReturnType)
                    {
                        ParameterInfo[] parameters = current.GetParameters();
                        ParameterInfo[] parameters2 = methodInfo.GetParameters();
                        if (parameters.Length == parameters2.Length)
                        {
                            bool flag2 = true;
                            for (int j = 0; j < parameters.Length; j++)
                            {
                                if (parameters[j].ParameterType != parameters2[j].ParameterType)
                                {
                                    flag2 = false;
                                }
                            }
                            if (flag2)
                            {
                                flag = true;
                                break;
                            }
                        }
                    }
                }
                if (!flag)
                {
                    methodList.Add(methodInfo);
                }
            }
            Type[] interfaces = interfaceType.GetInterfaces();
            for (int i = 0; i < interfaces.Length; i++)
            {
                Type interfaceType2 = interfaces[i];
                ReflectionHelper.DistillMethods(interfaceType2, ref methodList);
            }
        }
        /// <summary>
        /// SearchGenericMethodInType 搜索指定类型定义的泛型方法，不包括继承的。
        /// </summary>       
        public static MethodInfo SearchGenericMethodInType(Type originType, string methodName, Type[] argTypes)
        {
            MethodInfo[] methods = originType.GetMethods();
            MethodInfo result;
            for (int i = 0; i < methods.Length; i++)
            {
                MethodInfo methodInfo = methods[i];
                if (methodInfo.ContainsGenericParameters && methodInfo.Name == methodName)
                {
                    bool flag = true;
                    ParameterInfo[] parameters = methodInfo.GetParameters();
                    if (parameters.Length == argTypes.Length)
                    {
                        for (int j = 0; j < parameters.Length; j++)
                        {
                            if (!parameters[j].ParameterType.IsGenericParameter)
                            {
                                if (parameters[j].ParameterType.IsGenericType)
                                {
                                    if (parameters[j].ParameterType.GetGenericTypeDefinition() != argTypes[j].GetGenericTypeDefinition())
                                    {
                                        flag = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    if (parameters[j].ParameterType != argTypes[j])
                                    {
                                        flag = false;
                                        break;
                                    }
                                }
                            }
                        }
                        if (flag)
                        {
                            result = methodInfo;
                            return result;
                        }
                    }
                }
            }
            result = null;
            return result;
        }
        /// <summary>
        /// SearchMethod 包括被继承的所有方法，也包括泛型方法。
        /// </summary>       
        public static MethodInfo SearchMethod(Type originType, string methodName, Type[] argTypes)
        {
            MethodInfo methodInfo = originType.GetMethod(methodName, argTypes);
            MethodInfo result;
            if (methodInfo != null)
            {
                result = methodInfo;
            }
            else
            {
                methodInfo = ReflectionHelper.SearchGenericMethodInType(originType, methodName, argTypes);
                if (methodInfo != null)
                {
                    result = methodInfo;
                }
                else
                {
                    Type baseType = originType.BaseType;
                    if (baseType != null)
                    {
                        while (baseType != typeof(object))
                        {
                            MethodInfo methodInfo2 = baseType.GetMethod(methodName, argTypes);
                            if (methodInfo2 != null)
                            {
                                result = methodInfo2;
                                return result;
                            }
                            methodInfo2 = ReflectionHelper.SearchGenericMethodInType(baseType, methodName, argTypes);
                            if (methodInfo2 != null)
                            {
                                result = methodInfo2;
                                return result;
                            }
                            baseType = baseType.BaseType;
                        }
                    }
                    if (originType.GetInterfaces() != null)
                    {
                        IList<MethodInfo> allMethods = ReflectionHelper.GetAllMethods(originType.GetInterfaces());
                        foreach (MethodInfo current in allMethods)
                        {
                            if (!(current.Name != methodName))
                            {
                                ParameterInfo[] parameters = current.GetParameters();
                                if (parameters.Length == argTypes.Length)
                                {
                                    bool flag = true;
                                    for (int i = 0; i < parameters.Length; i++)
                                    {
                                        if (parameters[i].ParameterType != argTypes[i])
                                        {
                                            flag = false;
                                            break;
                                        }
                                    }
                                    if (flag)
                                    {
                                        result = current;
                                        return result;
                                    }
                                }
                            }
                        }
                    }
                    result = null;
                }
            }
            return result;
        }
        public static string GetMethodFullName(MethodInfo method)
        {
            return string.Format("{0}.{1}()", method.DeclaringType, method.Name);
        }
    }

    public class MapItem
    {
        private string source;
        private string target;
        public string Source
        {
            get
            {
                return this.source;
            }
            set
            {
                this.source = value;
            }
        }
        public string Target
        {
            get
            {
                return this.target;
            }
            set
            {
                this.target = value;
            }
        }
        public MapItem()
        {
        }
        public MapItem(string theSource, string theTarget)
        {
            this.source = theSource;
            this.target = theTarget;
        }
    }
}
