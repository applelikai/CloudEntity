using System.Collections.Generic;

namespace CloudEntity.CommandTrees.Commom.OracleClient
{
    /// <summary>
    /// 创建CommandTree用于生成查询Oracle的sql
    /// 李凯 Apple_Li 15150598493
    /// 最后修改日期：2023/02/05
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
        /// 创建Delete命令生成树
        /// </summary>
        /// <param name="schemaName">数据库架构名</param>
        /// <param name="tableName">表名</param>
        /// <param name="tableAlias">临时表名</param>
        /// <returns>Delete命令生成树</returns>
        protected override DeleteTree CreateDeleteTree(string schemaName, string tableName, string tableAlias)
        {
            return new OracleDeleteTree(schemaName, tableName, tableAlias, base.ParameterMarker);
        }

        /// <summary>
        /// 创建针对Oracle的sql驱动
        /// </summary>
        public OracleCommandTreeFactory()
            : base(':')
        { }
        /// <summary>
        /// 获取分页查询命令生成树
        /// </summary>
        /// <param name="queryChildBuilders">分页查询命令生成树的子节点集</param>
        /// <returns>分页查询命令生成树</returns>
        public override ICommandTree GetPagingQueryTree(IEnumerable<INodeBuilder> queryChildBuilders)
        {
            //创建Oracle分页sql生成树
            OraclePagingQueryTree queryTree = new OraclePagingQueryTree(base.ParameterMarker);
            //加载Oracle分页sql生成树
            base.LoadQueryTree(queryTree, queryChildBuilders);
            //返回Oracle分页sql生成树
            return queryTree;
        }
    }
}