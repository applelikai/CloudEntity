using CloudEntity.CommandTrees;
using CloudEntity.Data;
using CloudEntity.Mapping;

namespace CloudEntity.Internal.Data.Entity
{
    /// <summary>
    /// 去除重复项查询数据源类
    /// [作者：Apple_Li 李凯 15150598493]
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
            return base.CommandFactory.GetDistinctQueryTree(base.NodeBuilders);
        }

        /// <summary>
        /// 创建去除重复项查询数据源
        /// </summary>
        /// <param name="mapperContainer">Mapper容器</param>
        /// <param name="commandFactory">创建CommandTree的工厂</param>
        /// <param name="dbHelper">操作数据库的DbHelper</param>
        public DbDistinctQuery(IMapperContainer mapperContainer, ICommandFactory commandFactory, IDbHelper dbHelper)
            : base(mapperContainer, commandFactory, dbHelper) { }
    }
}
