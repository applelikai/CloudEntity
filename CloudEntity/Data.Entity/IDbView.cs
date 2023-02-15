using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CloudEntity.Data.Entity
{
    /// <summary>
    /// 视图查询数据源
    /// Apple_Li 李凯 15150598493
    /// </summary>
    /// <typeparam name="TModel">对象类型</typeparam>
    public interface IDbView<TModel> : IDbBase, IEnumerable<TModel>
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
        IDbView<TModel> SetWhere(Expression<Func<TModel, bool>> predicate);
        /// <summary>
        /// 设置数据源数据检索条件
        /// </summary>
        /// <param name="selector">指定对象成员表达式</param>
        /// <param name="sqlPredicate">sql条件</param>
        /// <typeparam name="TProperty">模型属性类型</typeparam>
        /// <returns>视图查询数据源（还是原来的数据源并未复制）</returns>
        IDbView<TModel> SetWhere<TProperty>(Expression<Func<TModel, TProperty>> selector, string sqlPredicate);
    }
}
