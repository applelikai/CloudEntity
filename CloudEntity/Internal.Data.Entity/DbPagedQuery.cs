using CloudEntity.CommandTrees;
using CloudEntity.CommandTrees.Commom;
using CloudEntity.Data;
using CloudEntity.Data.Entity;
using CloudEntity.Internal.CommandTreeGetters;
using CloudEntity.Mapping;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace CloudEntity.Internal.Data.Entity
{
    /// <summary>
    /// 分页查询数据源类
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    internal class DbPagedQuery<TEntity> : DbBase, IDbPagedQuery<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// 对象访问器
        /// </summary>
        private ObjectAccessor entityAccessor;

        /// <summary>
        /// 创建CommandTree的工厂
        /// </summary>
        internal ICommandTreeFactory CommandTreeFactory { private get; set; }
        /// <summary>
        /// 当前对象的关联对象属性链接数组
        /// </summary>
        internal PropertyLinker[] PropertyLinkers { private get; set; }

        /// <summary>
        /// 元素总数量
        /// </summary>
        public int Count
        {
            get
            {
                //获取Count查询命令生成树
                ICommandTree queryTree = this.CommandTreeFactory.CreateQueryTree(this.GetCountNodeBuilders());
                //执行Count查询获取元素总数量
                return TypeHelper.ConvertTo<int>(base.DbHelper.GetScalar(queryTree.Compile(), parameters: base.Parameters.ToArray()));
            }
        }
        /// <summary>
        /// 页元素数量
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 当前是第几页
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount
        {
            get
            {
                int totalCount = this.Count;
                return (totalCount / this.PageSize) + (totalCount % this.PageSize > 0 ? 1 : 0);
            }
        }

        /// <summary>
        /// 获取Count统计查询命令生成树的子节点集合
        /// </summary>
        /// <returns>Count统计查询命令生成树的子节点集合</returns>
        private IEnumerable<INodeBuilder> GetCountNodeBuilders()
        {
            //获取Count节点
            yield return new NodeBuilder(SqlType.Select, "COUNT(*)");
            //返回From节点和Where节点下的所有子表达式节点
            foreach (INodeBuilder nodeBuilder in base.NodeBuilders)
            {
                switch (nodeBuilder.ParentNodeType)
                {
                    case SqlType.From:
                    case SqlType.Where:
                        yield return nodeBuilder;
                        break;
                }
            }
        }
        /// <summary>
        /// 创建实体对象
        /// </summary>
        /// <param name="reader">数据流</param>
        /// <param name="columnNames">查询的列</param>
        /// <returns>实体对象</returns>
        private object CreateEntity(IDataReader reader, string[] columnNames)
        {
            ITableMapper tableMapper = base.MapperContainer.GetTableMapper(typeof(TEntity));
            AccessorLinker[] accessorLinkers = this.PropertyLinkers.Select(l => l.ToAccessorLinker(base.MapperContainer)).ToArray();
            return this.entityAccessor.CreateEntity(tableMapper, reader, columnNames, accessorLinkers);
        }

        /// <summary>
        /// 创建分页查询数据源
        /// </summary>
        /// <param name="mapperContainer">Mapper容器</param>
        /// <param name="queryTreeGetter">查询命令生成树获取器</param>
        /// <param name="dbHelper">操作数据库的DbHelper</param>
        public DbPagedQuery(IMapperContainer mapperContainer, CommandTreeGetter queryTreeGetter, DbHelper dbHelper)
            : base(mapperContainer, queryTreeGetter, dbHelper)
        {
            this.entityAccessor = ObjectAccessor.GetAccessor(typeof(TEntity));
        }
        /// <summary>
        /// 获取枚举器
        /// </summary>
        /// <returns>枚举器</returns>
        public IEnumerator<TEntity> GetEnumerator()
        {
            //获取查询命令生成树(当前可默认获取到分页查询命令生成树)
            ICommandTree queryTree = base.QueryTreeGetter.Get(base.NodeBuilders);
            //获取sql参数集合
            IList<IDbDataParameter> parameters = base.Parameters.ToList();
            parameters.Add(base.DbHelper.Parameter("SkipCount", this.PageSize * (this.PageIndex - 1)));
            parameters.Add(base.DbHelper.Parameter("NextCount", this.PageSize));
            //执行查询
            foreach (TEntity entity in base.DbHelper.GetResults(this.CreateEntity, queryTree.Compile(), parameters: parameters.ToArray()))
                yield return entity;
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
