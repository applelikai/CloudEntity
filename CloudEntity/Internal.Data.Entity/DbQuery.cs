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
    internal class DbQuery<TEntity> : DbBase, IDbQuery<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// 对象访问器
        /// </summary>
        private ObjectAccessor objectAccessor;

        /// <summary>
        /// 数据操作类的工厂
        /// </summary>
        public IDbFactory Factory { get; internal set; }
        /// <summary>
        /// 当前对象的关联对象属性链接数组
        /// </summary>
        public PropertyLinker[] PropertyLinkers { get; internal set; }
        /// <summary>
        /// Sql参数创建对象
        /// </summary>
        public IParameterFactory ParameterFactory => base.DbHelper;
        
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
            return this.objectAccessor.CreateEntity(tableMapper, reader, columnNames, accessorLinkers);
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
        public DbQuery(IMapperContainer mapperContainer, ICommandTreeFactory commandTreeFactory, DbHelper dbHelper)
            : base(mapperContainer, commandTreeFactory, dbHelper)
        {
            this.objectAccessor = ObjectAccessor.GetAccessor(typeof(TEntity));
        }
        /// <summary>
        /// 获取枚举器
        /// </summary>
        /// <returns>枚举器</returns>
        public IEnumerator<TEntity> GetEnumerator()
        {
            //创建CommandTree
            ICommandTree queryTree = base.CommandTreeFactory.GetQueryTree(this.GetNodeBuilders());
            //执行查询
            foreach (TEntity entity in base.DbHelper.GetResults(this.CreateEntity, queryTree.Compile(), parameters: base.Parameters.ToArray()))
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
