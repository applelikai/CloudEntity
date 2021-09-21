using System;
using System.Collections.Generic;

namespace CloudEntity
{
    /// <summary>
    /// 迭代器扩展类
    /// </summary>
    public static class ExtendEnumerable
    {
        /// <summary>
        /// Extendable method: 拼接数据源和一个数组数据源，获取新的数据源
        /// </summary>
        /// <typeparam name="TElement">元素类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="elements">数组数据源</param>
        /// <returns>拼接后的新数据源</returns>
        public static IEnumerable<TElement> Concat<TElement>(this IEnumerable<TElement> source, params TElement[] elements)
        {
            //遍历旧数据源
            foreach (TElement oldElement in source)
            {
                //依次获取元素
                yield return oldElement;
            }
            //遍历数组数据源
            foreach (TElement element in elements)
            {
                //依次获取元素
                yield return element;
            }
        }
        /// <summary>
        /// Extendable method: 获取Key值不重复的元素数据源
        /// </summary>
        /// <typeparam name="TElement">元素类型</typeparam>
        /// <typeparam name="TKey">key类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="keySelector">指定元素key值的表达式</param>
        /// <returns>Key值不重复的元素数据源</returns>
        public static IEnumerable<TElement> DistinctBy<TElement, TKey>(this IEnumerable<TElement> source, Func<TElement, TKey> keySelector)
        {
            //初始化不允许重复的keys
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            //遍历元素，去除相同key的元素
            foreach (TElement element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                    yield return element;
            }
        }
    }
}
