using CloudEntity.Core.Data.Entity;
using CloudEntity.Data;
using CloudEntity.Mapping;
using System.Data;
using System.Text;

namespace CloudEntity.Test.MySqlClient
{
    /// <summary>
    /// MySql Table创建器
    /// </summary>
    internal class MySqlTableInitializer : TableInitializer
    {
        /// <summary>
        /// 判断当前table是否存在
        /// </summary>
        /// <param name="dbHelper">操作数据库的Helper</param>
        /// <param name="tableHeader">Table基础数据</param>
        /// <returns>当前table是否存在</returns>
        public override bool IsExist(DbHelper dbHelper, ITableHeader tableHeader)
        {
            //获取sql命令
            StringBuilder commandText = new StringBuilder();
            commandText.AppendLine("SELECT COUNT(*)");
            commandText.AppendLine("  FROM information_schema.Tables t");
            commandText.AppendLine(" WHERE t.Table_Name = @TableName");
            commandText.AppendLine("   AND t.Table_Schema = @SchemaName");
            //获取sql参数数组
            IDbDataParameter[] parameters = new IDbDataParameter[]
            {
                dbHelper.Parameter("TableName", tableHeader.TableName),
                dbHelper.Parameter("SchemaName", tableHeader.SchemaName)
            };
            //执行获取结果
            int result = TypeHelper.ConvertTo<int>(dbHelper.GetScalar(commandText.ToString(), parameters: parameters));
            return result > 0;
        }
    }
}
