using CloudEntity.CommandTrees;
using CloudEntity.Data;
using CloudEntity.Data.Entity;
using CloudEntity.Mapping;
using System.Collections.Generic;
using System.Linq;

namespace CloudEntity.Internal.Data.Entity
{
    /// <summary>
    /// 排序数据源
    /// Apple_Li 李凯
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    internal class DbSortedQuery<TEntity> : DbQuery<TEntity>, IDbSortedQuery<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// 排序sql表达式节点集合
        /// </summary>
        public IEnumerable<INodeBuilder> SortBuilders { get; set; }

        /// <summary>
        /// 获取所有的sql表达式节点
        /// </summary>
        /// <returns>所有的sql表达式节点</returns>
        protected override IEnumerable<INodeBuilder> GetNodeBuilders()
        {
            return base.NodeBuilders.Concat(this.SortBuilders);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="mapperContainer">Mapper容器</param>
        /// <param name="commandTreeFactory">创建CommandTree的工厂</param>
        /// <param name="dbHelper">操作数据库的DbHelper</param>
        public DbSortedQuery(IMapperContainer mapperContainer, ICommandTreeFactory commandTreeFactory, DbHelper dbHelper)
            : base(mapperContainer, commandTreeFactory, dbHelper) { }
    }
}
