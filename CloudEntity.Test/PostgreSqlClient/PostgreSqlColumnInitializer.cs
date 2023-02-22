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
    /// <param name="dbHelper">操作数据库的DbHelper</param>
    /// <param name="tableHeader">table头</param>
    /// <returns>当前表中所有的列</returns>
    protected override IEnumerable<string> GetColumns(IDbHelper dbHelper, ITableHeader tableHeader)
    {
        //初始化sql参数数组
        IList<IDbDataParameter> parameters = new List<IDbDataParameter>
        {
            dbHelper.CreateParameter("TableName", tableHeader.TableName)
        };
        //初始化sql
        StringBuilder sqlBuilder = new StringBuilder();
        sqlBuilder.AppendLine("SELECT c.column_name");
        sqlBuilder.AppendLine("  FROM information_schema.columns c");
        sqlBuilder.AppendLine(" WHERE c.table_name = @TableName");
        //若有TABLE_SCHEMA查询条件则带上
        string schemaName = tableHeader.SchemaName ?? dbHelper.DefaultSchemaName;
        if (!string.IsNullOrEmpty(schemaName))
        {
            sqlBuilder.AppendLine("   AND c.table_schema = @SchemaName");
            parameters.Add(dbHelper.CreateParameter("SchemaName", schemaName));
        }
        //执行查询获取所有列
        return dbHelper.GetResults(reader => reader.GetString(0), sqlBuilder.ToString(), parameters: parameters.ToArray());
    }
}
