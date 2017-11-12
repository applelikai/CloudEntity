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
        /// 重命名旧表以获取当前实体所Mapping的表
        /// </summary>
        /// <param name="entityType">实体类型</param>
        /// <param name="oldTableName">旧的表名</param>
        void RenameTable(Type entityType, string oldTableName);
        /// <summary>
        /// 删除实体类Mapping的表
        /// </summary>
        /// <param name="entityType">实体类型</param>
        void DropTable(Type entityType);
        /// <summary>
        /// 创建事故执行器
        /// </summary>
        /// <returns>事故执行器</returns>
        IDbExecutor CreateExecutor();
        /// <summary>
        /// 创建实体操作器
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="transaction">事物出来对象</param>
        /// <returns>实体操作器</returns>
        IDbOperator<TEntity> CreateOperator<TEntity>(IDbTransaction transaction) where TEntity : class;
        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <returns>数据列表</returns>
        IDbList<TEntity> List<TEntity>() where TEntity : class;
        /// <summary>
        /// 创建视图查询数据源
        /// </summary>
        /// <typeparam name="TModel">对象类型</typeparam>
        /// <param name="querySql">查询sql</param>
        /// <param name="parameters">sql参数数组</param>
        /// <returns>视图查询数据源</returns>
        IDbView<TModel> View<TModel>(string querySql, params IDbDataParameter[] parameters) where TModel : class, new();
    }
}
