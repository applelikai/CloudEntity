using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace CloudEntity.Data.Entity
{
    /// <summary>
    /// 视图查询数据源
    /// [作者：Apple_Li 李凯 15150598493]
    /// </summary>
    /// <typeparam name="TModel">对象类型</typeparam>
    public interface IDbAsView<TModel> : IDbBase, IEnumerable<TModel>
        where TModel : class, new()
    {
        /// <summary>
        /// 查询数据源创建工厂
        /// </summary>
        IDbFactory Factory { get; }
        /// <summary>
        /// 查询sql
        /// </summary>
        string InnerQuerySql { get; }

        /// <summary>
        /// 设置数据源数据检索条件
        /// </summary>
        /// <param name="predicate">检索条件表达式</param>
        /// <returns>视图查询数据源（还是原来的数据源并未复制）</returns>
        IDbAsView<TModel> SetWhere(Expression<Func<TModel, bool>> predicate);
        /// <summary>
        /// 设置数据源数据检索条件
        /// </summary>
        /// <param name="selector">指定对象成员表达式</param>
        /// <param name="sqlPredicate">sql条件</param>
        /// <typeparam name="TProperty">模型属性类型</typeparam>
        /// <returns>视图查询数据源（还是原来的数据源并未复制）</returns>
        IDbAsView<TModel> SetWhere<TProperty>(Expression<Func<TModel, TProperty>> selector, string sqlPredicate);
        /// <summary>
        /// 设置数据源数据检索条件
        /// </summary>
        /// <param name="selector">指定对象成员表达式</param>
        /// <param name="sqlPredicate">sql条件</param>
        /// <param name="sqlParameters">sql参数数组</param>
        /// <typeparam name="TProperty">模型属性类型</typeparam>
        /// <returns>视图查询数据源（还是原来的数据源并未复制）</returns>
        IDbAsView<TModel> SetWhere<TProperty>(Expression<Func<TModel, TProperty>> selector, string sqlPredicate, params IDbDataParameter[] sqlParameters);
        /// <summary>
        /// 设置数据源数据检索条件
        /// </summary>
        /// <param name="selector">指定对象成员表达式</param>
        /// <param name="sqlFormat">sql条件格式化字符串</param>
        /// <param name="values">sql参数值数组</param>
        /// <typeparam name="TProperty">模型属性类型</typeparam>
        /// <returns>视图查询数据源（还是原来的数据源并未复制）</returns>
        IDbAsView<TModel> SetWhere<TProperty>(Expression<Func<TModel, TProperty>> selector, string sqlFormat, params TProperty[] values);
        /// <summary>
        /// 为数据源设置排序条件
        /// </summary>
        /// <param name="keySelector">指定排序项的表达式</param>
        /// <param name="isDesc">是否为降序</param>
        /// <typeparam name="TKey">排序项类型</typeparam>
        /// <returns>视图查询数据源（还是原来的数据源并未复制）</returns>
        IDbAsView<TModel> SetSort<TKey>(Expression<Func<TModel, TKey>> keySelector, bool isDesc = false);
        /// <summary>
        /// 为数据源重新设置排序条件（之前的排序条件会被清空）
        /// </summary>
        /// <param name="keySelector">指定排序项的表达式</param>
        /// <param name="isDesc">是否为降序</param>
        /// <typeparam name="TKey">排序项类型</typeparam>
        /// <returns>视图查询数据源（还是原来的数据源并未复制）</returns>
        IDbAsView<TModel> SetSortBy<TKey>(Expression<Func<TModel, TKey>> keySelector, bool isDesc = false);
    }
}
