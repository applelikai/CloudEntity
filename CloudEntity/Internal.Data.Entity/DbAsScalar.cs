using CloudEntity.CommandTrees;
using CloudEntity.Data;
using CloudEntity.Data.Entity;
using System.Linq;

namespace CloudEntity.Internal.Data.Entity
{
    /// <summary>
    /// 视图查询第一行第一列的值的类
    /// </summary>
    internal class DbAsScalar : DbQueryBase, IDbScalar
    {
        /// <summary>
        /// 查询sql
        /// </summary>
        private readonly string _innerQuerySql;
        /// <summary>
        /// 视图查询时使用的临时表名
        /// </summary>
        private readonly string _tableAlias;

        /// <summary>
        /// 创建统计查询类
        /// </summary>
        /// <param name="commandFactory">SQL命令工厂</param>
        /// <param name="dbHelper">操作数据库的DbHelper</param>
        /// <param name="innerQuerySql">查询sql</param>
        /// <param name="tableAlias">临时表名</param>
        public DbAsScalar(ICommandFactory commandFactory, IDbHelper dbHelper, string innerQuerySql, string tableAlias)
            : base(commandFactory, dbHelper)
        {
            _innerQuerySql = innerQuerySql;
            _tableAlias = tableAlias;
        }
        /// <summary>
        /// 执行查询获取第一行第一列的值
        /// </summary>
        /// <returns>第一行第一列的值</returns>
        public object Execute()
        {
            // 创建CommandTree
            ICommandTree queryTree = base.CommandFactory.GetWithAsQueryTree(_innerQuerySql, _tableAlias, base.NodeBuilders);
            // 执行查询获取第一行，第一列的值
            return base.DbHelper.GetScalar(queryTree.Compile(), parameters: base.Parameters.ToArray());
        }
    }
}
