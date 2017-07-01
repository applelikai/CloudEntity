using System.Collections.Generic;
using CloudEntity.CommandTrees;

namespace CloudEntity.Internal.CommandTreeGetters
{
    /// <summary>
    /// 查询命令生成树获取器
    /// </summary>
    internal class QueryTreeGetter : CommandTreeGetter
    {
        /// <summary>
        /// 创建查询命令生成树获取器
        /// </summary>
        /// <param name="commandTreeFactory">创建CommandTree的工厂</param>
        public QueryTreeGetter(ICommandTreeFactory commandTreeFactory)
            : base(commandTreeFactory)
        {
        }
        /// <summary>
        /// 获取查询命令生成树
        /// </summary>
        /// <param name="nodeBuilders">查询命令生成树的子节点集合</param>
        /// <returns>查询命令生成树</returns>
        public override ICommandTree Get(IEnumerable<INodeBuilder> nodeBuilders)
        {
            return base.CommandTreeFactory.CreateQueryTree(nodeBuilders);
        }
    }
}