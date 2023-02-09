using CloudEntity.CommandTrees;
using CloudEntity.Data;
using CloudEntity.Data.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CloudEntity.Internal.Data.Entity
{
    /// <summary>
    /// 视图查询数据源
    /// </summary>
    /// <typeparam name="TModel">对象类型</typeparam>
    internal class DbView<TModel> : IDbView<TModel>
        where TModel : class, new()
    {
        /// <summary>
        /// 视图查询时使用的临时表名
        /// </summary>
        private string _tableAlias;
        /// <summary>
        /// 对象访问器
        /// </summary>
        private ObjectAccessor _modelAccessor;
        /// <summary>
        /// 命令生成树工厂
        /// </summary>
        private ICommandTreeFactory _commandTreeFactory;
        /// <summary>
        /// 数据库操作对象
        /// </summary>
        private DbHelper _dbHelper;
        /// <summary>
        /// sql表达式节点集合
        /// </summary>
        private IList<INodeBuilder> _nodebuilders;
        /// <summary>
        /// sql参数集合
        /// </summary>
        private IList<IDbDataParameter> _sqlParameters;

        /// <summary>
        /// 查询数据源创建工厂
        /// </summary>
        public IDbFactory Factory { get; private set; }
        /// <summary>
        /// 查询sql
        /// </summary>
        public string InnerQuerySql { get; private set; }
        /// <summary>
        /// 查询命令生成树的表达式节点集合
        /// </summary>
        public IEnumerable<INodeBuilder> NodeBuilders
        {
            get { return _nodebuilders; }
        }
        /// <summary>
        /// Sql参数集合
        /// </summary>
        public IEnumerable<IDbDataParameter> Parameters
        {
            get { return _sqlParameters; }
        }

        /// <summary>
        /// 创建对象
        /// </summary>
        /// <param name="reader">数据流</param>
        /// <param name="columnNames">查询的列</param>
        /// <returns>视图模型对象</returns>
        private TModel CreateModel(IDataReader reader, string[] columnNames)
        {
            //创建模型对象
            TModel model = new TModel();
            //为模型对象每个属性赋值
            foreach (string propertyName in columnNames)
            {
                //获取值
                object value = reader[propertyName];
                if (value is DBNull)
                    value = null;
                //为模型对象当前属性赋值
                _modelAccessor.TrySetValue(propertyName, model, reader[propertyName]);
            }
            //返回模型对象
            return model;
        }

        /// <summary>
        /// 创建视图查询数据源
        /// </summary>
        /// <param name="dbFactory">查询数据源创建工厂</param>
        /// <param name="dbHelper">操作数据库的对象</param>
        /// <param name="commandTreeFactory">命令生成树工厂</param>
        /// <param name="innerQuerySql">查询sql</param>
        public DbView(IDbFactory dbFactory, DbHelper dbHelper, ICommandTreeFactory commandTreeFactory, string innerQuerySql)
        {
            //非空检查
            Check.ArgumentNull(dbFactory, nameof(dbFactory));
            Check.ArgumentNull(dbHelper, nameof(dbHelper));
            Check.ArgumentNull(commandTreeFactory, nameof(commandTreeFactory));
            Check.ArgumentNull(innerQuerySql, nameof(innerQuerySql));
            // 初始化
            _nodebuilders = new List<INodeBuilder>();
            _sqlParameters = new List<IDbDataParameter>();
            _modelAccessor = ObjectAccessor.GetAccessor(typeof(TModel));
            _tableAlias = typeof(TModel).Name.ToLower();
            //赋值
            _dbHelper = dbHelper;
            _commandTreeFactory = commandTreeFactory;
            this.Factory = dbFactory;
            this.InnerQuerySql = innerQuerySql;
        }
        /// <summary>
        /// 添加sql表达式节点
        /// </summary>
        /// <param name="nodeBuilder">sql表达式节点</param>
        public void AddNodeBuilder(INodeBuilder nodeBuilder)
        {
            _nodebuilders.Add(nodeBuilder);
        }
        /// <summary>
        /// 添加sql表达式节点列表
        /// </summary>
        /// <param name="nodeBuilders">sql表达式节点列表</param>
        public void AddNodeBuilders(IEnumerable<INodeBuilder> nodeBuilders)
        {
            foreach (INodeBuilder nodeBuilder in nodeBuilders)
                _nodebuilders.Add(nodeBuilder);
        }
        /// <summary>
        /// 添加sql参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="value">参数值</param>
        public void AddSqlParameter(string name, object value)
        {
            // 创建sql参数
            IDbDataParameter sqlParameter = _dbHelper.Parameter(name, value);
            // 添加sql参数
            _sqlParameters.Add(sqlParameter);
        }
        /// <summary>
        /// 添加sql参数
        /// </summary>
        /// <param name="sqlParameter">sql参数</param>
        public void AddSqlParameter(IDbDataParameter sqlParameter)
        {
            _sqlParameters.Add(sqlParameter);
        }
        /// <summary>
        /// 添加sql参数列表
        /// </summary>
        /// <param name="sqlParameters">sql参数列表</param>
        public void AddSqlParameters(IEnumerable<IDbDataParameter> sqlParameters)
        {
            foreach (IDbDataParameter sqlParameter in sqlParameters)
                _sqlParameters.Add(sqlParameter);
        }
        /// <summary>
        /// 获取枚举器
        /// </summary>
        /// <returns>枚举器</returns>
        public IEnumerator<TModel> GetEnumerator()
        {
            // 创建CommandTree
            ICommandTree queryTree = _commandTreeFactory.GetWithAsQueryTree(this.InnerQuerySql, _tableAlias, this.NodeBuilders);
            // 执行查询
            foreach (TModel model in _dbHelper.GetResults(this.CreateModel, queryTree.Compile(), parameters: this.Parameters.ToArray()))
                yield return model;
        }
        /// <summary>
        /// 获取枚举器
        /// </summary>
        /// <returns>枚举器</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
