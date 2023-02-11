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
        /// <param name="innerQuerySql">查询sql</param>
        public DbView(IDbFactory dbFactory, ICommandTreeFactory commandTreeFactory, DbHelper dbHelper, string innerQuerySql)
            : base(commandTreeFactory, dbHelper)
        {
            //非空检查
            Check.ArgumentNull(dbFactory, nameof(dbFactory));
            Check.ArgumentNull(innerQuerySql, nameof(innerQuerySql));
            // 初始化
            _modelAccessor = ObjectAccessor.GetAccessor(typeof(TModel));
            _tableAlias = typeof(TModel).Name.ToLower();
            //赋值
            this.Factory = dbFactory;
            this.InnerQuerySql = innerQuerySql;
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
