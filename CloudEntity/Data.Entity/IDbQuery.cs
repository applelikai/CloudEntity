using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace CloudEntity.Data.Entity
{
    /// <summary>
    /// 查询数据源
    /// [作者：Apple_Li 李凯 15150598493]
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    public interface IDbQuery<TEntity> : IDbSource<TEntity>
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
        /// 重新为数据源指定需要查询的项（不指定则查询所有项）
        /// </summary>
        /// <param name="selector">指定查询项表达式</param>
        /// <typeparam name="TElement">查询项类型</typeparam>
        /// <returns>数据源（还是原来的数据源并未复制）</returns>
        IDbQuery<TEntity> SetIncludeBy<TElement>(Expression<Func<TEntity, TElement>> selector);
        /// <summary>
        /// 为数据源关联其他实体查询数据源
        /// </summary>
        /// <param name="otherSource">关联的数据源</param>
        /// <param name="selector">指定关联实体类型的属性表达式</param>
        /// <param name="predicate">关联条件表达式</param>
        /// <typeparam name="TOther">关联的实体类型</typeparam>
        /// <returns>数据源（还是原来的数据源并未复制）</returns>
        IDbQuery<TEntity> SetJoin<TOther>(IDbQuery<TOther> otherSource, Expression<Func<TEntity, TOther>> selector, Expression<Func<TEntity, TOther, bool>> predicate)
            where TOther : class;
        /// <summary>
        /// 为数据源左连接关联其他实体查询数据源
        /// </summary>
        /// <param name="otherSource">关联的数据源</param>
        /// <param name="selector">指定关联实体类型的属性表达式</param>
        /// <param name="predicate">关联条件表达式</param>
        /// <typeparam name="TOther">关联的实体类型</typeparam>
        /// <returns>数据源（还是原来的数据源并未复制）</returns>
        IDbQuery<TEntity> SetLeftJoin<TOther>(IDbQuery<TOther> otherSource, Expression<Func<TEntity, TOther>> selector, Expression<Func<TEntity, TOther, bool>> predicate)
            where TOther : class;
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
        /// 设置数据源数据检索条件
        /// </summary>
        /// <param name="selector">指定对象成员表达式</param>
        /// <param name="sqlFormat">sql条件格式化字符串</param>
        /// <param name="values">sql参数值数组</param>
        /// <typeparam name="TProperty">实体属性类型</typeparam>
        /// <returns>数据源（还是原来的数据源并未复制）</returns>
        IDbQuery<TEntity> SetWhere<TProperty>(Expression<Func<TEntity, TProperty>> selector, string sqlFormat, params TProperty[] values);
        /// <summary>
        /// 为数据源设置排序条件
        /// </summary>
        /// <param name="keySelector">指定排序项的表达式</param>
        /// <param name="isDesc">是否为降序</param>
        /// <typeparam name="TKey">排序项类型</typeparam>
        /// <returns>数据源（还是原来的数据源并未复制）</returns>
        IDbQuery<TEntity> SetSort<TKey>(Expression<Func<TEntity, TKey>> keySelector, bool isDesc = false);
        /// <summary>
        /// 为数据源重新设置排序条件（之前的排序条件会被清空）
        /// </summary>
        /// <param name="keySelector">指定排序项的表达式</param>
        /// <param name="isDesc">是否为降序</param>
        /// <typeparam name="TKey">排序项类型</typeparam>
        /// <returns>数据源（还是原来的数据源并未复制）</returns>
        IDbQuery<TEntity> SetSortBy<TKey>(Expression<Func<TEntity, TKey>> keySelector, bool isDesc = false);
        /// <summary>
        /// 将此数据源的查询结果映射为TModel对象数据源
        /// </summary>
        /// <typeparam name="TModel">TModel对象（只要是有无参构造函数的类就可以）</typeparam>
        /// <returns>TModel对象数据源</returns>
        IEnumerable<TModel> Cast<TModel>()
            where TModel : class, new();
    }
}
