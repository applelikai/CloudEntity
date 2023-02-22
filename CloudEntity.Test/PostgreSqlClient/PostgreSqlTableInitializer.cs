using CloudEntity.Core.Data.Entity;
using CloudEntity.Data;
using CloudEntity.Mapping;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace CloudEntity.Test.PostgreSqlClient;

/// <summary>
/// PostgreSql Table初始化器
/// </summary>
public class PostgreSqlTableInitializer : TableInitializer
{
    /// <summary>
    /// 获取删除Table的命令
    /// </summary>
    /// <param name="schemaName">数据库模式(默认为 public)</param>
    /// <param name="tableName">表名</param>
    /// <returns>删除Table的命令</returns>
    protected override string GetDropTableCommand(string schemaName, string tableName)
    {
        //若架构名为空则直接(DROP TABLE 表名)
        if (string.IsNullOrEmpty(schemaName))
            return $"DROP TABLE \"{tableName}\"";
        //若不为空则(DROP TABLE 架构名.表名)
        return $"DROP TABLE {schemaName}.\"{tableName}\"";
    }
    /// <summary>
    /// 获取重命名Table的命令
    /// </summary>
    /// <param name="schemaName">数据库模式(默认为 public)</param>
    /// <param name="tableName">表名</param>
    /// <param name="oldTableName">原来的Table名</param>
    /// <returns>重命名Table的命令</returns>
    protected override string GetRenameTableCommand(string schemaName, string tableName, string oldTableName)
    {
        //若架构名为空则直接(ALTER TABLE 旧表名 RENAME TO 新表名)
        if (string.IsNullOrEmpty(schemaName))
            return $"ALTER TABLE \"{oldTableName}\" RENAME TO \"{tableName}\"";
        //若不为空则(ALTER TABLE 架构名.旧表名 RENAME TO 架构名.新表名)
        return string.Format("ALTER TABLE {0}.\"{1}\" RENAME TO {0}.\"{2}\"", schemaName, oldTableName, tableName);
    }

    /// <summary>
    /// 判断当前table是否存在
    /// </summary>
    /// <param name="dbHelper">操作数据库的Helper</param>
    /// <param name="tableHeader">Table头</param>
    /// <returns>当前table是否存在</returns>
    public override bool IsExist(IDbHelper dbHelper, ITableHeader tableHeader)
    {
        //获取sql参数数组
        IList<IDbDataParameter> parameters = new List<IDbDataParameter>
        {
            dbHelper.CreateParameter("TableName", tableHeader.TableName)
        };
        //初始化sql命令
        StringBuilder commandText = new StringBuilder();
        commandText.AppendLine("SELECT COUNT(*)");
        commandText.AppendLine("  FROM information_schema.tables t");
        commandText.AppendLine(" WHERE t.table_name = @TableName");
        //若有架构名则带上架构名
        string schemaName = tableHeader.SchemaName ?? dbHelper.DefaultSchemaName;
        if (!string.IsNullOrEmpty(schemaName))
        {
            commandText.AppendLine("   AND t.table_schema = @SchemaName");
            parameters.Add(dbHelper.CreateParameter("SchemaName", schemaName));
        }
        //执行获取结果
        int result = TypeHelper.ConvertTo<int>(dbHelper.GetScalar(commandText.ToString(), parameters: parameters.ToArray()));
        return result > 0;
    }
}