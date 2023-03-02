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
        /// 数据库操作对象
        /// </summary>
        protected IDbHelper DbHelper { get; private set; }
        /// <summary>
        /// SQL命令工厂
        /// </summary>
        protected ICommandFactory CommandFactory { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="dbHelper">数据库操作对象</param>
        /// <param name="commandFactory">SQL命令工厂</param>
        public TableInitializer(IDbHelper dbHelper, ICommandFactory commandFactory)
        {
            this.DbHelper = dbHelper;
            this.CommandFactory = commandFactory;
        }
        /// <summary>
        /// 判断当前table是否存在
        /// </summary>
        /// <param name="tableHeader">Table基本信息</param>
        /// <returns>当前table是否存在</returns>
        public abstract bool IsExist(ITableHeader tableHeader);
        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="tableMapper">Table元数据解析器</param>
        public int CreateTable(ITableMapper tableMapper)
        {
            //获取数据库架构名
            string schemaName = tableMapper.Header.SchemaName ?? this.DbHelper.DefaultSchemaName;
            //获取ColumnNodes
            IEnumerable<IColumnNode> columnNodes = tableMapper.GetColumnMappers().Select(m => m.ToColumnNode());
            //获取建表语句生成树
            ICommandTree buildTableTree = this.CommandFactory.GetBuildTableTree(schemaName, tableMapper.Header.TableName, columnNodes);
            //创建建表语句并执行
            return this.DbHelper.ExecuteUpdate(buildTableTree.Compile());
        }
        /// <summary>
        /// 删除表
        /// </summary>
        /// <param name="tableHeader">Table头信息</param>
        public int DropTable(ITableHeader tableHeader)
        {
            // 获取删除表的sql命令
            string schemaName = tableHeader.SchemaName ?? this.DbHelper.DefaultSchemaName;
            string commandText = this.CommandFactory.GetDropTableCommandText(schemaName, tableHeader.TableName);
            // 执行sql命令
            return this.DbHelper.ExecuteUpdate(commandText);
        }
        /// <summary>
        /// 重命名表
        /// </summary>
        /// <param name="tableHeader">Table头信息</param>
        /// <param name="oldTableName">旧的表名</param>
        /// <returns>受影响的行数</returns>
        public int RenameTable(ITableHeader tableHeader, string oldTableName)
        {
            // 获取变更表名的sql命令
            string schemaName = tableHeader.SchemaName ?? this.DbHelper.DefaultSchemaName;
            string commandText = this.CommandFactory.GetRenameTableCommandText(schemaName, tableHeader.TableName, oldTableName);
            // 执行sql命令
            return this.DbHelper.ExecuteUpdate(commandText);
        }
    }
}
