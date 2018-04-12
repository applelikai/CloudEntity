using System;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace CloudEntity.Data.Entity
{
    /// <summary>
    /// 创建数据操作对象的工厂
    /// </summary>
    public interface IDbFactory
    {
        /// <summary>
        /// 创建统计查询对象
        /// </summary>
        /// <param name="dbBase">操作数据库的基础对象</param>
        /// <param name="functionName">统计函数名</param>
        /// <returns>统计查询对象</returns>
        IDbScalar CreateScalar(IDbBase dbBase, string functionName);
        /// <summary>
        /// 创建统计查询对象
        /// </summary>
        /// <param name="dbBase">操作数据库的基础对象</param>
        /// <param name="functionName">统计函数名</param>
        /// <param name="lambdaExpression">指定对象某属性的表达式</param>
        /// <returns>统计查询对象</returns>
        IDbScalar CreateScalar(IDbBase dbBase, string functionName, LambdaExpression lambdaExpression);
        /// <summary>
        /// 创建新的查询数据源
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <returns>新的查询数据源</returns>
        IDbQuery<TEntity> CreateQuery<TEntity>()
            where TEntity : class;
        /// <summary>
        /// 创建新的查询数据源
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="predicates">查询条件表达式数组</param>
        /// <returns>新的查询数据源</returns>
        IDbQuery<TEntity> CreateQuery<TEntity>(IDbQuery<TEntity> source, params Expression<Func<TEntity, bool>>[] predicates)
            where TEntity : class;
        /// <summary>
        /// 创建新的查询对象
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="property">属性</param>
        /// <param name="whereTemplate">sql条件表达式</param>
        /// <param name="parameters">sql参数</param>
        /// <returns>新的查询对象</returns>
        IDbQuery<TEntity> CreateQuery<TEntity>(IDbQuery<TEntity> source, PropertyInfo property, string whereTemplate, params IDbDataParameter[] parameters)
            where TEntity : class;
        /// <summary>
        /// 创建根据某属性排好序的查询对象
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TKey">实体某属性类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="keySelector">指定实体对象某属性的表达式</param>
        /// <param name="isAsc">true:升序 false:降序</param>
        /// <returns>排好序的查询对象</returns>
        IDbQuery<TEntity> CreateSortedQuery<TEntity, TKey>(IDbQuery<TEntity> source, Expression<Func<TEntity, TKey>> keySelector, bool isAsc = true)
            where TEntity : class;
        /// <summary>
        /// 创建连接查询对象
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TOther">关联的实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="otherSource">关联对象的数据源</param>
        /// <param name="selector">指定关联实体类型的属性表达式</param>
        /// <param name="predicate">TEntity 与 TOther关系表达式</param>
        /// <returns>连接查询对象</returns>
        IDbQuery<TEntity> CreateJoinedQuery<TEntity, TOther>(IDbQuery<TEntity> source, IDbQuery<TOther> otherSource, Expression<Func<TEntity, TOther>> selector, Expression<Func<TEntity, TOther, bool>> predicate)
            where TEntity : class
            where TOther : class;
        /// <summary>
        /// 创建左连接查询对象
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TOther">关联的实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="otherSource">关联对象的数据源</param>
        /// <param name="selector">指定关联实体类型的属性表达式</param>
        /// <param name="predicate">TEntity 与 TOther关系表达式</param>
        /// <returns>左连接查询对象</returns>
        IDbQuery<TEntity> CreateLeftJoinedQuery<TEntity, TOther>(IDbQuery<TEntity> source, IDbQuery<TOther> otherSource, Expression<Func<TEntity, TOther>> selector, Expression<Func<TEntity, TOther, bool>> predicate)
            where TEntity : class
            where TOther : class;
        /// <summary>
        /// 创建分页查询数据源
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="orderBySelector">指定排序属性的表达式</param>
        /// <param name="pageSize">每页元素数量</param>
        /// <param name="pageIndex">当前第几页</param>
        /// <param name="isAsc">true:升序 false:降序</param>
        /// <returns>分页查询数据源</returns>
        IDbPagedQuery<TEntity> CreatePagedQuery<TEntity>(IDbQuery<TEntity> source, Expression<Func<TEntity, object>> orderBySelector, int pageSize, int pageIndex = 1, bool isAsc = true)
            where TEntity : class;
        /// <summary>
        /// 创建TOP选定项查询数据源
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="topCount">查询的前几条的元素数量</param>
        /// <returns>TOP选定项查询数据源</returns>
        IDbSelectedQuery<TEntity> CreateTopSelectedQuery<TEntity>(IDbQuery<TEntity> source, int topCount)
            where TEntity : class;
        /// <summary>
        /// 创建TOP选定项查询数据源
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TElement">结果类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="topCount">查询的前几条的元素数量</param>
        /// <param name="selector">转换实体对象到结果对象的表达式</param>
        /// <returns>TOP选定项查询数据源</returns>
        IDbSelectedQuery<TElement> CreateTopSelectedQuery<TEntity, TElement>(IDbQuery<TEntity> source, int topCount, Expression<Func<TEntity, TElement>> selector)
            where TEntity : class;
        /// <summary>
        /// 创建选定项查询数据源
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TElement">结果类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">转换实体对象到结果对象的表达式</param>
        /// <returns>选定项查询数据源</returns>
        IDbSelectedQuery<TElement> CreateSelectedQuery<TEntity, TElement>(IDbQuery<TEntity> source, Expression<Func<TEntity, TElement>> selector)
            where TEntity : class;
        /// <summary>
        /// 创建去除重复项查询数据源
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TElement">结果类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">转换实体对象到结果对象的表达式</param>
        /// <returns>去除重复项查询数据源</returns>
        IDbSelectedQuery<TElement> CreateDistinctQuery<TEntity, TElement>(IDbQuery<TEntity> source, Expression<Func<TEntity, TElement>> selector)
            where TEntity : class;
        /// <summary>
        /// 创建视图查询数据源
        /// </summary>
        /// <typeparam name="TModel">视图对象</typeparam>
        /// <param name="source">视图查询数据源</param>
        /// <param name="predicates">查询条件表达式数组</param>
        /// <returns>新的视图查询数据源</returns>
        IDbView<TModel> CreateView<TModel>(IDbView<TModel> source, params Expression<Func<TModel, bool>>[] predicates)
            where TModel : class, new();
        /// <summary>
        /// 创建视图查询数据源
        /// </summary>
        /// <typeparam name="TModel">视图模型对象</typeparam>
        /// <param name="source">视图查询数据源</param>
        /// <param name="property">属性</param>
        /// <param name="whereTemplate">sql条件表达式模板</param>
        /// <param name="parameters">sql参数数组</param>
        /// <returns>新的视图查询数据源</returns>
        IDbView<TModel> CreateView<TModel>(IDbView<TModel> source, PropertyInfo property, string whereTemplate, params IDbDataParameter[] parameters)
            where TModel : class, new();
        /// <summary>
        /// 创建根据某属性排好序的视图查询数据源
        /// </summary>
        /// <typeparam name="TModel">视图模型对象</typeparam>
        /// <typeparam name="TKey">对象某属性类型</typeparam>
        /// <param name="source">视图查询数据源</param>
        /// <param name="keySelector">指定视图模型对象某属性的表达式</param>
        /// <param name="isAsc">true:升序 false:降序</param>
        /// <returns>排好序的视图查询数据源</returns>
        IDbView<TModel> CreateSortedView<TModel, TKey>(IDbView<TModel> source, Expression<Func<TModel, TKey>> keySelector, bool isAsc = true)
            where TModel : class, new();
    }
}
