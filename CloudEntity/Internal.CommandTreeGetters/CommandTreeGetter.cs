using CloudEntity.CommandTrees;
using System;
using System.Collections.Generic;

namespace CloudEntity.Internal.CommandTreeGetters
{
    /// <summary>
    /// sql命令生成树获取器
    /// 李凯 Apple_Li
    /// </summary>
    internal abstract class CommandTreeGetter
    {
        /// <summary>
        /// 创建CommandTree的工厂
        /// </summary>
        protected ICommandTreeFactory CommandTreeFactory { get; private set; }

        /// <summary>
        /// 创建sql命令生成树获取器
        /// </summary>
        /// <param name="commandTreeFactory">创建CommandTree的工厂</param>
        public CommandTreeGetter(ICommandTreeFactory commandTreeFactory)
        {
            //赋值
            this.CommandTreeFactory = commandTreeFactory;
        }
        /// <summary>
        /// 获取sql命令生成树
        /// </summary>
        /// <param name="nodeBuilders">查询命令生成树的子节点集合</param>
        /// <returns>查询命令生成树</returns>
        public abstract ICommandTree Get(IEnumerable<INodeBuilder> nodeBuilders);
    }
}