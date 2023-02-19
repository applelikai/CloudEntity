using CloudEntity.CommandTrees;
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
    /// Apple_Li 李凯 15150598493
    /// 2017/06/19 最后修改：2023/02/09 21:07
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
        /// 获取条件查询命令生成树的子节点集合
        /// </summary>
        /// <returns>条件查询命令生成树的子节点集合</returns>
        private IEnumerable<INodeBuilder> GetWhereNodeBuilders()
        {
            foreach (INodeBuilder nodeBuilder in base.NodeBuilders)
            {
                switch (nodeBuilder.ParentNodeType)
                {
                    case SqlType.Select:
                    case SqlType.From:
                    case SqlType.Where:
                        yield return nodeBuilder;
                        break;
                }
            }
        }

        /// <summary>
        /// 创建查询命令生成树
        /// </summary>
        /// <returns>查询命令生成树</returns>
        protected virtual ICommandTree CreateQueryTree()
        {
            return base.CommandTreeFactory.GetQueryTree(base.NodeBuilders);
        }

        /// <summary>
        /// 创建选定项查询数据源对象
        /// </summary>
        /// <param name="mapperContainer">Mapper容器</param>
        /// <param name="commandTreeFactory">创建CommandTree的工厂</param>
        /// <param name="dbHelper">操作数据库的DbHelper</param>
        public DbSelectedQuery(IMapperContainer mapperContainer, ICommandTreeFactory commandTreeFactory, DbHelper dbHelper)
            : base(mapperContainer, commandTreeFactory, dbHelper) { }
        /// <summary>
        /// 获取枚举器
        /// </summary>
        /// <returns>枚举器</returns>
        public IEnumerator<TElement> GetEnumerator()
        {
            //获取查询命令生成树
            ICommandTree queryTree = this.CreateQueryTree();
            // 获取创建实体对象的匿名函数
            Func<IDataReader, string[], TEntity> getEntity = base.GetCreateEntityFunc();
            //执行查询获取TElement类型的枚举器
            foreach (TEntity entity in base.DbHelper.GetResults(getEntity, queryTree.Compile(), parameters: base.Parameters.ToArray()))
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
        /// <summary>
        /// 获取查询Sql字符串
        /// </summary>
        /// <returns>查询Sql字符串</returns>
        public string ToSqlString()
        {
            // 获取查询命令生成树
            ICommandTree queryTree = this.CreateQueryTree();
            // 获取生成的sql
            return queryTree.Compile();
        }
    }
}
