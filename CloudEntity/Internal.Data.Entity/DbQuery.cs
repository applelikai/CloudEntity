using CloudEntity.CommandTrees;
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
    /// 查询数据源
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    internal class DbQuery<TEntity> : DbQueryBase<TEntity>, IDbQuery<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// 对象访问器
        /// </summary>
        private ObjectAccessor _objectAccessor;
        /// <summary>
        /// 当前对象的关联对象属性链接列表
        /// </summary>
        private IList<PropertyLinker> _propertyLinkers;

        /// <summary>
        /// 创建数据操作对象的工厂
        /// </summary>
        public IDbFactory Factory { get; private set; }
        /// <summary>
        /// 当前对象的关联对象属性链接列表
        /// </summary>
        public IEnumerable<PropertyLinker> PropertyLinkers 
        {
            get { return _propertyLinkers; }
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
            return _objectAccessor.CreateEntity(tableMapper, reader, columnNames, accessorLinkers);
        }

        /// <summary>
        /// 获取所有的sql表达式节点
        /// </summary>
        /// <returns>所有的sql表达式节点</returns>
        protected virtual IEnumerable<INodeBuilder> GetNodeBuilders()
        {
            return base.NodeBuilders;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="mapperContainer">Mapper容器</param>
        /// <param name="commandTreeFactory">创建CommandTree的工厂</param>
        /// <param name="dbHelper">操作数据库的DbHelper</param>
        /// <param name="dbFactory">创建数据操作对象的工厂</param>
        public DbQuery(IMapperContainer mapperContainer, ICommandTreeFactory commandTreeFactory, DbHelper dbHelper, IDbFactory dbFactory)
            : base(mapperContainer, commandTreeFactory, dbHelper)
        {
            _objectAccessor = ObjectAccessor.GetAccessor(typeof(TEntity));
            _propertyLinkers = new List<PropertyLinker>();
            this.Factory = dbFactory;
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
            //获取sql命令
            return base.CommandTreeFactory.GetQueryTree(this.GetNodeBuilders()).Compile();
        }
        /// <summary>
        /// 将此数据源的查询结果映射为TModel对象数据源
        /// </summary>
        /// <typeparam name="TModel">TModel对象（只要是有无参构造函数的类就可以）</typeparam>
        /// <returns>TModel对象数据源</returns>
        public IEnumerable<TModel> Cast<TModel>()
            where TModel : class, new()
        {
            // 获取sql命令
            string commandText = base.CommandTreeFactory.GetQueryTree(this.GetNodeBuilders()).Compile();
            // 执行查询
            return base.DbHelper.GetModels<TModel>(commandText, base.Parameters.ToArray());
        }
        /// <summary>
        /// 获取枚举器
        /// </summary>
        /// <returns>枚举器</returns>
        public IEnumerator<TEntity> GetEnumerator()
        {
            // 获取sql命令
            string commandText = base.CommandTreeFactory.GetQueryTree(this.GetNodeBuilders()).Compile();
            // 执行查询
            foreach (TEntity entity in base.DbHelper.GetResults(this.CreateEntity, commandText, parameters: base.Parameters.ToArray()))
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
