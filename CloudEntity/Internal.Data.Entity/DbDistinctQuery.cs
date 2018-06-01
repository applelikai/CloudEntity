using CloudEntity.CommandTrees;
using CloudEntity.Data;
using CloudEntity.Mapping;

namespace CloudEntity.Internal.Data.Entity
{
    /// <summary>
    /// 去除重复项查询数据源类
    /// Apple_Li 李凯
    /// </summary>
    /// <typeparam name="TElement">元素类型</typeparam>
    /// <typeparam name="TEntity">实体类型</typeparam>
    internal class DbDistinctQuery<TElement, TEntity> : DbSelectedQuery<TElement, TEntity>
        where TEntity : class
    {
        /// <summary>
        /// 创建查询命令生成树
        /// </summary>
        /// <returns>查询命令生成树</returns>
        protected override ICommandTree CreateQueryTree()
        {
            return base.CommandTreeFactory.CreateDistinctQueryTree(base.NodeBuilders);
        }

        /// <summary>
        /// 创建去除重复项查询数据源
        /// </summary>
        /// <param name="mapperContainer">Mapper容器</param>
        /// <param name="commandTreeFactory">创建CommandTree的工厂</param>
        /// <param name="dbHelper">操作数据库的DbHelper</param>
        public DbDistinctQuery(IMapperContainer mapperContainer, ICommandTreeFactory commandTreeFactory, DbHelper dbHelper)
            : base(mapperContainer, commandTreeFactory, dbHelper) { }
    }
}
