using System.Collections.Generic;

namespace CloudEntity.CommandTrees
{
    /// <summary>
    /// SELECT SQL命令生成树
    /// [作者：Apple_Li 李凯 15150598493]
    /// </summary>
    public interface ISelectCommandTree : ICommandTree
    {
        /// <summary>
        /// 查询列名列表
        /// </summary>
        IEnumerable<string> SelectNames { get; }
    }
}