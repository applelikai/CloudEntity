using System;
using System.Data;

namespace CloudEntity.Data.Entity
{
    /// <summary>
    /// 数据容器接口
    /// </summary>
    public interface IDbContainer
    {
        /// <summary>
        /// 操作数据库的DbHelper
        /// </summary>
        DbHelper DbHelper { get; }

        /// <summary>
        /// 初始化某实体类所Mapping的Table
        /// </summary>
        /// <param name="entityType">某实体类的类型</param>
        void InitTable(Type entityType);
        /// <summary>
        /// 初始化某实体类所Mapping的Table
        /// </summary>
        /// <typeparam name="TEntity">实体类的类型</typeparam>
        void InitTable<TEntity>()
            where TEntity : class;
        /// <summary>
        /// 创建事故执行器
        /// </summary>
        /// <returns>事故执行器</returns>
        IDbExecutor CreateExecutor();
        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <returns>数据列表</returns>
        IDbList<TEntity> List<TEntity>()
            where TEntity : class;
        /// <summary>
        /// 创建实体操作器
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="transaction">事物出来对象</param>
        /// <returns>实体操作器</returns>
        IDbOperator<TEntity> CreateOperator<TEntity>(IDbTransaction transaction)
            where TEntity : class;
    }
}
