using CloudEntity.CommandTrees;
using CloudEntity.Data;
using CloudEntity.Data.Entity;
using CloudEntity.Internal.CommandTrees;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace CloudEntity.Internal.Data.Entity
{
    /// <summary>
    /// 视图查询数据源
    /// Apple_Li 李凯 15150598493
    /// 最后修改时间：2023/02/15 23:11
    /// </summary>
    /// <typeparam name="TModel">对象类型</typeparam>
    internal class DbView<TModel> : DbQueryBase, IDbView<TModel>
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
        /// 创建表达式解析器的工厂
        /// </summary>
        private IPredicateParserFactory _predicateParserFactory;

        /// <summary>
        /// 查询数据源创建工厂
        /// </summary>
        public IDbFactory Factory { get; private set; }
        /// <summary>
        /// 查询sql
        /// </summary>
        public string InnerQuerySql { get; private set; }

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
        /// <param name="commandTreeFactory">命令生成树工厂</param>
        /// <param name="dbHelper">操作数据库的对象</param>
        /// <param name="predicateParserFactory">创建表达式解析器的工厂</param>
        /// <param name="innerQuerySql">查询sql</param>
        public DbView(IDbFactory dbFactory, ICommandTreeFactory commandTreeFactory, DbHelper dbHelper, IPredicateParserFactory predicateParserFactory, string innerQuerySql)
            : base(commandTreeFactory, dbHelper)
        {
            //非空检查
            Check.ArgumentNull(dbFactory, nameof(dbFactory));
            Check.ArgumentNull(innerQuerySql, nameof(innerQuerySql));
            // 初始化
            _modelAccessor = ObjectAccessor.GetAccessor(typeof(TModel));
            _tableAlias = typeof(TModel).Name.ToLower();
            //赋值
            _predicateParserFactory = predicateParserFactory;
            this.Factory = dbFactory;
            this.InnerQuerySql = innerQuerySql;
        }
        /// <summary>
        /// 设置数据源数据检索条件
        /// </summary>
        /// <param name="predicate">检索条件表达式</param>
        /// <returns>视图查询数据源（还是原来的数据源并未复制）</returns>
        public IDbView<TModel> SetWhere(Expression<Func<TModel, bool>> predicate)
        {
            // 解析Lambda表达式，获取并添加Sql表达式节点，并添加附带的sql参数
            foreach (INodeBuilder sqlBuilder in _predicateParserFactory.GetWhereChildBuilders(this, predicate.Parameters[0], predicate.Body))
            {
                // （解析时，已自动为当前数据源添加解析得到的sql参数，只需要）添加sql表达式节点
                base.AddNodeBuilder(sqlBuilder);
            }
            // 获取当前视图查询数据源
            return this;
        }
        /// <summary>
        /// 设置数据源数据检索条件
        /// </summary>
        /// <param name="selector">指定对象成员表达式</param>
        /// <param name="sqlPredicate">sql条件</param>
        /// <typeparam name="TProperty">模型属性类型</typeparam>
        /// <returns>视图查询数据源（还是原来的数据源并未复制）</returns>
        public IDbView<TModel> SetWhere<TProperty>(Expression<Func<TModel, TProperty>> selector, string sqlPredicate)
        {
            // 获取视图查询临时表名
            string tableAlias = typeof(TModel).Name.ToLower();
            // 获取指定的对象成员名称为视图查询映射列名
            string columnName = selector.Body.GetMemberExpression().Member.Name;
            // 获取sql查询条件表达式节点
            INodeBuilder nodeBuilder = base.CommandTreeFactory.GetWhereChildBuilder(tableAlias, columnName, sqlPredicate);
            // 添加sql查询条件表达式节点
            base.AddNodeBuilder(nodeBuilder);
            // 获取当前视图查询数据源
            return this;
        }
        /// <summary>
        /// 设置数据源数据检索条件
        /// </summary>
        /// <param name="selector">指定对象成员表达式</param>
        /// <param name="sqlPredicate">sql条件</param>
        /// <param name="sqlParameters">sql参数数组</param>
        /// <typeparam name="TProperty">模型属性类型</typeparam>
        /// <returns>视图查询数据源（还是原来的数据源并未复制）</returns>
        public IDbView<TModel> SetWhere<TProperty>(Expression<Func<TModel, TProperty>> selector, string sqlPredicate, params IDbDataParameter[] sqlParameters)
        {
            // 获取视图查询临时表名
            string tableAlias = typeof(TModel).Name.ToLower();
            // 获取指定的对象成员名称为视图查询映射列名
            string columnName = selector.Body.GetMemberExpression().Member.Name;
            // 获取sql查询条件表达式节点
            INodeBuilder nodeBuilder = base.CommandTreeFactory.GetWhereChildBuilder(tableAlias, columnName, sqlPredicate);
            // 添加sql查询条件表达式节点
            base.AddNodeBuilder(nodeBuilder);
            // 添加sql参数数组
            base.AddSqlParameters(sqlParameters);
            // 获取当前视图查询数据源
            return this;
        }
        /// <summary>
        /// 获取枚举器
        /// </summary>
        /// <returns>枚举器</returns>
        public IEnumerator<TModel> GetEnumerator()
        {
            // 创建CommandTree
            ICommandTree queryTree = base.CommandTreeFactory.GetWithAsQueryTree(this.InnerQuerySql, _tableAlias, this.NodeBuilders);
            // 执行查询
            foreach (TModel model in base.DbHelper.GetResults(this.CreateModel, queryTree.Compile(), parameters: this.Parameters.ToArray()))
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
