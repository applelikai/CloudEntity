﻿using CloudEntity.CommandTrees;
using System.Collections.Generic;
using System.Data;

namespace CloudEntity.Data.Entity
{
    /// <summary>
    /// 数据操作基础接口
    /// [作者：Apple_Li 李凯 15150598493]
    /// </summary>
    public interface IDbBase
    {
        /// <summary>
        /// sql表达式节点集合
        /// </summary>
        IEnumerable<INodeBuilder> NodeBuilders { get; }
        /// <summary>
        /// sql参数集合
        /// </summary>
        IEnumerable<IDbDataParameter> Parameters { get; }
    }
}
