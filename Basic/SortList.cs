using System;
using System.Net;
using System.Collections.Generic;

namespace G.W.Y.Basic
{
    public class SortList<T> : List<T> where T : IComparable
    {
        public void AddSorted(T item)
        {
            if (this.Count == 0)
            {
                this.Add(item);
                return;
            }
            //BinarySearch Instructrcation
            //如果找到 item，则为已排序的 List 中 item 的从零开始的索引；
            //否则为一个负数，该负数是大于 item 的第一个元素的索引的按位求补。
            //如果没有更大的元素，则为 Count 的按位求补 Form msdn 2010.5.11
            int position = this.BinarySearch(item);
            if (position < 0)
            {
                position = ~position;
            }
            this.Insert(position, item);
        }

        public void ModifySorted(T item, int index)
        {
            this.RemoveAt(index);
            int position = this.BinarySearch(item);
            if (position < 0)
            {
                position = ~position;
            }
            this.Insert(position, item);
        }
    }
}
