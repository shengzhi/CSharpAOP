using System;
using System.Collections.Generic;
using System.Text;

namespace G.W.Y.Emit
{
    public sealed class InterceptedMethod
    {
        private string methodName;
        private object target;
        private string[] argumentNames;
        private object[] argumentValues;
        private Type[] genericTypes = null;
        /// <summary>
        /// MethodName 被截获的目标方法
        /// </summary>
        public string MethodName
        {
            get
            {
                return this.methodName;
            }
            set
            {
                this.methodName = value;
            }
        }
        /// <summary>
        /// Target 被截获的方法需要在哪个对象上调用。
        /// </summary>
        public object Target
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
        /// <summary>
        /// 调用被截获的方法的参数名称
        /// </summary>
        public string[] ArgumentNames
        {
            get
            {
                return this.argumentNames;
            }
            set
            {
                this.argumentNames = value;
            }
        }
        /// <summary>
        /// 调用被截获的方法的参数值
        /// </summary>
        public object[] ArgumentValues
        {
            get
            {
                return this.argumentValues;
            }
            set
            {
                this.argumentValues = value;
            }
        }
        /// <summary>
        /// 如果目标方法为泛型方法，则该属性记录泛型参数的类型。
        /// </summary>
        public Type[] GenericTypes
        {
            get
            {
                return this.genericTypes;
            }
            set
            {
                this.genericTypes = value;
            }
        }
        public InterceptedMethod()
        {
        }
        public InterceptedMethod(object _target, string _method, Type[] _genericTypes, string[] paraNames, object[] paraValues)
        {
            this.target = _target;
            this.methodName = _method;
            this.genericTypes = _genericTypes;
            this.argumentNames = paraNames;
            this.argumentValues = paraValues;
        }
    }
}
