using System;
using System.Collections.Generic;
using System.Text;

namespace G.W.Y.Helper
{
    public static class TypeHelper
    {
        #region IsSimpleType
        /// <summary>
        /// IsSimpleType 是否为简单类型：数值、字符、字符串、日期、布尔、枚举、Type
        /// </summary>      
        public static bool IsSimpleType(Type t)
        {
            if (TypeHelper.IsNumbericType(t))
            {
                return true;
            }

            if (t == typeof(char))
            {
                return true;
            }            

            if (t == typeof(string))
            {
                return true;
            }
            

            if (t == typeof(bool))
            {
                return true;
            }
            

            if (t == typeof(DateTime))
            {
                return true;
            }
            
            if (t == typeof(Type))
            {
                return true;
            }           

            if (t.IsEnum)
            {
                return true;
            }

            return false;
        } 
        #endregion

        #region IsNumbericType 是否为数值类型
        public static bool IsNumbericType(Type destDataType)
        {
            if ((destDataType == typeof(int)) || (destDataType == typeof(uint)) || (destDataType == typeof(double))
                || (destDataType == typeof(short)) || (destDataType == typeof(ushort)) || (destDataType == typeof(decimal))
                || (destDataType == typeof(long)) || (destDataType == typeof(ulong)) || (destDataType == typeof(float))
                || (destDataType == typeof(byte)) || (destDataType == typeof(sbyte)))
            {
                return true;
            }

            return false;
        }
        #endregion

        #region IsIntegerCompatibleType 是否为整数兼容类型
        public static bool IsIntegerCompatibleType(Type destDataType)
        {
            if ((destDataType == typeof(int)) || (destDataType == typeof(uint)) || (destDataType == typeof(short)) || (destDataType == typeof(ushort)) 
                || (destDataType == typeof(long)) || (destDataType == typeof(ulong)) || (destDataType == typeof(byte)) || (destDataType == typeof(sbyte)))
            {
                return true;
            }

            return false;
        }
        #endregion

        #region GetClassSimpleName
        /// <summary>
        /// GetClassSimpleName 获取class的声明名称，如 Person
        /// </summary>      
        public static string GetClassSimpleName(Type t)
        {
            string[] parts = t.ToString().Split('.');
            return parts[parts.Length - 1].ToString();
        } 
        #endregion

        #region IsFixLength
        public static bool IsFixLength(Type destDataType)
        {
            if (TypeHelper.IsNumbericType(destDataType))
            {
                return true;
            }

            if (destDataType == typeof(byte[]))
            {
                return true;
            }

            if ((destDataType == typeof(DateTime)) || (destDataType == typeof(bool)))
            {
                return true;
            }

            return false;
        }
        #endregion

        #region GetDefaultValue
        public static object GetDefaultValue(Type destType)
        {
            if (TypeHelper.IsNumbericType(destType))
            {
                return 0;
            }

            if (destType == typeof(string))
            {
                return "";
            }

            if (destType == typeof(bool))
            {
                return false;
            }

            if (destType == typeof(DateTime))
            {
                return DateTime.Now;
            }

            if (destType == typeof(Guid))
            {
                return System.Guid.NewGuid();
            }

            if (destType == typeof(TimeSpan))
            {
                return System.TimeSpan.Zero;
            }

            return null;
        } 
        #endregion

        #region GetDefaultValueString
        public static string GetDefaultValueString(Type destType)
        {
            if (TypeHelper.IsNumbericType(destType))
            {
                return "0";
            }

            if (destType == typeof(string))
            {
                return "\"\"";
            }

            if (destType == typeof(bool))
            {
                return "false";
            }

            if (destType == typeof(DateTime))
            {
                return "DateTime.Now";
            }

            if (destType == typeof(Guid))
            {
                return "System.Guid.NewGuid()";
            }

            if (destType == typeof(TimeSpan))
            {
                return "System.TimeSpan.Zero";
            }


            return "null";
        }
        #endregion

        #region GetTypeRegularName
        /// <summary>
        /// GetTypeRegularName 获取类型的完全名称
        /// </summary>      
        public static string GetTypeRegularName(Type destType)
        {
            string assName = destType.Assembly.FullName.Split(',')[0];

            return string.Format("{0},{1}", destType.ToString(), assName);

        }

        public static string GetTypeRegularNameOf(object obj)
        {
            Type destType = obj.GetType();
            return TypeHelper.GetTypeRegularName(destType);
        } 
        #endregion              

        #region ChangeType
        public static object ChangeType(Type targetType, object val)
        {
            object result;
            if (val == null)
            {
                result = null;
            }
            else
            {
                if (targetType.IsAssignableFrom(val.GetType()))
                {
                    result = val;
                }
                else
                {
                    if (targetType == val.GetType())
                    {
                        result = val;
                    }
                    else
                    {
                        if (targetType == typeof(bool))
                        {
                            if (val.ToString() == "0")
                            {
                                result = false;
                                return result;
                            }
                            if (val.ToString() == "1")
                            {
                                result = true;
                                return result;
                            }
                        }
                        if (targetType.IsEnum)
                        {
                            int num = 0;
                            if (!int.TryParse(val.ToString(), out num))
                            {
                                result = Enum.Parse(targetType, val.ToString());
                            }
                            else
                            {
                                result = val;
                            }
                        }
                        else
                        {
                            if (targetType == typeof(Type))
                            {
                                result = ReflectionHelper.GetType(val.ToString());
                            }
                            else
                            {
                                if (targetType == typeof(IComparable))
                                {
                                    result = val;
                                }
                                else
                                {
                                    result = Convert.ChangeType(val, targetType);
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }
        #endregion
    }
}
