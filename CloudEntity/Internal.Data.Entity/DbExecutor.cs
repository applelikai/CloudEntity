using CloudEntity.Data.Entity;
using System;
using System.Collections.Generic;
using System.Data;

namespace CloudEntity.Internal.Data.Entity
{
    /// <summary>
    /// 事故执行器
    /// </summary>
    internal class DbExecutor : IDbExecutor
    {
        private bool isCommit;                                  //是否已提交
        private IDbContainer container;                         //数据容器
        private IDbConnection connection;                       //数据库连接
        private IDbTransaction transaction;                     //事故
        private IDictionary<Type, object> operators;            //操作器字典
        private object operatorsLocker = new object();          //控制操作器字典的线程锁
        private static object transactionLocker = new object(); //事故执行线程锁
        
        /// <summary>
        /// 创建事故执行器
        /// </summary>
        /// <param name="container">数据容器</param>
        internal DbExecutor(IDbContainer container)
        {
            //赋值
            this.container = container;
            this.operators = new Dictionary<Type, dynamic>();
            //开启事故
            this.container.DbHelper.BeginTransaction(out this.connection, out this.transaction);
        }

        /// <summary>
        /// 提交
        /// </summary>
        public void Commit()
        {
            lock (DbExecutor.transactionLocker)
            {
                this.transaction.Commit();
                this.isCommit = true;
            }
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (!this.isCommit)
                this.transaction.Rollback();
            this.container.DbHelper.EndTransaction(this.connection, this.transaction);
        }
        /// <summary>
        /// 获取当前实体的数据操作对象
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <returns>当前实体的数据操作对象</returns>
        public IDbOperator<TEntity> Operator<TEntity>()
            where TEntity : class
        {
            Start:
            //若字典中包含当前操作对象，直接返回
            if (this.operators.ContainsKey(typeof(TEntity)))
                return this.operators[typeof(TEntity)] as IDbOperator<TEntity>;
            //进入线程安全模式
            lock (this.operatorsLocker)
            {
                //若当前字典中不包含当前操作对象
                if (!this.operators.ContainsKey(typeof(TEntity)))
                {
                    //创建并注册至字典中
                    this.operators.Add(typeof(TEntity), this.container.CreateOperator<TEntity>(transaction));
                }
                //回到开始
                goto Start;
            }
        }
        /// <summary>
        /// Execute and get result
        /// 执行并获取单个结果
        /// </summary>
        /// <param name="commandText">sql命令</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameters">sql参数数组</param>
        /// <returns>单个结果</returns>
        public object GetScalar(string commandText, CommandType commandType = CommandType.Text, params IDbDataParameter[] parameters)
        {
            return this.container.DbHelper.GetScalar(commandText, this.transaction, commandType, parameters);
        }
        /// <summary>
        /// Execute and get changed rows numbers
        /// 执行获取DB受影响行数
        /// </summary>
        /// <param name="commandText">sql命令</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameters">sql参数数组</param>
        /// <returns>DB受影响行数</returns>
        public int ExecuteUpdate(string commandText, CommandType commandType = CommandType.Text, params IDbDataParameter[] parameters)
        {
            return this.container.DbHelper.ExecuteUpdate(commandText, this.transaction, commandType, parameters);
        }
    }
}
