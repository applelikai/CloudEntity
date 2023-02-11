using CloudEntity.CommandTrees;
using CloudEntity.CommandTrees.Commom;
using CloudEntity.Data;
using CloudEntity.Data.Entity;
using CloudEntity.Mapping;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CloudEntity.Internal.Data.Entity
{
    /// <summary>
    /// 分页查询数据源类
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    internal class DbPagedQuery<TEntity> : DbQueryBase<TEntity>, IDbPagedQuery<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// 对象访问器
        /// </summary>
        private ObjectAccessor _entityAccessor;
        /// <summary>
        /// 当前对象的关联对象属性链接列表
        /// </summary>
        private IList<PropertyLinker> _propertyLinkers;

        /// <summary>
        /// 元素总数量
        /// </summary>
        public int Count
        {
            get
            {
                //获取Count查询命令生成树
                ICommandTree queryTree = this.CommandTreeFactory.GetQueryTree(this.GetCountNodeBuilders());
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
            // 获取Count节点
            yield return new NodeBuilder(SqlType.Select, "COUNT(*)");
            // 返回From节点和Where节点下的所有子表达式节点
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
            //获取实体类型的Mapper对象
            ITableMapper tableMapper = base.MapperContainer.GetTableMapper(typeof(TEntity));
            //获取对象访问关联对象数组
            AccessorLinker[] accessorLinkers = _propertyLinkers.Select(l => l.ToAccessorLinker(base.MapperContainer)).ToArray();
            //获取创建的实体对象
            return _entityAccessor.CreateEntity(tableMapper, reader, columnNames, accessorLinkers);
        }

        /// <summary>
        /// 创建分页查询数据源
        /// </summary>
        /// <param name="mapperContainer">Mapper容器</param>
        /// <param name="commandTreeFactory">创建CommandTree的工厂</param>
        /// <param name="dbHelper">操作数据库的DbHelper</param>
        public DbPagedQuery(IMapperContainer mapperContainer, ICommandTreeFactory commandTreeFactory, DbHelper dbHelper)
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
        /// 获取sql字符串
        /// </summary>
        /// <returns>sql字符串</returns>
        public string ToSqlString()
        {
            //获取查询命令
            ICommandTree queryTree = base.CommandTreeFactory.GetPagingQueryTree(base.NodeBuilders);
            return queryTree.Compile();
        }
        /// <summary>
        /// 将此数据源的查询结果映射为TModel对象数据源
        /// </summary>
        /// <typeparam name="TModel">TModel对象（只要是有无参构造函数的类就可以）</typeparam>
        /// <returns>TModel对象数据源</returns>
        public IEnumerable<TModel> Cast<TModel>()
            where TModel : class, new()
        {
            //获取查询命令
            ICommandTree queryTree = base.CommandTreeFactory.GetPagingQueryTree(base.NodeBuilders);
            string commandText = queryTree.Compile();
            //获取sql参数集合
            IList<IDbDataParameter> parameters = base.Parameters.ToList();
            parameters.Add(base.DbHelper.Parameter("SkipCount", this.PageSize * (this.PageIndex - 1)));
            parameters.Add(base.DbHelper.Parameter("NextCount", this.PageSize));
            //执行查询
            return base.DbHelper.GetModels<TModel>(commandText, parameters.ToArray());
        }
        /// <summary>
        /// 获取枚举器
        /// </summary>
        /// <returns>枚举器</returns>
        public IEnumerator<TEntity> GetEnumerator()
        {
            //获取查询命令
            ICommandTree queryTree = base.CommandTreeFactory.GetPagingQueryTree(base.NodeBuilders);
            string commandText = queryTree.Compile();
            //获取sql参数集合
            IList<IDbDataParameter> parameters = base.Parameters.ToList();
            parameters.Add(base.DbHelper.Parameter("SkipCount", this.PageSize * (this.PageIndex - 1)));
            parameters.Add(base.DbHelper.Parameter("NextCount", this.PageSize));
            //执行查询
            foreach (TEntity entity in base.DbHelper.GetResults(this.CreateEntity, commandText, parameters: parameters.ToArray()))
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
