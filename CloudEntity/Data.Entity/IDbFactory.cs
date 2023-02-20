using System;
using System.Linq.Expressions;

namespace CloudEntity.Data.Entity
{
    /// <summary>
    /// 创建数据操作对象的工厂
    /// Apple_Li 李凯 15150598493
    /// 最后修改时间：2023/02/17 11:06
    /// </summary>
    public interface IDbFactory
    {
        #region 创建统计查询数据源
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
        #endregion
        #region 创建查询数据源
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
        /// <param name="source">基础数据源</param>
        /// <returns>新的查询数据源</returns>
        IDbQuery<TEntity> CreateQuery<TEntity>(IDbSource<TEntity> source)
            where TEntity : class;
        /// <summary>
        /// 创建新的查询数据源
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">查询数据源</param>
        /// <returns>新的查询数据源</returns>
        IDbQuery<TEntity> CreateQuery<TEntity>(IDbQuery<TEntity> source)
            where TEntity : class;
        /// <summary>
        /// 创建TOP实体查询数据源
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="topCount">查询的前几条的元素数量</param>
        /// <returns>TOP实体查询数据源</returns>
        IDbTopQuery<TEntity> CreateTopQuery<TEntity>(IDbQuery<TEntity> source, int topCount)
            where TEntity : class;
        #endregion
        #region 创建分页查询数据源
        /// <summary>
        /// 创建分页查询数据源
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">排好序的数据源</param>
        /// <param name="pageSize">每页元素数量</param>
        /// <param name="pageIndex">当前第几页</param>
        /// <returns>分页查询数据源</returns>
        IDbPagedQuery<TEntity> CreatePagedQuery<TEntity>(IDbQuery<TEntity> source, int pageSize, int pageIndex)
            where TEntity : class;
        /// <summary>
        /// 创建分页查询数据源
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="orderBySelector">指定排序属性的表达式</param>
        /// <param name="pageSize">每页元素数量</param>
        /// <param name="pageIndex">当前第几页</param>
        /// <param name="isDesc">是否降序</param>
        /// <returns>分页查询数据源</returns>
        IDbPagedQuery<TEntity> CreatePagedQuery<TEntity>(IDbQuery<TEntity> source, Expression<Func<TEntity, object>> orderBySelector, int pageSize, int pageIndex, bool isDesc = false)
            where TEntity : class;
        #endregion
        #region 创建包含项或选定项查询数据源
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
        #endregion
        #region 创建sql视图查询数据源
        /// <summary>
        /// 创建视图查询数据源
        /// </summary>
        /// <typeparam name="TModel">视图对象</typeparam>
        /// <param name="source">视图查询数据源</param>
        /// <returns>新的视图查询数据源</returns>
        IDbView<TModel> CreateView<TModel>(IDbView<TModel> source)
            where TModel : class, new();
        #endregion
    }
}
