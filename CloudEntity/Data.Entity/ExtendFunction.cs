using System;
using System.Linq.Expressions;

namespace CloudEntity.Data.Entity
{
    /// <summary>
    /// 数据源统计函数扩展类
    /// Apple_Li 李凯 15150598493
    /// </summary>
    public static class ExtendFunction
    {
        /// <summary>
        /// Internal Extendable method: 执行对数据源中所有元素的某属性的统计,获取统计值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TProperty">属性类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="methodName">源方法名</param>
        /// <param name="selector">指定实体某属性表达式</param>
        /// <returns>数据源中所有元素某属性的统计值</returns>
        private static TProperty Function<TEntity, TProperty>(this IDbSource<TEntity> source, string methodName, Expression<Func<TEntity, TProperty>> selector)
            where TEntity : class
            where TProperty : struct
        {
            //非空检查
            Check.ArgumentNull(source, nameof(source));
            Check.ArgumentNull(selector, nameof(selector));
            IDbScalar dbScaler = source.Factory.CreateScalar(source, methodName.ToUpper(), selector);
            //执行获取第一行第一列的统计值
            return TypeHelper.ConvertTo<TProperty>(dbScaler.Execute());
        }
        /// <summary>
        /// Internal Extendable method: 执行对数据源中所有元素的某属性的统计,获取统计值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TProperty">属性类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="methodName">源扩展方法</param>
        /// <param name="selector">指定实体某属性表达式</param>
        /// <returns>数据源中所有元素某属性的统计值</returns>
        private static TProperty Function<TEntity, TProperty>(this IDbSource<TEntity> source, string methodName, Expression<Func<TEntity, TProperty?>> selector)
            where TEntity : class
            where TProperty : struct
        {
            //非空检查
            Check.ArgumentNull(source, nameof(source));
            Check.ArgumentNull(selector, nameof(selector));
            //创建DbScaler
            IDbScalar dbScaler = source.Factory.CreateScalar(source, methodName.ToUpper(), selector);
            //执行获取第一行第一列的统计值
            return TypeHelper.ConvertTo<TProperty>(dbScaler.Execute());
        }
        /// <summary>
        /// Internal Extendable method: 执行对数据源中符合条件的元素的某属性的统计,获取统计值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TProperty">属性类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="methodName">源扩展方法</param>
        /// <param name="selector">指定实体某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>数据源中符合条件的元素某属性的统计值</returns>
        private static TProperty Function<TEntity, TProperty>(this IDbSource<TEntity> source, string methodName, Expression<Func<TEntity, TProperty>> selector, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
            where TProperty : struct
        {
            //非空检查
            Check.ArgumentNull(source, nameof(source));
            Check.ArgumentNull(selector, nameof(selector));
            Check.ArgumentNull(predicate, nameof(predicate));
            //创建DbScaler
            IDbScalar dbScaler = source.Factory.CreateScalar(source.Where(predicate), methodName.ToUpper(), selector);
            //执行获取第一行第一列的统计值
            return TypeHelper.ConvertTo<TProperty>(dbScaler.Execute());
        }
        /// <summary>
        /// Internal Extendable method: 执行对数据源中符合条件的元素的某属性的统计,获取统计值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TProperty">属性类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="methodName">源扩展方法</param>
        /// <param name="selector">指定实体某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>数据源中符合条件的元素某属性的统计值</returns>
        private static TProperty Function<TEntity, TProperty>(this IDbSource<TEntity> source, string methodName, Expression<Func<TEntity, TProperty?>> selector, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
            where TProperty : struct
        {
            //非空检查
            Check.ArgumentNull(source, nameof(source));
            Check.ArgumentNull(selector, nameof(selector));
            Check.ArgumentNull(predicate, nameof(predicate));
            //创建DbScaler
            IDbScalar dbScaler = source.Factory.CreateScalar(source.Where(predicate), methodName.ToUpper(), selector);
            //执行获取第一行第一列的统计值
            return TypeHelper.ConvertTo<TProperty>(dbScaler.Execute());
        }

        /// <summary>
        /// Extendable method: 数据源中所有元素是否都满足条件
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="predicate">条件表达式</param>
        /// <returns>数据源中所有元素是否都满足条件</returns>
        private static bool All<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return source.Count() == source.Count(predicate);
        }
        /// <summary>
        /// Extendable method: 数据源中是否任何一个元素都满足条件
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="predicate">条件表达式</param>
        /// <returns>数据源中是否任何一个元素都满足条件</returns>
        private static bool Any<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return source.Count(predicate) > 0;
        }

        /// <summary>
        /// Extendable method: 获取实体数据源中所有实体某属性的平均值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定实体某属性表达式</param>
        /// <returns>实体数据源中所有实体某属性的平均值</returns>
        public static int Avg<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, int>> selector)
            where TEntity : class
        {
            return source.Function("Avg", selector);
        }
        /// <summary>
        /// Extendable method: 获取实体数据源中所有实体某属性的平均值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定实体某属性表达式</param>
        /// <returns>实体数据源中所有实体某属性的平均值</returns>
        public static int Avg<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, int?>> selector)
            where TEntity : class
        {
            return source.Function("Avg", selector);
        }
        /// <summary>
        /// Extendable method: 获取实体数据源中符合条件的实体某属性的平均值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定实体某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>实体数据源中符合条件的实体某属性的平均值</returns>
        public static int Avg<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, int>> selector, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return source.Function("Avg", selector, predicate);
        }
        /// <summary>
        /// Extendable method: 获取实体数据源中符合条件的实体某属性的平均值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定实体某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>实体数据源中符合条件的实体某属性的平均值</returns>
        public static int Avg<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, int?>> selector, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return source.Function("Avg", selector, predicate);
        }
        /// <summary>
        /// Extendable method: 获取实体数据源中所有实体某属性的平均值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定实体某属性表达式</param>
        /// <returns>实体数据源中所有实体某属性的平均值</returns>
        public static double Avg<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, double>> selector)
            where TEntity : class
        {
            return source.Function("Avg", selector);
        }
        /// <summary>
        /// Extendable method: 获取实体数据源中所有实体某属性的平均值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定实体某属性表达式</param>
        /// <returns>实体数据源中所有实体某属性的平均值</returns>
        public static double Avg<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, double?>> selector)
            where TEntity : class
        {
            return source.Function("Avg", selector);
        }
        /// <summary>
        /// Extendable method: 获取实体数据源中符合条件的实体某属性的平均值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定实体某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>实体数据源中符合条件的实体某属性的平均值</returns>
        public static double Avg<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, double>> selector, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return source.Function("Avg", selector, predicate);
        }
        /// <summary>
        /// Extendable method: 获取实体数据源中符合条件的实体某属性的平均值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定实体某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>实体数据源中符合条件的实体某属性的平均值</returns>
        public static double Avg<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, double?>> selector, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return source.Function("Avg", selector, predicate);
        }
        /// <summary>
        /// Extendable method: 获取实体数据源中所有实体某属性的平均值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定实体某属性表达式</param>
        /// <returns>实体数据源中所有实体某属性的平均值</returns>
        public static decimal Avg<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, decimal>> selector)
            where TEntity : class
        {
            return source.Function("Avg", selector);
        }
        /// <summary>
        /// Extendable method: 获取实体数据源中所有实体某属性的平均值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定实体某属性表达式</param>
        /// <returns>实体数据源中所有实体某属性的平均值</returns>
        public static decimal Avg<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, decimal?>> selector)
            where TEntity : class
        {
            return source.Function("Avg", selector);
        }
        /// <summary>
        /// Extendable method: 获取实体数据源中符合条件的实体某属性的平均值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定实体某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>实体数据源中符合条件的实体某属性的平均值</returns>
        public static decimal Avg<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, decimal>> selector, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return source.Function("Avg", selector, predicate);
        }
        /// <summary>
        /// Extendable method: 获取实体数据源中符合条件的实体某属性的平均值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定实体某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>实体数据源中符合条件的实体某属性的平均值</returns>
        public static decimal Avg<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, decimal?>> selector, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return source.Function("Avg", selector, predicate);
        }

        /// <summary>
        /// Extendable method: 统计实体数据源中所有元素属性的和
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定实体某属性表达式</param>
        /// <returns>实体数据源中所有元素某属性的和</returns>
        public static int Sum<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, int>> selector)
            where TEntity : class
        {
            return source.Function("Sum", selector);
        }
        /// <summary>
        /// Extendable method: 统计实体数据源中所有元素属性的和
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定实体某属性表达式</param>
        /// <returns>实体数据源中所有元素某属性的和</returns>
        public static int Sum<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, int?>> selector)
            where TEntity : class
        {
            return source.Function("Sum", selector);
        }
        /// <summary>
        /// Extendable method: 统计实体数据源中符合条件的元素属性的和
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定实体某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>实体数据源中符合条件的元素某属性的和</returns>
        public static int Sum<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, int>> selector, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return source.Function("Sum", selector, predicate);
        }
        /// <summary>
        /// Extendable method: 统计实体数据源中符合条件的元素属性的和
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定实体某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>实体数据源中符合条件的元素某属性的和</returns>
        public static int Sum<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, int?>> selector, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return source.Function("Sum", selector, predicate);
        }
        /// <summary>
        /// Extendable method: 统计实体数据源中所有元素某属性的和
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定实体某属性表达式</param>
        /// <returns>实体数据源中所有元素某属性的和</returns>
        public static double Sum<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, double>> selector)
            where TEntity : class
        {
            return source.Function("Sum", selector);
        }
        /// <summary>
        /// Extendable method: 统计实体数据源中所有元素某属性的和
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源函数执行器</param>
        /// <param name="selector">指定实体某属性表达式</param>
        /// <returns>实体数据源中所有元素某属性的和</returns>
        public static double Sum<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, double?>> selector)
            where TEntity : class
        {
            return source.Function("Sum", selector);
        }
        /// <summary>
        /// Extendable method: 统计实体数据源中符合条件的元素某属性的和
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定实体某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>实体数据源中符合条件的元素某属性的和</returns>
        public static double Sum<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, double>> selector, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return source.Function("Sum", selector, predicate);
        }
        /// <summary>
        /// Extendable method: 统计实体数据源中符合条件的元素某属性的和
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定实体某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>实体数据源中符合条件的元素某属性的和</returns>
        public static double Sum<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, double?>> selector, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return source.Function("Sum", selector, predicate);
        }
        /// <summary>
        /// Extendable method: 统计实体数据源中所有元素某属性的和
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定实体某属性表达式</param>
        /// <returns>实体数据源中所有元素某属性的和</returns>
        public static decimal Sum<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, decimal>> selector)
            where TEntity : class
        {
            return source.Function("Sum", selector);
        }
        /// <summary>
        /// Extendable method: 统计实体数据源中所有元素某属性的和
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源函数执行器</param>
        /// <param name="selector">指定实体某属性表达式</param>
        /// <returns>实体数据源中所有元素某属性的和</returns>
        public static decimal Sum<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, decimal?>> selector)
            where TEntity : class
        {
            return source.Function("Sum", selector);
        }
        /// <summary>
        /// Extendable method: 统计实体数据源中符合条件的元素某属性的和
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定实体某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>实体数据源中符合条件的元素某属性的和</returns>
        public static decimal Sum<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, decimal>> selector, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return source.Function("Sum", selector, predicate);
        }
        /// <summary>
        /// Extendable method: 统计实体数据源中符合条件的元素某属性的和
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定实体某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>实体数据源中符合条件的元素某属性的和</returns>
        public static decimal Sum<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, decimal?>> selector, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return source.Function("Sum", selector, predicate);
        }

        /// <summary>
        /// Extendable method: 查询该对象化数据源中总共多少个元素
        /// </summary>
        /// <typeparam name="TEntity">对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <returns>元素个数</returns>
        public static int Count<TEntity>(this IDbSource<TEntity> source)
            where TEntity : class
        {
            //检查数据源是否为空
            Check.ArgumentNull(source, nameof(source));
            //创建DbScaler
            IDbScalar dbScaler = source.Factory.CreateScalar(source, "COUNT");
            //执行统计查询获取元素数量
            return TypeHelper.ConvertTo<int>(dbScaler.Execute());
        }
        /// <summary>
        /// Extendable method: 统计符合条件的对象个数
        /// </summary>
        /// <typeparam name="TEntity">对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="predicate">指定与对象属性比较的表达式</param>
        /// <returns>统计符合条件的对象个数</returns>
        public static int Count<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return source.Where(predicate).Count();
        }

        /// <summary>
        /// Extendable method: 查询该对象化数据源中总共多少个元素
        /// </summary>
        /// <typeparam name="TEntity">对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <returns>元素个数</returns>
        public static long LongCount<TEntity>(this IDbSource<TEntity> source)
            where TEntity : class
        {
            //检查数据源是否为空
            Check.ArgumentNull(source, nameof(source));
            //创建DbScaler
            IDbScalar dbScaler = source.Factory.CreateScalar(source, "COUNT");
            //执行统计查询获取元素数量
            return TypeHelper.ConvertTo<long>(dbScaler.Execute());
        }
        /// <summary>
        /// Extendable method: 查询该对象化数据源中总共多少个元素
        /// </summary>
        /// <typeparam name="TEntity">对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="predicate">指定与对象属性比较的表达式</param>
        /// <returns>元素个数</returns>
        public static long LongCount<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return source.Where(predicate).LongCount();
        }

        /// <summary>
        /// Extendable method: 统计数据源中所有元素某属性的最大值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TProperty">属性类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定元素某属性的表达式</param>
        /// <returns>数据源中所有元素某属性的最大值</returns>
        public static TProperty Max<TEntity, TProperty>(this IDbSource<TEntity> source, Expression<Func<TEntity, TProperty>> selector)
            where TEntity : class
        {
            //非空检查
            Check.ArgumentNull(source, nameof(source));
            Check.ArgumentNull(selector, nameof(selector));
            //创建DbScaler
            IDbScalar dbScaler = source.Factory.CreateScalar(source, "MAX", selector);
            //执行统计查询获取元素属性最大值
            return TypeHelper.ConvertTo<TProperty>(dbScaler.Execute());
        }
        /// <summary>
        /// Extendable method: 统计数据源中符合条件的元素某属性的最大值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TProperty">属性类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定元素某属性的表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>数据源中符合条件的元素某属性的最大值</returns>
        public static TProperty Max<TEntity, TProperty>(this IDbSource<TEntity> source, Expression<Func<TEntity, TProperty>> selector, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return source.Where(predicate).Max(selector);
        }

        /// <summary>
        /// Extendable method: 统计数据源中所有元素某属性的最小值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TProperty">属性类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定元素某属性的表达式</param>
        /// <returns>数据源中所有元素某属性的最小值</returns>
        public static TProperty Min<TEntity, TProperty>(this IDbSource<TEntity> source, Expression<Func<TEntity, TProperty>> selector)
            where TEntity : class
        {
            //非空检查
            Check.ArgumentNull(source, nameof(source));
            Check.ArgumentNull(selector, nameof(selector));
            //创建DbScaler
            IDbScalar dbScaler = source.Factory.CreateScalar(source, "MIN", selector);
            //执行统计查询获取元素属性最大值
            return TypeHelper.ConvertTo<TProperty>(dbScaler.Execute());
        }
        /// <summary>
        /// Extendable method: 统计数据源中符合条件的元素某属性的最小值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TProperty">属性类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定元素某属性的表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>数据源中符合条件的元素某属性的最小值</returns>
        public static TProperty Min<TEntity, TProperty>(this IDbSource<TEntity> source, Expression<Func<TEntity, TProperty>> selector, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return source.Where(predicate).Min(selector);
        }
    }
}