using System;

namespace CloudEntity.Data.Entity
{
    /// <summary>
    /// 事物执行器
    /// 李凯 Apple_Li
    /// </summary>
    public interface IDbExecutor : IDisposable
    {
        /// <summary>
        /// 提交
        /// </summary>
        void Commit();
        /// <summary>
        /// 获取当前实体的数据操作对象
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <returns>当前实体的数据操作对象</returns>
        IDbOperator<TEntity> Operator<TEntity>()
            where TEntity : class;
    }
}
