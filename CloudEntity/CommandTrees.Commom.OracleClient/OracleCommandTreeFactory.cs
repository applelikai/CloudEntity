using System.Collections.Generic;

namespace CloudEntity.CommandTrees.Commom.OracleClient
{
    /// <summary>
    /// 创建CommandTree用于生成查询Oracle的sql
    /// 李凯 Apple_Li
    /// </summary>
    public class OracleCommandTreeFactory : CommandTreeFactory
    {
        /// <summary>
        /// 创建读取节点信息的Helper
        /// </summary>
        /// <returns>读取节点信息的Helper</returns>
        protected override ColumnNodeHelper CreateColumnNodeHelper()
        {
            return new OracleColumnNodeHelper();
        }

        /// <summary>
        /// 创建针对Oracle的sql驱动
        /// </summary>
        public OracleCommandTreeFactory()
            : base(':')
        { }
        /// <summary>
        /// 创建分页查询命令生成树
        /// </summary>
        /// <param name="queryChildBuilders">分页查询命令生成树的子节点集</param>
        /// <param name="orderByColumn">排序的列</param>
        /// <param name="isAsc">True:升序(False为降序)</param>
        /// <returns>分页查询命令生成树</returns>
        public override ICommandTree CreatePagingQueryTree(IEnumerable<INodeBuilder> queryChildBuilders, string orderByColumn, bool isAsc = true)
        {
            //创建Oracle分页sql生成树
            OraclePagingQueryTree queryTree = new OraclePagingQueryTree(base.ParameterMarker);
            //加载Oracle分页sql生成树
            base.LoadQueryTree(queryTree, queryChildBuilders);
            queryTree.OrderBy.Append(new SqlBuilder("{0} {1}", orderByColumn, isAsc ? "ASC" : "DESC"));
            //返回Oracle分页sql生成树
            return queryTree;
        }
    }
}