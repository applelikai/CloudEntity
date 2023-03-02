using CloudEntity.CommandTrees;
using CloudEntity.Core.Data.Entity;
using CloudEntity.Data;
using CloudEntity.Mapping;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace CloudEntity.Test.PostgreSqlClient;

/// <summary>
/// PostgreSql列初始化器
/// </summary>
public class PostgreSqlColumnInitializer : ColumnInitializer
{
    /// <summary>
    /// 查询获取当前表中所有的列
    /// </summary>
    /// <param name="tableHeader">table头</param>
    /// <returns>当前表中所有的列</returns>
    protected override IEnumerable<string> GetColumns(ITableHeader tableHeader)
    {
        //初始化sql参数数组
        IList<IDbDataParameter> parameters = new List<IDbDataParameter>
        {
            base.DbHelper.CreateParameter("TableName", tableHeader.TableName)
        };
        //初始化sql
        StringBuilder sqlBuilder = new StringBuilder();
        sqlBuilder.AppendLine("SELECT c.column_name");
        sqlBuilder.AppendLine("  FROM information_schema.columns c");
        sqlBuilder.AppendLine(" WHERE c.table_name = @TableName");
        //若有TABLE_SCHEMA查询条件则带上
        string schemaName = tableHeader.SchemaName ?? base.DbHelper.DefaultSchemaName;
        if (!string.IsNullOrEmpty(schemaName))
        {
            sqlBuilder.AppendLine("   AND c.table_schema = @SchemaName");
            parameters.Add(base.DbHelper.CreateParameter("SchemaName", schemaName));
        }
        //执行查询获取所有列
        return base.DbHelper.GetResults(reader => reader.GetString(0), sqlBuilder.ToString(), parameters: parameters.ToArray());
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="dbHelper">数据库操作对象</param>
    /// <param name="commandFactory">SQL命令工厂</param>
    public PostgreSqlColumnInitializer(IDbHelper dbHelper, ICommandFactory commandFactory)
     : base(dbHelper, commandFactory) { }
}
