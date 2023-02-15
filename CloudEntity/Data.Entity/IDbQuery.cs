using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace CloudEntity.Data.Entity
{
    /// <summary>
    /// 查询数据源
    /// Apple_Li 李凯 15150598493
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    public interface IDbQuery<TEntity> : IDbSource<TEntity>, IDbBase
        where TEntity : class
    {
        /// <summary>
        /// 当前对象的关联对象属性链接数组
        /// </summary>
        IEnumerable<PropertyLinker> PropertyLinkers { get; }

        /// <summary>
        /// 获取sql字符串
        /// </summary>
        /// <returns>sql字符串</returns>
        string ToSqlString();
        /// <summary>
        /// 设置数据源数据检索条件
        /// </summary>
        /// <param name="predicate">检索条件表达式</param>
        /// <returns>数据源（还是原来的数据源并未复制）</returns>
        IDbQuery<TEntity> SetWhere(Expression<Func<TEntity, bool>> predicate);
        /// <summary>
        /// 设置数据源数据检索条件
        /// </summary>
        /// <param name="selector">指定对象成员表达式</param>
        /// <param name="sqlPredicate">sql条件</param>
        /// <typeparam name="TProperty">实体属性类型</typeparam>
        /// <returns>数据源（还是原来的数据源并未复制）</returns>
        IDbQuery<TEntity> SetWhere<TProperty>(Expression<Func<TEntity, TProperty>> selector, string sqlPredicate);
        /// <summary>
        /// 设置数据源数据检索条件
        /// </summary>
        /// <param name="selector">指定对象成员表达式</param>
        /// <param name="sqlPredicate">sql条件</param>
        /// <param name="sqlParameters">sql参数数组</param>
        /// <typeparam name="TProperty">实体属性类型</typeparam>
        /// <returns>数据源（还是原来的数据源并未复制）</returns>
        IDbQuery<TEntity> SetWhere<TProperty>(Expression<Func<TEntity, TProperty>> selector, string sqlPredicate, params IDbDataParameter[] sqlParameters);
        /// <summary>
        /// 将此数据源的查询结果映射为TModel对象数据源
        /// </summary>
        /// <typeparam name="TModel">TModel对象（只要是有无参构造函数的类就可以）</typeparam>
        /// <returns>TModel对象数据源</returns>
        IEnumerable<TModel> Cast<TModel>()
            where TModel : class, new();
    }
}
