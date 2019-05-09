using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace CloudEntity.Data.Entity
{
    /// <summary>
    /// 排序扩展类
    /// 李凯 Apple_Li
    /// </summary>
    public static class ExtendSort
    {
        /// <summary>
        /// Extendable method: 对查询数据源升序排序
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TKey">实体属性类型</typeparam>
        /// <param name="source">排好序的数据源</param>
        /// <param name="keySelector">指定某属性的表达式</param>
        /// <returns>排好序的数据源</returns>
        public static IDbSortedQuery<TEntity> OrderBy<TEntity, TKey>(this IDbQuery<TEntity> source, Expression<Func<TEntity, TKey>> keySelector)
            where TEntity : class
        {
            //非空验证
            Check.ArgumentNull(source, nameof(source));
            Check.ArgumentNull(keySelector, nameof(keySelector));
            //创建新的查询对象
            return source.Factory.CreateSortedQuery(source, keySelector);
        }
        /// <summary>
        /// Extendable method: 对查询数据源降序排序
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TKey">实体属性类型</typeparam>
        /// <param name="source">排好序的数据源</param>
        /// <param name="keySelector">指定某属性的表达式</param>
        /// <returns>排好序的数据源</returns>
        public static IDbSortedQuery<TEntity> OrderByDescending<TEntity, TKey>(this IDbQuery<TEntity> source, Expression<Func<TEntity, TKey>> keySelector)
            where TEntity : class
        {
            //非空验证
            Check.ArgumentNull(source, nameof(source));
            Check.ArgumentNull(keySelector, nameof(keySelector));
            //创建新的查询对象
            return source.Factory.CreateSortedQuery(source, keySelector, true);
        }
        /// <summary>
        /// Extendable method: 对排好序的数据源再次升序排序
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TKey">实体属性类型</typeparam>
        /// <param name="source">排好序的数据源</param>
        /// <param name="keySelector">指定某属性的表达式</param>
        /// <returns>再次排好序的数据源</returns>
        public static IDbSortedQuery<TEntity> ThenBy<TEntity, TKey>(this IDbSortedQuery<TEntity> source, Expression<Func<TEntity, TKey>> keySelector)
            where TEntity : class
        {
            //非空验证
            Check.ArgumentNull(source, nameof(source));
            Check.ArgumentNull(keySelector, nameof(keySelector));
            //创建新的查询对象
            return source.Factory.CreateSortedQuery(source, keySelector);
        }
        /// <summary>
        /// Extendable method: 对排好序的数据源再次降序排序
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TKey">实体属性类型</typeparam>
        /// <param name="source">排好序的数据源</param>
        /// <param name="keySelector">指定某属性的表达式</param>
        /// <returns>再次排好序的数据源</returns>
        public static IDbSortedQuery<TEntity> ThenByDescending<TEntity, TKey>(this IDbSortedQuery<TEntity> source, Expression<Func<TEntity, TKey>> keySelector)
            where TEntity : class
        {
            //非空验证
            Check.ArgumentNull(source, nameof(source));
            Check.ArgumentNull(keySelector, nameof(keySelector));
            //创建新的查询对象
            return source.Factory.CreateSortedQuery(source, keySelector, true);
        }

        /// <summary>
        /// Extendable method: 对视图查询数据源按某属性升序排序
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <typeparam name="TKey">对象属性类型</typeparam>
        /// <param name="source">视图查询数据源</param>
        /// <param name="keySelector">指定对象某属性的表达式(根据此属性排序)</param>
        /// <returns>排好序的视图查询数据源</returns>
        public static IDbView<TModel> OrderBy<TModel, TKey>(this IDbView<TModel> source, Expression<Func<TModel, TKey>> keySelector)
            where TModel : class, new()
        {
            //非空验证
            Check.ArgumentNull(source, nameof(source));
            Check.ArgumentNull(keySelector, nameof(keySelector));
            //创建新的视图查询数据源并返回
            return source.Factory.CreateSortedView(source, keySelector);
        }
        /// <summary>
        /// Extendable method: 对视图查询数据源按某属性降序排序
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
            Check.ArgumentNull(keySelector, nameof(keySelector));
            //创建新的视图查询数据源并返回
            return source.Factory.CreateSortedView(source, keySelector, false);
        }
    }
}
