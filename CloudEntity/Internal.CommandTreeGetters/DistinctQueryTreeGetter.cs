using System.Collections.Generic;
using CloudEntity.CommandTrees;

namespace CloudEntity.Internal.CommandTreeGetters
{
    /// <summary>
    /// 去除重复的查询命令生成树获取器
    /// </summary>
    internal class DistinctQueryTreeGetter : CommandTreeGetter
    {
        /// <summary>
        /// 创建去除重复的查询命令生成树获取器
        /// </summary>
        /// <param name="commandTreeFactory">创建CommandTree的工厂</param>
        public DistinctQueryTreeGetter(ICommandTreeFactory commandTreeFactory)
            : base(commandTreeFactory)
        {
        }
        /// <summary>
        /// 获取去除重复的查询命令生成树
        /// </summary>
        /// <param name="nodeBuilders">查询命令生成树的子节点集合</param>
        /// <returns>去除重复的查询命令生成树</returns>
        public override ICommandTree Get(IEnumerable<INodeBuilder> nodeBuilders)
        {
            return base.CommandTreeFactory.CreateDistinctQueryTree(nodeBuilders);
        }
    }
}