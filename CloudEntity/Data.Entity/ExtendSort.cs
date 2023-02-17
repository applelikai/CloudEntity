using System;
using System.Linq.Expressions;

namespace CloudEntity.Data.Entity
{
    /// <summary>
    /// 排序扩展类
    /// 李凯 Apple_Li 15150598493
    /// 最后修改时间：2023/02/05
    /// </summary>
    public static class ExtendSort
    {
        /// <summary>
        /// Extendable method: 复制来源数据源 并为新数据源重新设置 数据排序条件
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TKey">实体属性类型</typeparam>
        /// <param name="source">排好序的数据源</param>
        /// <param name="keySelector">指定某属性的表达式</param>
        /// <returns>排好序的数据源</returns>
        public static IDbQuery<TEntity> OrderBy<TEntity, TKey>(this IDbQuery<TEntity> source, Expression<Func<TEntity, TKey>> keySelector)
            where TEntity : class
        {
            //非空验证
            Check.ArgumentNull(source, nameof(source));
            // 复制来源数据源获取新的查询数据源
            IDbQuery<TEntity> cloned = source.Factory.CreateQuery(source);
            // 为复制的数据源重新设置排序条件
            cloned.SetSortBy(keySelector);
            // 最后获取复制的查询数据源
            return cloned;
        }
        /// <summary>
        /// Extendable method: 复制来源数据源 并为新数据源重新设置 数据降序排序条件
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TKey">实体属性类型</typeparam>
        /// <param name="source">排好序的数据源</param>
        /// <param name="keySelector">指定某属性的表达式</param>
        /// <returns>排好序的数据源</returns>
        public static IDbQuery<TEntity> OrderByDescending<TEntity, TKey>(this IDbQuery<TEntity> source, Expression<Func<TEntity, TKey>> keySelector)
            where TEntity : class
        {
            //非空验证
            Check.ArgumentNull(source, nameof(source));
            // 复制来源数据源获取新的查询数据源
            IDbQuery<TEntity> cloned = source.Factory.CreateQuery(source);
            // 为复制的数据源重新设置降序排序条件
            cloned.SetSortBy(keySelector, true);
            // 最后获取复制的查询数据源
            return cloned;
        }
        /// <summary>
        /// Extendable method: 复制来源数据源 并为新数据源添加 数据排序条件
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TKey">实体属性类型</typeparam>
        /// <param name="source">排好序的数据源</param>
        /// <param name="keySelector">指定某属性的表达式</param>
        /// <returns>再次排好序的数据源</returns>
        public static IDbQuery<TEntity> ThenBy<TEntity, TKey>(this IDbQuery<TEntity> source, Expression<Func<TEntity, TKey>> keySelector)
            where TEntity : class
        {
            // 非空验证
            Check.ArgumentNull(source, nameof(source));
            // 复制来源数据源获取新的查询数据源
            IDbQuery<TEntity> cloned = source.Factory.CreateQuery(source);
            // 为复制的数据源设置排序条件
            cloned.SetSort(keySelector);
            // 最后获取复制的查询数据源
            return cloned;
        }
        /// <summary>
        /// Extendable method: 复制来源数据源 并为新数据源添加 数据降序排序条件
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TKey">实体属性类型</typeparam>
        /// <param name="source">排好序的数据源</param>
        /// <param name="keySelector">指定某属性的表达式</param>
        /// <returns>再次排好序的数据源</returns>
        public static IDbQuery<TEntity> ThenByDescending<TEntity, TKey>(this IDbQuery<TEntity> source, Expression<Func<TEntity, TKey>> keySelector)
            where TEntity : class
        {
            //非空验证
            Check.ArgumentNull(source, nameof(source));
            // 复制来源数据源获取新的查询数据源
            IDbQuery<TEntity> cloned = source.Factory.CreateQuery(source);
            // 为复制的数据源设置降序排序条件
            cloned.SetSort(keySelector, true);
            // 最后获取复制的查询数据源
            return cloned;
        }

        /// <summary>
        /// Extendable method: 复制来源数据源 并为新数据源重新设置 数据排序条件
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <typeparam name="TKey">对象属性类型</typeparam>
        /// <param name="source">视图查询数据源</param>
        /// <param name="keySelector">指定对象某属性的表达式(根据此属性排序)</param>
        /// <returns>排好序的视图查询数据源</returns>
        public static IDbView<TModel> OrderBy<TModel, TKey>(this IDbView<TModel> source, Expression<Func<TModel, TKey>> keySelector)
            where TModel : class, new()
        {
            // 非空验证
            Check.ArgumentNull(source, nameof(source));
            // 复制来源数据源获取新的视图查询数据源
            IDbView<TModel> cloned = source.Factory.CreateView(source);
            // 为复制的数据源重新设置排序条件
            cloned.SetSortBy(keySelector);
            // 最后获取复制的视图查询数据源
            return cloned;
        }
        /// <summary>
        /// Extendable method: 复制来源数据源 并为新数据源重新设置 数据降序排序条件
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <typeparam name="TKey">对象属性类型</typeparam>
        /// <param name="source">视图查询数据源</param>
        /// <param name="keySelector">指定对象某属性的表达式(根据此属性排序)</param>
        /// <returns>排好序的视图查询数据源</returns>
        public static IDbView<TModel> OrderByDescending<TModel, TKey>(this IDbView<TModel> source, Expression<Func<TModel, TKey>> keySelector)
            where TModel : class, new()
        {
            //非空验证
            Check.ArgumentNull(source, nameof(source));
            // 复制来源数据源获取新的视图查询数据源
            IDbView<TModel> cloned = source.Factory.CreateView(source);
            // 为复制的数据源重新设置降序排序条件
            cloned.SetSortBy(keySelector, true);
            // 最后获取复制的视图查询数据源
            return cloned;
        }
        /// <summary>
        /// Extendable method: 复制来源数据源 并为新数据源设置 数据排序条件
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <typeparam name="TKey">对象属性类型</typeparam>
        /// <param name="source">视图查询数据源</param>
        /// <param name="keySelector">指定对象某属性的表达式(根据此属性排序)</param>
        /// <returns>排好序的视图查询数据源</returns>
        public static IDbView<TModel> ThenBy<TModel, TKey>(this IDbView<TModel> source, Expression<Func<TModel, TKey>> keySelector)
            where TModel : class, new()
        {
            // 非空验证
            Check.ArgumentNull(source, nameof(source));
            // 复制来源数据源获取新的视图查询数据源
            IDbView<TModel> cloned = source.Factory.CreateView(source);
            // 为复制的数据源设置排序条件
            cloned.SetSort(keySelector);
            // 最后获取复制的视图查询数据源
            return cloned;
        }
        /// <summary>
        /// Extendable method: 复制来源数据源 并为新数据源设置 数据降序排序条件
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <typeparam name="TKey">对象属性类型</typeparam>
        /// <param name="source">视图查询数据源</param>
        /// <param name="keySelector">指定对象某属性的表达式(根据此属性排序)</param>
        /// <returns>排好序的视图查询数据源</returns>
        public static IDbView<TModel> ThenByDescending<TModel, TKey>(this IDbView<TModel> source, Expression<Func<TModel, TKey>> keySelector)
            where TModel : class, new()
        {
            //非空验证
            Check.ArgumentNull(source, nameof(source));
            // 复制来源数据源获取新的视图查询数据源
            IDbView<TModel> cloned = source.Factory.CreateView(source);
            // 为复制的数据源设置降序排序条件
            cloned.SetSort(keySelector, true);
            // 最后获取复制的视图查询数据源
            return cloned;
        }
    }
}
