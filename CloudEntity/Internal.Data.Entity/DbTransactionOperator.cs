﻿using CloudEntity.CommandTrees;
using CloudEntity.Data;
using CloudEntity.Data.Entity;
using CloudEntity.Mapping;
using System.Data;

namespace CloudEntity.Internal.Data.Entity
{
    /// <summary>
    /// 带事故执行的实体数据操作类
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    internal class DbTransactionOperator<TEntity> : DbOperator<TEntity>
        where TEntity : class
    {
        //事故
        private IDbTransaction transaction;

        /// <summary>
        /// 执行更新
        /// </summary>
        /// <param name="commandText">sql命令</param>
        /// <param name="parameters">sql参数数组</param>
        /// <returns>受影响的行数</returns>
        protected override int ExecuteUpdate(string commandText, IDbDataParameter[] parameters)
        {
            return base.DbHelper.ExecuteUpdate(commandText, this.transaction, parameters: parameters);
        }

        /// <summary>
        /// 创建带事故执行的实体数据操作对象
        /// </summary>
        /// <param name="factory">数据源创建工厂</param>
        /// <param name="dbHelper">数据库操作对象</param>
        /// <param name="commandTreeFactory">创建CommandTree的工厂</param>
        /// <param name="mapperContainer">mapper容器</param>
        /// <param name="transaction">事故</param>
        public DbTransactionOperator(IDbFactory factory, DbHelper dbHelper, ICommandTreeFactory commandTreeFactory, IMapperContainer mapperContainer, IDbTransaction transaction)
            : base(factory, dbHelper, commandTreeFactory, mapperContainer)
        {
            this.transaction = transaction;
        }
    }
}
