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
        /// <param name="tableHeader">Table元数据</param>
        /// <returns>删除Table的命令</returns>
        protected virtual string GetDropTableCommand(ITableHeader tableHeader)
        {
            //若架构名为空则直接(DROP TABLE 表名)
            if (string.IsNullOrEmpty(tableHeader.SchemaName))
                return $"DROP TABLE {tableHeader.TableName}";
            //若不为空则(DROP TABLE 架构名.表名)
            return $"DROP TABLE {tableHeader.SchemaName}.{tableHeader.TableName}";
        }
        /// <summary>
        /// 获取重命名Table的命令
        /// </summary>
        /// <param name="tableHeader">Table元数据</param>
        /// <param name="oldTableName">原来的Table名</param>
        /// <returns>重命名Table的命令</returns>
        protected virtual string GetRenameTableCommand(ITableHeader tableHeader, string oldTableName)
        {
            //若架构名为空则直接(ALTER TABLE 旧表名 RENAME TO 新表名)
            if (string.IsNullOrEmpty(tableHeader.SchemaName))
                return $"ALTER TABLE {oldTableName} RENAME TO {tableHeader.TableName}";
            //若不为空则(ALTER TABLE 架构名.旧表名 RENAME TO 架构名.新表名)
            return string.Format("ALTER TABLE {0}.{1} RENAME TO {0}.{2}", tableHeader.SchemaName, oldTableName, tableHeader.TableName);
        }

        /// <summary>
        /// 判断当前table是否存在
        /// </summary>
        /// <param name="dbHelper">操作数据库的Helper</param>
        /// <param name="tableHeader">Table基本信息</param>
        /// <returns>当前table是否存在</returns>
        public abstract bool IsExist(DbHelper dbHelper, ITableHeader tableHeader);
        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="dbHelper">操作数据库的Helper</param>
        /// <param name="commandTreeFactory">创建CommandTree的工厂</param>
        /// <param name="tableMapper">Table元数据解析器</param>
        public int CreateTable(DbHelper dbHelper, ICommandTreeFactory commandTreeFactory, ITableMapper tableMapper)
        {
            //获取TableHeader
            ITableHeader tableHeader = tableMapper.Header;
            //获取ColumnNodes
            IEnumerable<IColumnNode> columnNodes = tableMapper.GetColumnMappers().Select(m => m.ToColumnNode());
            //获取建表语句生成树
            ICommandTree buildTableTree = commandTreeFactory.CreateBuildTableTree(tableHeader.SchemaName, tableHeader.TableName, columnNodes);
            //创建建表语句并执行
            return dbHelper.ExecuteUpdate(buildTableTree.Compile());
        }
        /// <summary>
        /// 删除表
        /// </summary>
        /// <param name="dbHelper">操作数据库的Helper</param>
        /// <param name="tableHeader">Table头信息</param>
        public int DropTable(DbHelper dbHelper, ITableHeader tableHeader)
        {
            return dbHelper.ExecuteUpdate(this.GetDropTableCommand(tableHeader));
        }
        /// <summary>
        /// 重命名表
        /// </summary>
        /// <param name="dbHelper">操作数据库的Helper</param>
        /// <param name="tableHeader">Table头信息</param>
        /// <param name="oldTableName">旧的表名</param>
        /// <returns>受影响的行数</returns>
        public int RenameTable(DbHelper dbHelper, ITableHeader tableHeader, string oldTableName)
        {
            return dbHelper.ExecuteUpdate(this.GetRenameTableCommand(tableHeader, oldTableName));
        }
    }
}
