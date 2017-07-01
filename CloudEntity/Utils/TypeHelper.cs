using System;
using System.Reflection;

namespace CloudEntity
{
    /// <summary>
    /// 用于扩展object
    /// 李凯 2015/10/08
    /// </summary>
    public static class TypeHelper
    {
        /// <summary>
        /// object类型扩展方法:将value转换为目标类型
        /// </summary>
        /// <typeparam name="TValue">目标类型</typeparam>
        /// <param name="value">原始值</param>
        /// <returns>目标类型的值</returns>
        public static TValue ConvertTo<TValue>(object value)
        {
            //若值为空,返回默认值
            if (value == null)
                return default(TValue);
            //获取目标类型
            Type resultType = typeof(TValue);
            //若目标类型与value的类型一直，直接范围， 否则在转换
            object result = resultType.GetTypeInfo().IsInstanceOfType(value) ? value : Convert.ChangeType(value, resultType);
            //返回最终的值
            return (TValue)result;
        }
        /// <summary>
        /// 获取当前类型的目标类型名称
        /// </summary>
        /// <param name="type">当前类型</param>
        /// <returns>当前类型的目标类型名称</returns>
        public static string SourceTypeName(this Type type)
        {
            //若当前类型不是基本类型，扔出异常
            Check.IsBaseType(type);
            //若当前类型不是可空类型,直接返回当前类型
            if (type.Name != "Nullable`1")
                return type.Name;
            //从Nullable类型名称中获取最终的类型名称
            int start = "System.Nullable`1[[System.".Length;
            int length = type.FullName.IndexOf(',') - start;
            return type.FullName.Substring(start, length);
        }
    }
}
