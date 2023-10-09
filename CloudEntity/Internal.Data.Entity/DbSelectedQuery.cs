using CloudEntity.CommandTrees;
using CloudEntity.CommandTrees.Commom;
using CloudEntity.Data;
using CloudEntity.Data.Entity;
using CloudEntity.Mapping;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CloudEntity.Internal.Data.Entity
{
    /// <summary>
    /// 选定项查询数据源类
    /// [作者：Apple_Li 李凯 15150598493]
    /// </summary>
    /// <typeparam name="TElement">元素类型</typeparam>
    /// <typeparam name="TEntity">实体类型</typeparam>
    internal class DbSelectedQuery<TElement, TEntity> : DbEntityBase<TEntity>, IDbSelectedQuery<TElement>
        where TEntity : class
    {
        /// <summary>
        /// 转换实体对象为TElement类型的委托
        /// </summary>
        internal Func<TEntity, TElement> Convert { private get; set; }

        /// <summary>
        /// 创建查询命令生成树
        /// </summary>
        /// <returns>查询命令生成树</returns>
        protected virtual ICommandTree CreateQueryTree()
        {
            return base.CommandFactory.GetQueryTree(base.NodeBuilders);
        }

        /// <summary>
        /// 创建选定项查询数据源对象
        /// </summary>
        /// <param name="mapperContainer">Mapper容器</param>
        /// <param name="commandFactory">SQL命令工厂</param>
        /// <param name="dbHelper">操作数据库的DbHelper</param>
        public DbSelectedQuery(IMapperContainer mapperContainer, ICommandFactory commandFactory, IDbHelper dbHelper)
            : base(mapperContainer, commandFactory, dbHelper) { }
        /// <summary>
        /// 获取查询Sql字符串
        /// </summary>
        /// <returns>查询Sql字符串</returns>
        public string ToSqlString()
        {
            // 获取查询命令生成树
            ICommandTree commandTree = this.CreateQueryTree();
            // 获取生成的sql
            return commandTree.Compile();
        }
        /// <summary>
        /// 获取的查询列名列表
        /// </summary>
        /// <returns>查询列名列表</returns>
        public IEnumerable<string> GetSelectNames()
        {
            // 遍历所有的sql表达式节点
            foreach (INodeBuilder nodeBuilder in base.NodeBuilders)
            {
                // 若不是Column节点则跳过
                if (nodeBuilder.BuilderType != BuilderType.Column)
                    continue;
                // 获取Column节点
                ColumnBuilder columnBuilder = nodeBuilder as ColumnBuilder;
                // 获取查询列名
                yield return columnBuilder.ColumnName;
            }
        }
        /// <summary>
        /// 获取枚举器
        /// </summary>
        /// <returns>枚举器</returns>
        public IEnumerator<TElement> GetEnumerator()
        {
            // 获取sql命令
            string commandText = this.CreateQueryTree().Compile();
            // 构建读取DataReader，创建填充获取实体对象的匿名函数
            Func<IDataReader, TEntity> getEntity = base.BuildGetEntityFunc(this.GetSelectNames().ToArray());
            //执行查询获取TElement类型的枚举器
            foreach (TEntity entity in base.DbHelper.GetResults(getEntity, commandText, parameters: base.Parameters.ToArray()))
                yield return this.Convert(entity);
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
