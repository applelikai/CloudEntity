using CloudEntity.CommandTrees;
using CloudEntity.Data;
using CloudEntity.Mapping;
using System.Collections.Generic;
using System.Linq;

namespace CloudEntity.Core.Data.Entity
{
    /// <summary>
    /// Table初始化器
    /// </summary>
    public abstract class TableInitializer
    {
        /// <summary>
        /// 获取删除Table的命令
        /// </summary>
        /// <param name="schemaName">数据库架构名（或用户名 或模式）</param>
        /// <param name="tableName">表名</param>
        /// <returns>删除Table的命令</returns>
        protected virtual string GetDropTableCommand(string schemaName, string tableName)
        {
            //若架构名为空则直接(DROP TABLE 表名)
            if (string.IsNullOrEmpty(schemaName))
                return $"DROP TABLE {tableName}";
            //若不为空则(DROP TABLE 架构名.表名)
            return $"DROP TABLE {schemaName}.{tableName}";
        }
        /// <summary>
        /// 获取重命名Table的命令
        /// </summary>
        /// <param name="schemaName">数据库架构名（或用户名 或模式）</param>
        /// <param name="tableName">表名</param>
        /// <param name="oldTableName">原来的Table名</param>
        /// <returns>重命名Table的命令</returns>
        protected virtual string GetRenameTableCommand(string schemaName, string tableName, string oldTableName)
        {
            //若架构名为空则直接(ALTER TABLE 旧表名 RENAME TO 新表名)
            if (string.IsNullOrEmpty(schemaName))
                return $"ALTER TABLE {oldTableName} RENAME TO {tableName}";
            //若不为空则(ALTER TABLE 架构名.旧表名 RENAME TO 架构名.新表名)
            return string.Format("ALTER TABLE {0}.{1} RENAME TO {0}.{2}", schemaName, oldTableName, tableName);
        }

        /// <summary>
        /// 判断当前table是否存在
        /// </summary>
        /// <param name="dbHelper">操作数据库的Helper</param>
        /// <param name="tableHeader">Table基本信息</param>
        /// <returns>当前table是否存在</returns>
        public abstract bool IsExist(IDbHelper dbHelper, ITableHeader tableHeader);
        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="dbHelper">操作数据库的Helper</param>
        /// <param name="commandTreeFactory">创建CommandTree的工厂</param>
        /// <param name="tableMapper">Table元数据解析器</param>
        public int CreateTable(IDbHelper dbHelper, ICommandTreeFactory commandTreeFactory, ITableMapper tableMapper)
        {
            //获取数据库架构名
            string schemaName = tableMapper.Header.SchemaName ?? dbHelper.DefaultSchemaName;
            //获取ColumnNodes
            IEnumerable<IColumnNode> columnNodes = tableMapper.GetColumnMappers().Select(m => m.ToColumnNode());
            //获取建表语句生成树
            ICommandTree buildTableTree = commandTreeFactory.GetBuildTableTree(schemaName, tableMapper.Header.TableName, columnNodes);
            //创建建表语句并执行
            return dbHelper.ExecuteUpdate(buildTableTree.Compile());
        }
        /// <summary>
        /// 删除表
        /// </summary>
        /// <param name="dbHelper">操作数据库的Helper</param>
        /// <param name="tableHeader">Table头信息</param>
        public int DropTable(IDbHelper dbHelper, ITableHeader tableHeader)
        {
            // 获取删除表的sql命令
            string schemaName = tableHeader.SchemaName ?? dbHelper.DefaultSchemaName;
            string commandText = this.GetDropTableCommand(schemaName, tableHeader.TableName);
            // 执行sql命令
            return dbHelper.ExecuteUpdate(commandText);
        }
        /// <summary>
        /// 重命名表
        /// </summary>
        /// <param name="dbHelper">操作数据库的Helper</param>
        /// <param name="tableHeader">Table头信息</param>
        /// <param name="oldTableName">旧的表名</param>
        /// <returns>受影响的行数</returns>
        public int RenameTable(IDbHelper dbHelper, ITableHeader tableHeader, string oldTableName)
        {
            // 获取变更表名的sql命令
            string schemaName = tableHeader.SchemaName ?? dbHelper.DefaultSchemaName;
            string commandText = this.GetRenameTableCommand(schemaName, tableHeader.TableName, oldTableName);
            // 执行sql命令
            return dbHelper.ExecuteUpdate(commandText);
        }
    }
}
