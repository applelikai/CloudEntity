using System;
using System.Data;

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
        /// <summary>
        /// Execute and get changed rows numbers
        /// 执行获取DB受影响行数
        /// </summary>
        /// <param name="commandText">sql命令</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameters">sql参数数组</param>
        /// <returns>DB受影响行数</returns>
        int ExecuteUpdate(string commandText, CommandType commandType = CommandType.Text, params IDbDataParameter[] parameters);
    }
}
