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
        /// <param name="selector">指定对象某属性表达式</param>
        /// <returns>数据源中所有元素某属性的统计值</returns>
        private static TProperty Function<TEntity, TProperty>(this IDbSource<TEntity> source, string methodName, Expression<Func<TEntity, TProperty>> selector)
            where TEntity : class
            where TProperty : struct
        {
            // 非空检查
            Check.ArgumentNull(source, nameof(source));
            Check.ArgumentNull(selector, nameof(selector));
            // 获取统计查询数据源对象
            IDbScalar dbScaler = source.Factory.CreateScalar(source, methodName.ToUpper(), selector);
            // 执行获取第一行第一列的统计值
            return TypeHelper.ConvertTo<TProperty>(dbScaler.Execute());
        }
        /// <summary>
        /// Internal Extendable method: 执行对数据源中所有元素的某属性的统计,获取统计值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TProperty">属性类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="methodName">源扩展方法</param>
        /// <param name="selector">指定对象某属性表达式</param>
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
        /// <param name="selector">指定对象某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>数据源中符合条件的元素某属性的统计值</returns>
        private static TProperty Function<TEntity, TProperty>(this IDbSource<TEntity> source, string methodName, Expression<Func<TEntity, TProperty>> selector, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
            where TProperty : struct
        {
            // 非空检查
            Check.ArgumentNull(source, nameof(source));
            Check.ArgumentNull(selector, nameof(selector));
            Check.ArgumentNull(predicate, nameof(predicate));
            // 创建DbScaler
            IDbScalar dbScaler = source.Factory.CreateScalar(source.Where(predicate), methodName.ToUpper(), selector);
            // 执行获取第一行第一列的统计值
            return TypeHelper.ConvertTo<TProperty>(dbScaler.Execute());
        }
        /// <summary>
        /// Internal Extendable method: 执行对数据源中符合条件的元素的某属性的统计,获取统计值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TProperty">属性类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="methodName">源扩展方法</param>
        /// <param name="selector">指定对象某属性表达式</param>
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
        /// Internal Extendable method: 执行对数据源中所有元素的某属性的统计,获取统计值
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <typeparam name="TProperty">属性类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="methodName">源方法名</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <returns>数据源中所有元素某属性的统计值</returns>
        private static TProperty Function<TModel, TProperty>(this IDbAsView<TModel> source, string methodName, Expression<Func<TModel, TProperty>> selector)
            where TModel : class, new()
            where TProperty : struct
        {
            // 非空检查
            Check.ArgumentNull(source, nameof(source));
            Check.ArgumentNull(selector, nameof(selector));
            // 获取统计查询数据源对象
            IDbScalar dbScaler = source.Factory.CreateScalar(source, methodName.ToUpper(), selector);
            // 执行获取第一行第一列的统计值
            return TypeHelper.ConvertTo<TProperty>(dbScaler.Execute());
        }
        /// <summary>
        /// Internal Extendable method: 执行对数据源中所有元素的某属性的统计,获取统计值
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <typeparam name="TProperty">属性类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="methodName">源方法名</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <returns>数据源中所有元素某属性的统计值</returns>
        private static TProperty Function<TModel, TProperty>(this IDbAsView<TModel> source, string methodName, Expression<Func<TModel, TProperty?>> selector)
            where TModel : class, new()
            where TProperty : struct
        {
            // 非空检查
            Check.ArgumentNull(source, nameof(source));
            Check.ArgumentNull(selector, nameof(selector));
            // 获取统计查询数据源对象
            IDbScalar dbScaler = source.Factory.CreateScalar(source, methodName.ToUpper(), selector);
            // 执行获取第一行第一列的统计值
            return TypeHelper.ConvertTo<TProperty>(dbScaler.Execute());
        }
        /// <summary>
        /// Internal Extendable method: 执行对数据源中所有元素的某属性的统计,获取统计值
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <typeparam name="TProperty">属性类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="methodName">源方法名</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>数据源中所有元素某属性的统计值</returns>
        private static TProperty Function<TModel, TProperty>(this IDbAsView<TModel> source, string methodName, Expression<Func<TModel, TProperty>> selector, Expression<Func<TModel, bool>> predicate)
            where TModel : class, new()
            where TProperty : struct
        {
            // 非空检查
            Check.ArgumentNull(source, nameof(source));
            Check.ArgumentNull(selector, nameof(selector));
            Check.ArgumentNull(predicate, nameof(predicate));
            // 获取统计查询数据源对象
            IDbScalar dbScaler = source.Factory.CreateScalar(source.Where(predicate), methodName.ToUpper(), selector);
            // 执行获取第一行第一列的统计值
            return TypeHelper.ConvertTo<TProperty>(dbScaler.Execute());
        }
        /// <summary>
        /// Internal Extendable method: 执行对数据源中所有元素的某属性的统计,获取统计值
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <typeparam name="TProperty">属性类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="methodName">源方法名</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>数据源中所有元素某属性的统计值</returns>
        private static TProperty Function<TModel, TProperty>(this IDbAsView<TModel> source, string methodName, Expression<Func<TModel, TProperty?>> selector, Expression<Func<TModel, bool>> predicate)
            where TModel : class, new()
            where TProperty : struct
        {
            // 非空检查
            Check.ArgumentNull(source, nameof(source));
            Check.ArgumentNull(selector, nameof(selector));
            Check.ArgumentNull(predicate, nameof(predicate));
            // 获取统计查询数据源对象
            IDbScalar dbScaler = source.Factory.CreateScalar(source.Where(predicate), methodName.ToUpper(), selector);
            // 执行获取第一行第一列的统计值
            return TypeHelper.ConvertTo<TProperty>(dbScaler.Execute());
        }

        /// <summary>
        /// Extendable method: 数据源中所有元素是否都满足条件
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="predicate">条件表达式</param>
        /// <returns>数据源中所有元素是否都满足条件</returns>
        public static bool All<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return source.Count() == source.Count(predicate);
        }
        /// <summary>
        /// Extendable method: 数据源中所有元素是否都满足条件
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="predicate">条件表达式</param>
        /// <returns>数据源中所有元素是否都满足条件</returns>
        public static bool All<TModel>(this IDbAsView<TModel> source, Expression<Func<TModel, bool>> predicate)
            where TModel : class, new()
        {
            return source.Count() == source.Count(predicate);
        }
        /// <summary>
        /// Extendable method: 数据源中是否有元素满足条件
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="predicate">条件表达式</param>
        /// <returns>数据源中是否有元素满足条件</returns>
        public static bool Any<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return source.Count(predicate) > 0;
        }
        /// <summary>
        /// Extendable method: 数据源中是否有元素满足条件
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="predicate">条件表达式</param>
        /// <returns>数据源中是否有元素满足条件</returns>
        public static bool Any<TModel>(this IDbAsView<TModel> source, Expression<Func<TModel, bool>> predicate)
            where TModel : class, new()
        {
            return source.Count(predicate) > 0;
        }

        /// <summary>
        /// Extendable method: 获取数据源中所有对象某属性的平均值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <returns>数据源中所有对象某属性的平均值</returns>
        public static int Avg<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, int>> selector)
            where TEntity : class
        {
            return source.Function("Avg", selector);
        }
        /// <summary>
        /// Extendable method: 获取数据源中所有对象某属性的平均值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <returns>数据源中所有对象某属性的平均值</returns>
        public static int Avg<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, int?>> selector)
            where TEntity : class
        {
            return source.Function("Avg", selector);
        }
        /// <summary>
        /// Extendable method: 获取数据源中符合条件的对象某属性的平均值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>数据源中符合条件的对象某属性的平均值</returns>
        public static int Avg<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, int>> selector, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return source.Function("Avg", selector, predicate);
        }
        /// <summary>
        /// Extendable method: 获取数据源中符合条件的对象某属性的平均值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>数据源中符合条件的对象某属性的平均值</returns>
        public static int Avg<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, int?>> selector, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return source.Function("Avg", selector, predicate);
        }

        /// <summary>
        /// Extendable method: 获取数据源中所有视图对象某属性的平均值
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <returns>数据源中所有对象某属性的平均值</returns>
        public static int Avg<TModel>(this IDbAsView<TModel> source, Expression<Func<TModel, int>> selector)
            where TModel : class, new()
        {
            return source.Function("Avg", selector);
        }
        /// <summary>
        /// Extendable method: 获取数据源中所有对象某属性的平均值
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <returns>数据源中所有对象某属性的平均值</returns>
        public static int Avg<TModel>(this IDbAsView<TModel> source, Expression<Func<TModel, int?>> selector)
            where TModel : class, new()
        {
            return source.Function("Avg", selector);
        }
        /// <summary>
        /// Extendable method: 获取数据源中符合条件的对象某属性的平均值
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>数据源中符合条件的对象某属性的平均值</returns>
        public static int Avg<TModel>(this IDbAsView<TModel> source, Expression<Func<TModel, int>> selector, Expression<Func<TModel, bool>> predicate)
            where TModel : class, new()
        {
            return source.Function("Avg", selector, predicate);
        }
        /// <summary>
        /// Extendable method: 获取数据源中符合条件的对象某属性的平均值
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>数据源中符合条件的对象某属性的平均值</returns>
        public static int Avg<TModel>(this IDbAsView<TModel> source, Expression<Func<TModel, int?>> selector, Expression<Func<TModel, bool>> predicate)
            where TModel : class, new()
        {
            return source.Function("Avg", selector, predicate);
        }

        /// <summary>
        /// Extendable method: 获取数据源中所有对象某属性的平均值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <returns>数据源中所有对象某属性的平均值</returns>
        public static long Avg<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, long>> selector)
            where TEntity : class
        {
            return source.Function("Avg", selector);
        }
        /// <summary>
        /// Extendable method: 获取数据源中所有对象某属性的平均值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <returns>数据源中所有对象某属性的平均值</returns>
        public static long Avg<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, long?>> selector)
            where TEntity : class
        {
            return source.Function("Avg", selector);
        }
        /// <summary>
        /// Extendable method: 获取数据源中符合条件的对象某属性的平均值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>数据源中符合条件的对象某属性的平均值</returns>
        public static long Avg<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, long>> selector, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return source.Function("Avg", selector, predicate);
        }
        /// <summary>
        /// Extendable method: 获取数据源中符合条件的对象某属性的平均值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>数据源中符合条件的对象某属性的平均值</returns>
        public static long Avg<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, long?>> selector, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return source.Function("Avg", selector, predicate);
        }

        /// <summary>
        /// Extendable method: 获取数据源中所有对象某属性的平均值
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <returns>数据源中所有对象某属性的平均值</returns>
        public static long Avg<TModel>(this IDbAsView<TModel> source, Expression<Func<TModel, long>> selector)
            where TModel : class, new()
        {
            return source.Function("Avg", selector);
        }
        /// <summary>
        /// Extendable method: 获取数据源中所有对象某属性的平均值
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <returns>数据源中所有对象某属性的平均值</returns>
        public static long Avg<TModel>(this IDbAsView<TModel> source, Expression<Func<TModel, long?>> selector)
            where TModel : class, new()
        {
            return source.Function("Avg", selector);
        }
        /// <summary>
        /// Extendable method: 获取数据源中符合条件的对象某属性的平均值
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>数据源中符合条件的对象某属性的平均值</returns>
        public static long Avg<TModel>(this IDbAsView<TModel> source, Expression<Func<TModel, long>> selector, Expression<Func<TModel, bool>> predicate)
            where TModel : class, new()
        {
            return source.Function("Avg", selector, predicate);
        }
        /// <summary>
        /// Extendable method: 获取数据源中符合条件的对象某属性的平均值
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>数据源中符合条件的对象某属性的平均值</returns>
        public static long Avg<TModel>(this IDbAsView<TModel> source, Expression<Func<TModel, long?>> selector, Expression<Func<TModel, bool>> predicate)
            where TModel : class, new()
        {
            return source.Function("Avg", selector, predicate);
        }

        /// <summary>
        /// Extendable method: 获取数据源中所有对象某属性的平均值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <returns>数据源中所有对象某属性的平均值</returns>
        public static double Avg<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, double>> selector)
            where TEntity : class
        {
            return source.Function("Avg", selector);
        }
        /// <summary>
        /// Extendable method: 获取数据源中所有对象某属性的平均值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <returns>数据源中所有对象某属性的平均值</returns>
        public static double Avg<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, double?>> selector)
            where TEntity : class
        {
            return source.Function("Avg", selector);
        }
        /// <summary>
        /// Extendable method: 获取数据源中符合条件的对象某属性的平均值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>数据源中符合条件的对象某属性的平均值</returns>
        public static double Avg<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, double>> selector, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return source.Function("Avg", selector, predicate);
        }
        /// <summary>
        /// Extendable method: 获取数据源中符合条件的对象某属性的平均值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>数据源中符合条件的对象某属性的平均值</returns>
        public static double Avg<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, double?>> selector, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return source.Function("Avg", selector, predicate);
        }

        /// <summary>
        /// Extendable method: 获取数据源中所有对象某属性的平均值
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <returns>数据源中所有对象某属性的平均值</returns>
        public static double Avg<TModel>(this IDbAsView<TModel> source, Expression<Func<TModel, double>> selector)
            where TModel : class, new()
        {
            return source.Function("Avg", selector);
        }
        /// <summary>
        /// Extendable method: 获取数据源中所有对象某属性的平均值
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <returns>数据源中所有对象某属性的平均值</returns>
        public static double Avg<TModel>(this IDbAsView<TModel> source, Expression<Func<TModel, double?>> selector)
            where TModel : class, new()
        {
            return source.Function("Avg", selector);
        }
        /// <summary>
        /// Extendable method: 获取数据源中符合条件的对象某属性的平均值
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>数据源中符合条件的对象某属性的平均值</returns>
        public static double Avg<TModel>(this IDbAsView<TModel> source, Expression<Func<TModel, double>> selector, Expression<Func<TModel, bool>> predicate)
            where TModel : class, new()
        {
            return source.Function("Avg", selector, predicate);
        }
        /// <summary>
        /// Extendable method: 获取数据源中符合条件的对象某属性的平均值
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>数据源中符合条件的对象某属性的平均值</returns>
        public static double Avg<TModel>(this IDbAsView<TModel> source, Expression<Func<TModel, double?>> selector, Expression<Func<TModel, bool>> predicate)
            where TModel : class, new()
        {
            return source.Function("Avg", selector, predicate);
        }

        /// <summary>
        /// Extendable method: 获取数据源中所有对象某属性的平均值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <returns>数据源中所有对象某属性的平均值</returns>
        public static decimal Avg<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, decimal>> selector)
            where TEntity : class
        {
            return source.Function("Avg", selector);
        }
        /// <summary>
        /// Extendable method: 获取数据源中所有对象某属性的平均值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <returns>数据源中所有对象某属性的平均值</returns>
        public static decimal Avg<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, decimal?>> selector)
            where TEntity : class
        {
            return source.Function("Avg", selector);
        }
        /// <summary>
        /// Extendable method: 获取数据源中符合条件的对象某属性的平均值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>数据源中符合条件的对象某属性的平均值</returns>
        public static decimal Avg<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, decimal>> selector, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return source.Function("Avg", selector, predicate);
        }
        /// <summary>
        /// Extendable method: 获取数据源中符合条件的对象某属性的平均值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>数据源中符合条件的对象某属性的平均值</returns>
        public static decimal Avg<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, decimal?>> selector, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return source.Function("Avg", selector, predicate);
        }

        /// <summary>
        /// Extendable method: 获取数据源中所有对象某属性的平均值
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <returns>数据源中所有对象某属性的平均值</returns>
        public static decimal Avg<TModel>(this IDbAsView<TModel> source, Expression<Func<TModel, decimal>> selector)
            where TModel : class, new()
        {
            return source.Function("Avg", selector);
        }
        /// <summary>
        /// Extendable method: 获取数据源中所有对象某属性的平均值
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <returns>数据源中所有对象某属性的平均值</returns>
        public static decimal Avg<TModel>(this IDbAsView<TModel> source, Expression<Func<TModel, decimal?>> selector)
            where TModel : class, new()
        {
            return source.Function("Avg", selector);
        }
        /// <summary>
        /// Extendable method: 获取数据源中符合条件的对象某属性的平均值
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>数据源中符合条件的对象某属性的平均值</returns>
        public static decimal Avg<TModel>(this IDbAsView<TModel> source, Expression<Func<TModel, decimal>> selector, Expression<Func<TModel, bool>> predicate)
            where TModel : class, new()
        {
            return source.Function("Avg", selector, predicate);
        }
        /// <summary>
        /// Extendable method: 获取数据源中符合条件的对象某属性的平均值
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>数据源中符合条件的对象某属性的平均值</returns>
        public static decimal Avg<TModel>(this IDbAsView<TModel> source, Expression<Func<TModel, decimal?>> selector, Expression<Func<TModel, bool>> predicate)
            where TModel : class, new()
        {
            return source.Function("Avg", selector, predicate);
        }

        /// <summary>
        /// Extendable method: 统计数据源中所有元素某属性值的和
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <returns>数据源中所有元素某属性值的和</returns>
        public static int Sum<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, int>> selector)
            where TEntity : class
        {
            return source.Function("Sum", selector);
        }
        /// <summary>
        /// Extendable method: 统计数据源中所有元素某属性值的和
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <returns>数据源中所有元素某属性值的和</returns>
        public static int Sum<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, int?>> selector)
            where TEntity : class
        {
            return source.Function("Sum", selector);
        }
        /// <summary>
        /// Extendable method: 统计数据源中符合条件的元素属性的和
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>数据源中符合条件的元素某属性的和</returns>
        public static int Sum<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, int>> selector, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return source.Function("Sum", selector, predicate);
        }
        /// <summary>
        /// Extendable method: 统计数据源中符合条件的元素属性的和
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>数据源中符合条件的元素某属性的和</returns>
        public static int Sum<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, int?>> selector, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return source.Function("Sum", selector, predicate);
        }

        /// <summary>
        /// Extendable method: 统计数据源中所有元素某属性值的和
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <returns>数据源中所有元素某属性值的和</returns>
        public static int Sum<TModel>(this IDbAsView<TModel> source, Expression<Func<TModel, int>> selector)
            where TModel : class, new()
        {
            return source.Function("Sum", selector);
        }
        /// <summary>
        /// Extendable method: 统计数据源中所有元素某属性值的和
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <returns>数据源中所有元素某属性值的和</returns>
        public static int Sum<TModel>(this IDbAsView<TModel> source, Expression<Func<TModel, int?>> selector)
            where TModel : class, new()
        {
            return source.Function("Sum", selector);
        }
        /// <summary>
        /// Extendable method: 统计数据源中符合条件的元素属性的和
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>数据源中符合条件的元素某属性的和</returns>
        public static int Sum<TModel>(this IDbAsView<TModel> source, Expression<Func<TModel, int>> selector, Expression<Func<TModel, bool>> predicate)
            where TModel : class, new()
        {
            return source.Function("Sum", selector, predicate);
        }
        /// <summary>
        /// Extendable method: 统计数据源中符合条件的元素属性的和
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>数据源中符合条件的元素某属性的和</returns>
        public static int Sum<TModel>(this IDbAsView<TModel> source, Expression<Func<TModel, int?>> selector, Expression<Func<TModel, bool>> predicate)
            where TModel : class, new()
        {
            return source.Function("Sum", selector, predicate);
        }

        /// <summary>
        /// Extendable method: 统计数据源中所有元素某属性值的和
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <returns>数据源中所有元素某属性值的和</returns>
        public static long Sum<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, long>> selector)
            where TEntity : class
        {
            return source.Function("Sum", selector);
        }
        /// <summary>
        /// Extendable method: 统计数据源中所有元素某属性值的和
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <returns>数据源中所有元素某属性值的和</returns>
        public static long Sum<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, long?>> selector)
            where TEntity : class
        {
            return source.Function("Sum", selector);
        }
        /// <summary>
        /// Extendable method: 统计数据源中符合条件的元素属性的和
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>数据源中符合条件的元素某属性的和</returns>
        public static long Sum<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, long>> selector, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return source.Function("Sum", selector, predicate);
        }
        /// <summary>
        /// Extendable method: 统计数据源中符合条件的元素属性的和
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>数据源中符合条件的元素某属性的和</returns>
        public static long Sum<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, long?>> selector, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return source.Function("Sum", selector, predicate);
        }

        /// <summary>
        /// Extendable method: 统计数据源中所有元素某属性值的和
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <returns>数据源中所有元素某属性值的和</returns>
        public static long Sum<TModel>(this IDbAsView<TModel> source, Expression<Func<TModel, long>> selector)
            where TModel : class, new()
        {
            return source.Function("Sum", selector);
        }
        /// <summary>
        /// Extendable method: 统计数据源中所有元素某属性值的和
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <returns>数据源中所有元素某属性值的和</returns>
        public static long Sum<TModel>(this IDbAsView<TModel> source, Expression<Func<TModel, long?>> selector)
            where TModel : class, new()
        {
            return source.Function("Sum", selector);
        }
        /// <summary>
        /// Extendable method: 统计数据源中符合条件的元素属性的和
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>数据源中符合条件的元素某属性的和</returns>
        public static long Sum<TModel>(this IDbAsView<TModel> source, Expression<Func<TModel, long>> selector, Expression<Func<TModel, bool>> predicate)
            where TModel : class, new()
        {
            return source.Function("Sum", selector, predicate);
        }
        /// <summary>
        /// Extendable method: 统计数据源中符合条件的元素属性的和
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>数据源中符合条件的元素某属性的和</returns>
        public static long Sum<TModel>(this IDbAsView<TModel> source, Expression<Func<TModel, long?>> selector, Expression<Func<TModel, bool>> predicate)
            where TModel : class, new()
        {
            return source.Function("Sum", selector, predicate);
        }

        /// <summary>
        /// Extendable method: 统计数据源中所有元素某属性值的和
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <returns>数据源中所有元素某属性值的和</returns>
        public static double Sum<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, double>> selector)
            where TEntity : class
        {
            return source.Function("Sum", selector);
        }
        /// <summary>
        /// Extendable method: 统计数据源中所有元素某属性值的和
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源函数执行器</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <returns>数据源中所有元素某属性值的和</returns>
        public static double Sum<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, double?>> selector)
            where TEntity : class
        {
            return source.Function("Sum", selector);
        }
        /// <summary>
        /// Extendable method: 统计数据源中符合条件的元素某属性的和
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>数据源中符合条件的元素某属性的和</returns>
        public static double Sum<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, double>> selector, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return source.Function("Sum", selector, predicate);
        }
        /// <summary>
        /// Extendable method: 统计数据源中符合条件的元素某属性的和
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>数据源中符合条件的元素某属性的和</returns>
        public static double Sum<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, double?>> selector, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return source.Function("Sum", selector, predicate);
        }

        /// <summary>
        /// Extendable method: 统计数据源中所有元素某属性值的和
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <returns>数据源中所有元素某属性值的和</returns>
        public static double Sum<TModel>(this IDbAsView<TModel> source, Expression<Func<TModel, double>> selector)
            where TModel : class, new()
        {
            return source.Function("Sum", selector);
        }
        /// <summary>
        /// Extendable method: 统计数据源中所有元素某属性值的和
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <param name="source">数据源函数执行器</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <returns>数据源中所有元素某属性值的和</returns>
        public static double Sum<TModel>(this IDbAsView<TModel> source, Expression<Func<TModel, double?>> selector)
            where TModel : class, new()
        {
            return source.Function("Sum", selector);
        }
        /// <summary>
        /// Extendable method: 统计数据源中符合条件的元素某属性的和
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>数据源中符合条件的元素某属性的和</returns>
        public static double Sum<TModel>(this IDbAsView<TModel> source, Expression<Func<TModel, double>> selector, Expression<Func<TModel, bool>> predicate)
            where TModel : class, new()
        {
            return source.Function("Sum", selector, predicate);
        }
        /// <summary>
        /// Extendable method: 统计数据源中符合条件的元素某属性的和
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>数据源中符合条件的元素某属性的和</returns>
        public static double Sum<TModel>(this IDbAsView<TModel> source, Expression<Func<TModel, double?>> selector, Expression<Func<TModel, bool>> predicate)
            where TModel : class, new()
        {
            return source.Function("Sum", selector, predicate);
        }

        /// <summary>
        /// Extendable method: 统计数据源中所有元素某属性值的和
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <returns>数据源中所有元素某属性值的和</returns>
        public static decimal Sum<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, decimal>> selector)
            where TEntity : class
        {
            return source.Function("Sum", selector);
        }
        /// <summary>
        /// Extendable method: 统计数据源中所有元素某属性值的和
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源函数执行器</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <returns>数据源中所有元素某属性值的和</returns>
        public static decimal Sum<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, decimal?>> selector)
            where TEntity : class
        {
            return source.Function("Sum", selector);
        }
        /// <summary>
        /// Extendable method: 统计数据源中符合条件的元素某属性的和
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>数据源中符合条件的元素某属性的和</returns>
        public static decimal Sum<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, decimal>> selector, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return source.Function("Sum", selector, predicate);
        }
        /// <summary>
        /// Extendable method: 统计数据源中符合条件的元素某属性的和
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>数据源中符合条件的元素某属性的和</returns>
        public static decimal Sum<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, decimal?>> selector, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return source.Function("Sum", selector, predicate);
        }

        /// <summary>
        /// Extendable method: 统计数据源中所有元素某属性值的和
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <returns>数据源中所有元素某属性值的和</returns>
        public static decimal Sum<TModel>(this IDbAsView<TModel> source, Expression<Func<TModel, decimal>> selector)
            where TModel : class, new()
        {
            return source.Function("Sum", selector);
        }
        /// <summary>
        /// Extendable method: 统计数据源中所有元素某属性值的和
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <param name="source">数据源函数执行器</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <returns>数据源中所有元素某属性值的和</returns>
        public static decimal Sum<TModel>(this IDbAsView<TModel> source, Expression<Func<TModel, decimal?>> selector)
            where TModel : class, new()
        {
            return source.Function("Sum", selector);
        }
        /// <summary>
        /// Extendable method: 统计数据源中符合条件的元素某属性的和
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>数据源中符合条件的元素某属性的和</returns>
        public static decimal Sum<TModel>(this IDbAsView<TModel> source, Expression<Func<TModel, decimal>> selector, Expression<Func<TModel, bool>> predicate)
            where TModel : class, new()
        {
            return source.Function("Sum", selector, predicate);
        }
        /// <summary>
        /// Extendable method: 统计数据源中符合条件的元素某属性的和
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>数据源中符合条件的元素某属性的和</returns>
        public static decimal Sum<TModel>(this IDbAsView<TModel> source, Expression<Func<TModel, decimal?>> selector, Expression<Func<TModel, bool>> predicate)
            where TModel : class, new()
        {
            return source.Function("Sum", selector, predicate);
        }

        /// <summary>
        /// Extendable method: 统计数据源中所有元素个数
        /// </summary>
        /// <typeparam name="TEntity">对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <returns>元素个数</returns>
        public static int Count<TEntity>(this IDbSource<TEntity> source)
            where TEntity : class
        {
            // 非空检查
            Check.ArgumentNull(source, nameof(source));
            // 创建DbScaler
            IDbScalar dbScaler = source.Factory.CreateScalar(source, "COUNT");
            // 执行统计查询获取元素数量
            return TypeHelper.ConvertTo<int>(dbScaler.Execute());
        }
        /// <summary>
        /// Extendable method: 统计数据源中符合条件的元素个数
        /// </summary>
        /// <typeparam name="TEntity">对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="predicate">指定与对象属性比较的表达式</param>
        /// <returns>元素个数</returns>
        public static int Count<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return source.Where(predicate).Count();
        }
        /// <summary>
        /// Extendable method: 统计数据源中所有元素个数
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <returns>元素个数</returns>
        public static int Count<TModel>(this IDbAsView<TModel> source)
            where TModel : class, new()
        {
            // 非空检查
            Check.ArgumentNull(source, nameof(source));
            // 创建DbScaler
            IDbScalar dbScaler = source.Factory.CreateScalar(source, "COUNT");
            // 执行统计查询获取元素数量
            return TypeHelper.ConvertTo<int>(dbScaler.Execute());
        }
        /// <summary>
        /// Extendable method: 统计数据源中符合条件的元素个数
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="predicate">指定与对象属性比较的表达式</param>
        /// <returns>元素个数</returns>
        public static int Count<TModel>(this IDbAsView<TModel> source, Expression<Func<TModel, bool>> predicate)
            where TModel : class, new()
        {
            return source.Where(predicate).Count();
        }

        /// <summary>
        /// Extendable method: 统计数据源中所有的元素个数
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
        /// Extendable method: 统计数据源中符合条件的元素个数
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
        /// Extendable method: 统计数据源中所有的元素个数
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <returns>元素个数</returns>
        public static long LongCount<TModel>(this IDbAsView<TModel> source)
            where TModel : class, new()
        {
            //检查数据源是否为空
            Check.ArgumentNull(source, nameof(source));
            //创建DbScaler
            IDbScalar dbScaler = source.Factory.CreateScalar(source, "COUNT");
            //执行统计查询获取元素数量
            return TypeHelper.ConvertTo<long>(dbScaler.Execute());
        }
        /// <summary>
        /// Extendable method: 统计数据源中符合条件的元素个数
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="predicate">指定与对象属性比较的表达式</param>
        /// <returns>元素个数</returns>
        public static long LongCount<TModel>(this IDbAsView<TModel> source, Expression<Func<TModel, bool>> predicate)
            where TModel : class, new()
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
        /// Extendable method: 统计数据源中所有元素某属性的最大值
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <typeparam name="TProperty">属性类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定元素某属性的表达式</param>
        /// <returns>数据源中所有元素某属性的最大值</returns>
        public static TProperty Max<TModel, TProperty>(this IDbAsView<TModel> source, Expression<Func<TModel, TProperty>> selector)
            where TModel : class, new()
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
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <typeparam name="TProperty">属性类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定元素某属性的表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>数据源中符合条件的元素某属性的最大值</returns>
        public static TProperty Max<TModel, TProperty>(this IDbAsView<TModel> source, Expression<Func<TModel, TProperty>> selector, Expression<Func<TModel, bool>> predicate)
            where TModel : class, new()
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
        /// <summary>
        /// Extendable method: 统计数据源中所有元素某属性的最小值
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <typeparam name="TProperty">属性类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定元素某属性的表达式</param>
        /// <returns>数据源中所有元素某属性的最小值</returns>
        public static TProperty Min<TModel, TProperty>(this IDbAsView<TModel> source, Expression<Func<TModel, TProperty>> selector)
            where TModel : class, new()
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
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <typeparam name="TProperty">属性类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定元素某属性的表达式</param>
        /// <param name="predicate">筛选条件表达式</param>
        /// <returns>数据源中符合条件的元素某属性的最小值</returns>
        public static TProperty Min<TModel, TProperty>(this IDbAsView<TModel> source, Expression<Func<TModel, TProperty>> selector, Expression<Func<TModel, bool>> predicate)
            where TModel : class, new()
        {
            return source.Where(predicate).Min(selector);
        }
    }
}