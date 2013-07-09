using System;
using System.Net;
using System.Text;
using System.Collections.Generic;

namespace G.W.Y.Helper
{
    public static class StringConnect
    {
        #region ContactString ,SplitString
        public static string ContactString<T>(string contactor, params T[] objs)
        {
            StringBuilder names = new StringBuilder("");
            for (int i = 0; i < objs.Length; i++)
            {
                names.Append(objs[i].ToString());
                if (i != (objs.Length - 1))
                {
                    names.Append(contactor);
                }
            }

            return names.ToString();
        }

        /// <summary>
        /// 将列表元素拼接成由contactor分隔的字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="toString">将列表元素转换为字符串的委托</param>
        /// <param name="contactor">分隔符（可为空）</param>
        /// <returns></returns>
        //public static string ContractString<T>(this IEnumerable<T> source, Func<T, string> toString, string contactor)
        //{
        //    StringBuilder result = new StringBuilder();
        //    contactor = contactor ?? string.Empty;
        //    foreach (T item in source)
        //    {
        //        result.Append(toString(item));
        //        result.Append(contactor);
        //    }
        //    string resultStr = result.ToString();
        //    if (resultStr.EndsWith(contactor))
        //    {
        //        resultStr = resultStr.Remove(resultStr.Length - contactor.Length, contactor.Length);
        //    }
        //    return resultStr;
        //}

        public static string ContactString<T>(IList<T> objList, string contactor)
        {
            StringBuilder names = new StringBuilder("");
            for (int i = 0; i < objList.Count; i++)
            {
                names.Append(objList[i].ToString());
                if (i != (objList.Count - 1))
                {
                    names.Append(contactor);
                }
            }

            return names.ToString();
        }

        /// <summary>
        /// SplitStringToStrs 将目标字符串进行分割，并对分割值进行修整
        /// </summary>
        public static string[] SplitStringToStrs(string target, char separator)
        {
            if (target == null)
            {
                return null;
            }

            string[] temps = target.Split(separator);
            for (int i = 0; i < temps.Length; i++)
            {
                temps[i] = temps[i].Trim();
            }

            return temps;
        }
        #endregion
    }
}
