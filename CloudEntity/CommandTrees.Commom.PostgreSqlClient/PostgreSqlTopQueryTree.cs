﻿using System.Text;

namespace CloudEntity.CommandTrees.Commom.PostgreSqlClient
{
    /// <summary>
    /// PostgreSql的top查询命令生成树
    /// 李凯 Apple_Li 2021/09/19
    /// </summary>
    internal class PostgreSqlTopQueryTree : QueryTree
    {
        /// <summary>
        /// 查询的前几条的元素数量
        /// </summary>
        private int _topCount;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="parameterMarker">sql参数标识符号</param>
        /// <param name="topCount">查询的前几条的元素数量</param>
        public PostgreSqlTopQueryTree(char parameterMarker, int topCount)
            : base(parameterMarker)
        {
            _topCount = topCount;
        }
        /// <summary>
        /// 构建sql命令
        /// </summary>
        /// <param name="commandText">待构建的sql</param>
        public override void Build(StringBuilder commandText)
        {
            base.Build(commandText);
            //拼接PostgreSql的top查询条件
            commandText.AppendLine();
            commandText.AppendFormat("     LIMIT {0}", _topCount);
        }
    }
}
