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
    internal class DbSelectedQuery<TElement, TEntity> : DbSortedQuery<TEntity>, IDbSelectedQuery<TElement>
        where TEntity : class
    {
        /// <summary>
        /// 实体访问器
        /// </summary>
        private ObjectAccessor _entityAccessor;
        /// <summary>
        /// 当前对象的关联对象属性链接列表
        /// </summary>
        private IList<PropertyLinker> _propertyLinkers;

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
        /// 创建TElement类型的对象
        /// </summary>
        /// <param name="reader">数据流</param>
        /// <param name="columnNames">查询的列</param>
        /// <returns>实体对象</returns>
        private TElement CreateElement(IDataReader reader, string[] columnNames)
        {
            ITableMapper tableMapper = base.MapperContainer.GetTableMapper(typeof(TEntity));
            AccessorLinker[] accessorLinkers = _propertyLinkers.Select(l => l.ToAccessorLinker(base.MapperContainer)).ToArray();
            TEntity entity = _entityAccessor.CreateEntity(tableMapper, reader, columnNames, accessorLinkers) as TEntity;
            return this.Convert(entity);
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
            : base(mapperContainer, commandTreeFactory, dbHelper)
        {
            _entityAccessor = ObjectAccessor.GetAccessor(typeof(TEntity));
            _propertyLinkers = new List<PropertyLinker>();
        }
        /// <summary>
        /// 添加关联的对象属性链接
        /// </summary>
        /// <param name="propertyLinker">关联的对象属性链接</param>
        public void AddPropertyLinker(PropertyLinker propertyLinker)
        {
            _propertyLinkers.Add(propertyLinker);
        }
        /// <summary>
        /// 添加关联的对象属性链接列表
        /// </summary>
        /// <param name="propertyLinkers">关联的对象属性链接列表</param>
        public void AddPropertyLinkers(IEnumerable<PropertyLinker> propertyLinkers)
        {
            foreach (PropertyLinker propertyLinker in propertyLinkers)
                _propertyLinkers.Add(propertyLinker);
        }
        /// <summary>
        /// 获取枚举器
        /// </summary>
        /// <returns>枚举器</returns>
        public IEnumerator<TElement> GetEnumerator()
        {
            //获取查询命令生成树
            ICommandTree queryTree = this.CreateQueryTree();
            //执行查询获取TElement类型的枚举器
            foreach (TElement element in base.DbHelper.GetResults(this.CreateElement, queryTree.Compile(), parameters: base.Parameters.ToArray()))
                yield return element;
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
