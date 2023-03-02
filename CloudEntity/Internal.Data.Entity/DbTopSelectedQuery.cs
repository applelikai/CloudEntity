using CloudEntity.CommandTrees;
using CloudEntity.Data;
using CloudEntity.Mapping;

namespace CloudEntity.Internal.Data.Entity
{
    /// <summary>
    /// 去除TOP选定项查询数据源类
    /// [作者：Apple_Li 李凯 15150598493]
    /// </summary>
    /// <typeparam name="TElement">元素类型</typeparam>
    /// <typeparam name="TEntity">实体类型</typeparam>
    internal class DbTopSelectedQuery<TElement, TEntity> : DbSelectedQuery<TElement, TEntity>
        where TEntity : class
    {
        /// <summary>
        /// 查询的前几条的元素数量
        /// </summary>
        private int _topCount;

        /// <summary>
        /// 创建查询命令生成树
        /// </summary>
        /// <returns>查询命令生成树</returns>
        protected override ICommandTree CreateQueryTree()
        {
            return base.CommandFactory.GetTopQueryTree(base.NodeBuilders, _topCount);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="mapperContainer">Mapper容器</param>
        /// <param name="commandFactory">SQL命令工厂</param>
        /// <param name="dbHelper">操作数据库的DbHelper</param>
        /// <param name="topCount">查询的前几条的元素数量</param>
        public DbTopSelectedQuery(IMapperContainer mapperContainer, ICommandFactory commandFactory, IDbHelper dbHelper, int topCount)
            : base(mapperContainer, commandFactory, dbHelper)
        {
            _topCount = topCount;
        }
    }
}