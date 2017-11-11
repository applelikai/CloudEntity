using CloudEntity.CommandTrees;
using CloudEntity.Data;
using CloudEntity.Data.Entity;
using CloudEntity.Mapping;
using System.Collections.Generic;
using System.Data;

namespace CloudEntity.Internal.Data.Entity
{
    /// <summary>
    /// 数据操作基础类
    /// </summary>
    internal class DbBase : IDbBase
    {
        /// <summary>
        /// Mapper容器
        /// </summary>
        protected IMapperContainer MapperContainer { get; private set; }
        /// <summary>
        /// 创建CommandTree的工厂
        /// </summary>
        protected ICommandTreeFactory CommandTreeFactory { get; private set; }
        /// <summary>
        /// 操作数据库的DbHelper
        /// </summary>
        protected DbHelper DbHelper { get; private set; }

        /// <summary>
        /// sql表达式节点集合
        /// </summary>
        public IEnumerable<INodeBuilder> NodeBuilders { get; internal set; }
        /// <summary>
        /// sql参数集合
        /// </summary>
        public IEnumerable<IDbDataParameter> Parameters { get; internal set; }
        
        /// <summary>
        /// 创建操作数据库的基础对象
        /// </summary>
        /// <param name="mapperContainer">Mapper容器</param>
        /// <param name="commandTreeFactory">创建CommandTree的工厂</param>
        /// <param name="dbHelper">操作数据库的DbHelper</param>
        public DbBase(IMapperContainer mapperContainer, ICommandTreeFactory commandTreeFactory, DbHelper dbHelper)
        {
            this.MapperContainer = mapperContainer;
            this.CommandTreeFactory = commandTreeFactory;
            this.DbHelper = dbHelper;
        }
    }
}
