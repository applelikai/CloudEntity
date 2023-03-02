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
/// PostgreSql Table初始化器
/// </summary>
public class PostgreSqlTableInitializer : TableInitializer
{
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="dbHelper">数据库操作对象</param>
    /// <param name="commandFactory">SQL命令工厂</param>
    public PostgreSqlTableInitializer(IDbHelper dbHelper, ICommandFactory commandFactory)
     : base(dbHelper, commandFactory) { }
    /// <summary>
    /// 判断当前table是否存在
    /// </summary>
    /// <param name="tableHeader">Table头</param>
    /// <returns>当前table是否存在</returns>
    public override bool IsExist(ITableHeader tableHeader)
    {
        //获取sql参数数组
        IList<IDbDataParameter> parameters = new List<IDbDataParameter>
        {
            base.DbHelper.CreateParameter("TableName", tableHeader.TableName)
        };
        //初始化sql命令
        StringBuilder commandText = new StringBuilder();
        commandText.AppendLine("SELECT COUNT(*)");
        commandText.AppendLine("  FROM information_schema.tables t");
        commandText.AppendLine(" WHERE t.table_name = @TableName");
        //若有架构名则带上架构名
        string schemaName = tableHeader.SchemaName ?? base.DbHelper.DefaultSchemaName;
        if (!string.IsNullOrEmpty(schemaName))
        {
            commandText.AppendLine("   AND t.table_schema = @SchemaName");
            parameters.Add(base.DbHelper.CreateParameter("SchemaName", schemaName));
        }
        //执行获取结果
        int result = TypeHelper.ConvertTo<int>(base.DbHelper.GetScalar(commandText.ToString(), parameters: parameters.ToArray()));
        return result > 0;
    }
}