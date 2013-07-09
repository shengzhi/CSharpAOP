using System;
using System.Net;

namespace G.W.Y.Helper
{
    public static class SortHelper
    {
        /// <summary>
        /// 插入排序算法就像我们一边抓扑克牌，一边排列扑克牌的顺序那样。
        ///假设输入是：{ 4, 2, 5, 10, 7 }
        ///在这里，我们定义“有序”的集合是一个“元素按从小到大的顺序排列”的集合。
        ///输入自然是无序的。
        ///第一步，我们要把输入分成2个子集，一个是有序的（我们给它个名字，叫L好了），
        ///一个是无序的（我们叫它R吧）。初始时，我们不妨让L={ 4 }, R = { 2, 5, 10, 7 }，
        ///这样的话由于L里面只有一个元素，本身就是有序的，我们都不用再做什么处理了。
        ///接下来，我们每次从R里面拿出一个元素放到L里面，这可能会导致L变成无序的，
        ///所以我们还要再次把L处理成有序的。
        ///现在到了关键的地方：我们怎么把L抽象成只有有限个状态的系统？
        ///（因为只有这样，我们才能在向L里放入一个新元素之后，使用有限的语句再次把L处理成有序的）
        ///在本例中很简单，当我们把一个新元素b放入L的末尾时，
        ///我们可以把L里面小于等于b的、紧挨着的元素称为a，把大于b的、
        ///紧挨着的元素称为c，因为L是有序的，所以，此时L的状态只可能有3种：{ a, b }、{ a, c, b }、{ c, b }。
        ///对于第1种情况，L已经是有序的，
        ///不需要再做处理；对于第2、第3种情况，我们只需把b跟c交换一下位置就可以使L再次变为有序的了
        /// 让我们把整个过程写一遍：
        ///初始时：L = { 4 }, R = { 2, 5, 10, 7 }。
        ///第1次迭代：L = { c, b }, 其中 c = { 4 }  b={ 2 }, R = { 5, 10, 7 }。我们把b和c交换，L = { 2, 4 }。
        ///第2次迭代：L = { a, b }, 其中 a = { 2, 4 }  b={ 5 }, R = { 10, 7 }。L已经是有序的，不需要处理，L = { 2, 4, 5 }。
        ///第3次迭代：L = { a, b }, 其中 a = { 2, 4, 5 }  b={ 10 }, R = { 7 }。L已经是有序的，不需要处理，L = { 2, 4, 5, 10 }。
        ///第4次迭代：L = { a, c, b }, 其中 a = { 2, 4, 5 }  c={ 10 }  b={ 7 }, R = {  }。我们把b和c交换，L = { 2, 4, 5, 7, 10 }。
        ///程序结束。
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static void ArraySort<T>(T[] array) where T : IComparable
        {
            for (int i = 0; i < array.Length - 1; i++)//array[0..i] 是每次迭代前的 L 
            {
                T b = array[i + 1];//array[0..i+1] 是迭代后的 L 
                int j = i;
                while (j >= 0 && array[j].CompareTo(b) > 0)
                {
                    array[j + 1] = array[j];
                    j--;
                }
                array[j + 1] = b;
            }
        }

        /// <summary>
        /// 冒泡排序,这个比较经典，无需多解释
        /// </summary>
        /// <param name="a"></param>
        public static void BubbleSort<T>(T[] array) where T : IComparable
        {
            int bound = array.Length - 1;
            while (bound > 0)
            {
                int index = 0;
                for (int j = 0; j < bound; j++)
                {
                    if (array[j + 1].CompareTo(array[j]) < 0)
                    {
                        T temp = array[j];
                        array[j] = array[j + 1];
                        array[j + 1] = temp;
                        index = j;
                    }
                }
                bound = index;
            }
        }

        /// <summary>
        ///每次从无序表中取出第一个元素，把它插入到有序表的合适位置，使有序表仍然有序。 
        ///第一趟比较前两个数,然后把第二个数按大小插入到有序表中; 第二趟把第三个数据与前两个数从后向前扫描，把第三个数按大小插入到有序表中；
        ///依次进行下去，进行了(n-1)趟扫描以后就完成了整个排序过程。
        ///直接插入排序属于稳定的排序，时间复杂性为o(n^2)，空间复杂度为O(1)。 
        ///直接插入排序是由两层嵌套循环组成的。外层循环标识并决定待比较的数值。内层循环为待比较数值确定其最终位置。
        ///直接插入排序是将待比较的数值与它的前一个数值进行比较，所以外层循环是从第二个数值开始的
        ///。当前一数值比待比较数值大的情况下继续循环比较，直到找到比待比较数值小的并将待比较数值置入其后一位置，结束该次循环。
        ///值得注意的是，我们必需用一个存储空间来保存当前待比较的数值，因为当一趟比较完成时，
        ///我们要将待比较数值置入比它小的数值的后一位 插入排序类似玩牌时整理手中纸牌的过程。
        ///插入排序的基本方法是：每步将一个待排序的记录按其关键字的大小插到前面已经排序的序列中的适当位置，直到全部记录插入完毕为止。
        ///第一层循环是为了依次将数组中的值放入到有序表里，这里循环是从1开始，第1个元素就是有序表。
        ///比如循环进到了5，那么前五个元素就是有序表，后面的就是无序表 
        ///第二层循环是为了形成有序表，第一次外层循环，都有一个元素放到有序表中，并形成新的有序表。 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        public static void InsertSort<T>(T[] array) where T : IComparable
        {
            int length = array.Length;
            for (int i = 1; i < length; i++)
            {
                T temp = array[i];
                if (temp.CompareTo(array[i - 1]) < 0)
                {
                    for (int j = 0; j < i; j++)
                    {
                        if (temp.CompareTo(array[j]) < 0)
                        {
                            temp = array[j];
                            array[j] = array[i];
                            array[i] = temp;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Shell Sort
        /// 在直接插入排序算法中，每次插入一个数，使有序序列只增加1个节点， 
        ///并且对插入下一个数没有提供任何帮助。如果比较相隔较远距离（称为 
        ///增量）的数，使得数移动时能跨过多个元素，则进行一次比较就可能消除 
        ///多个元素交换。D.L.shell于1959年在以他名字命名的排序算法中实现 
        ///了这一思想。算法先将要排序的一组数按某个增量d分成若干组，每组中 
        ///记录的下标相差d.对每组中全部元素进行排序，然后再用一个较小的增量 
        ///对它进行，在每组中再进行排序。当增量减到1时，整个要排序的数被分成一组，排序完成。 
        ///下面的函数是一个希尔排序算法的一个实现，初次取序列的一半为增量， 
        ///以后每次减半，直到增量为1。 
        ///希尔排序是不稳定的。 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        public static void ShellSort<T>(T[] array) where T : IComparable
        {
            int length = array.Length;
            for (int h = length / 2; h > 0; h = h / 2)
            {
                for (int i = h; i < length; i++)
                {
                    T temp = array[i];
                    if (temp.CompareTo(array[i - h]) < 0)
                    {
                        for (int j = 0; j < i; j += h)
                        {
                            if (temp.CompareTo(array[j]) < 0)
                            {
                                temp = array[j];
                                array[j] = array[i];
                                array[i] = temp;
                            }
                        }
                    }
                }
            }
        }

        #region 快速排序
        /// <summary>
        /// 参考
        /// http://www.cnblogs.com/1-2-3/archive/2010/04/07/grasp-algorithm3.html
        /// </summary>
        /// <param name="s"></param>
        /// <param name="p"></param>
        /// <param name="r"></param>
        private static void FQuickSort(int[] s, int p, int r)
        {
            do
            {
                // 分解：将数组s[p..r]分解为 { A, x, C }的形式，A=s[p..q-1], x=s[q], C=s[q+1..r]。并且A中的所有元素都小于等于x，C中的所有元素都大于x  
                int q = FPartition(s, p, r);
                // 递归解决  
                if (q - p <= r - q) // A 的元素比较少  
                {
                    if (p < q - 1) FQuickSort(s, p, q - 1); // 排序A  
                    p = q + 1;
                }
                else // C 的元素比较少  
                {
                    if (q + 1 < r) FQuickSort(s, q + 1, r); // 排序C  
                    r = q - 1;
                }
            }
            while (p < r); // 需要进一步分解  
        }

        /// <summary>
        /// 趴堆（分区）方法
        /// </summary>
        /// <param name="s"></param>
        /// <param name="p"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        private static int FPartition(int[] s, int p, int r)
        {
            int m = p + (r - p) / 2; // 数组中间的元素的下标  
            // 为使s[r]成为s[p]、s[r]、s[m]的中数，对这三个数排序  
            if (s[p] > s[r])
            {
                int temp1 = s[p];
                s[p] = s[r];
                s[r] = temp1;
            }
            if (s[r] > s[m])
            {
                int temp1 = s[r];
                s[r] = s[m];
                s[m] = temp1;
            }

            if (s[p] > s[r])
            {
                int temp1 = s[p];
                s[p] = s[r];
                s[r] = temp1;
            }

            int x = s[r];
            int i = p;
            int j = r - 1;

            // 每次迭代都确保 s[p..i-1] 中的每个元素都小于等于x，s[j+1..r] 中的每个元素都大于等于x  
            do
            {
                while (i <= r - 1 && s[i] < x) i++;
                while (j >= p && x < s[j]) j--;
                if (i <= j)
                {
                    int temp = s[i];
                    s[i] = s[j];
                    s[j] = temp;
                    i++;
                    j--;
                }
            } while (i <= j);

            // 交换s[r] 和 s[j+1]  
            s[r] = s[j + 1];
            s[j + 1] = x;
            return j + 1;
        }

        /// <summary>
        /// 快速排序方法
        /// </summary>
        /// <param name="s"></param>
        public static void Sort(int[] s)
        {
            if (s != null && s.Length > 1)
                FQuickSort(s, 0, s.Length - 1);
        }
        #endregion
    }
}
