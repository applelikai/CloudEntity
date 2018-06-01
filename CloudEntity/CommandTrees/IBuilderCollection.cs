using System;
using System.Collections.Generic;
using System.Text;

namespace CloudEntity.CommandTrees
{
    /// <summary>
    /// sql节点集
    /// 李凯 Apple_Li
    /// </summary>
    public interface IBuilderCollection : ISqlBuilder, IEnumerable<ISqlBuilder>
    {
        /// <summary>
        /// 节点数量
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 添加sql子节点
        /// </summary>
        /// <param name="sqlBuilder">sql子节点</param>
        void Append(ISqlBuilder sqlBuilder);
        /// <summary>
        /// 按索引插入新的Sql节点
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="sqlBuilder">Sql节点</param>
        void Insert(int index, ISqlBuilder sqlBuilder);
    }
}
