using CloudEntity.CommandTrees;
using CloudEntity.Data;
using CloudEntity.Data.Entity;
using CloudEntity.Mapping;
using System;
using System.Collections.Generic;
using System.Data;

namespace CloudEntity.Internal.Data.Entity
{
    /// <summary>
    /// 事务执行器
    /// [作者：Apple_Li 李凯 15150598493]
    /// </summary>
    internal class DbExecutor : IDbExecutor
    {
        /// <summary>
        /// 是否已提交
        /// </summary>
        private bool _isCommit;
        /// <summary>
        /// 数据容器
        /// </summary>
        private IDbFactory _factory;
        /// <summary>
        /// 数据库操作对象
        /// </summary>
        private IDbHelper _dbHelper;
        /// <summary>
        /// Sql命令工厂
        /// </summary>
        private ICommandFactory _commandFactory;
        /// <summary>
        /// Mapper容器
        /// </summary>
        private IMapperContainer _mapperContainer;
        /// <summary>
        /// 数据库连接
        /// </summary>
        private IDbConnection _connection;
        /// <summary>
        /// 事务
        /// </summary>
        private IDbTransaction _transaction;
        /// <summary>
        /// 操作器字典
        /// </summary>
        private IDictionary<Type, object> _operators;
        /// <summary>
        /// 控制操作器字典的线程锁
        /// </summary>
        private object _operatorsLocker;
        /// <summary>
        /// 事务执行线程锁
        /// </summary>
        private static object _transactionLocker = new object();

        /// <summary>
        /// 创建事务执行器
        /// </summary>
        /// <param name="factory">查询数据源创建工厂</param>
        /// <param name="dbHelper">数据库操作对象</param>
        /// <param name="commandFactory">Sql命令工厂</param>
        /// <param name="mapperContainer">mapper容器</param>
        internal DbExecutor(IDbFactory factory, IDbHelper dbHelper, ICommandFactory commandFactory, IMapperContainer mapperContainer)
        {
            // 赋值
            _factory = factory;
            _dbHelper = dbHelper;
            _commandFactory = commandFactory;
            _mapperContainer = mapperContainer;
            // 初始化
            _operators = new Dictionary<Type, dynamic>();
            _operatorsLocker = new object();
            // 开启事务
            _dbHelper.BeginTransaction(out _connection, out _transaction);
        }

        /// <summary>
        /// 提交
        /// </summary>
        public void Commit()
        {
            lock (_transactionLocker)
            {
                _transaction.Commit();
                _isCommit = true;
            }
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            // 若没有提交，则回滚
            if (!_isCommit)
                _transaction.Rollback();
            // 结束事务
            _dbHelper.EndTransaction(_connection, _transaction);
        }
        /// <summary>
        /// 获取当前实体的数据操作对象
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <returns>当前实体的数据操作对象</returns>
        public IDbOperator<TEntity> Operator<TEntity>()
            where TEntity : class
        {
            // 获取实体类型
            Type entityType = typeof(TEntity);
            // 开始
            Start:
            // 若字典中包含当前操作对象，直接返回
            if (_operators.ContainsKey(entityType))
                return _operators[entityType] as IDbOperator<TEntity>;
            // 进入线程安全模式
            lock (_operatorsLocker)
            {
                // 若当前字典中不包含当前操作对象
                if (!_operators.ContainsKey(entityType))
                {
                    // 创建操作对象
                    IDbOperator<TEntity> dbOperator = new DbTransactionOperator<TEntity>(_factory, _dbHelper, _commandFactory, _mapperContainer, _transaction);
                    // 添加操作对象到字典中
                    _operators.Add(entityType, dbOperator);
                }
                //回到开始
                goto Start;
            }
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
            return _dbHelper.ExecuteUpdate(commandText, _transaction, commandType, parameters);
        }
    }
}
