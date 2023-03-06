using CloudEntity.CommandTrees;
using CloudEntity.Data;
using CloudEntity.Mapping;
using System.Collections.Generic;
using System.Linq;

namespace CloudEntity.Core.Data.Entity
{
    /// <summary>
    /// 列元数据初始化器
    /// [Apple_Li 李凯 15150598493]
    /// </summary>
    public abstract class ColumnInitializer
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
        /// 获取当前Table下所有的列
        /// </summary>
        /// <param name="tableHeader">Table基本信息</param>
        /// <returns>当前Table下所有的列</returns>
        protected abstract IEnumerable<string> GetColumns(ITableHeader tableHeader);

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="dbHelper">数据库操作对象</param>
        /// <param name="commandFactory">SQL命令工厂</param>
        public ColumnInitializer(IDbHelper dbHelper, ICommandFactory commandFactory)
        {
            this.DbHelper = dbHelper;
            this.CommandFactory = commandFactory;
        }
        /// <summary>
        /// 为当前实体所Mapping的Table添加没有添加的列
        /// </summary>
        /// <param name="tableMapper">Table元数据解析器</param>
        public void AlterTableAddColumns(ITableMapper tableMapper)
        {
            //获取当前实体元数据解析器中某些属性Mapping的Column未包含在Table中的属性对应的ColumnMapper,并转换获取Column节点
            string[] columnNames = this.GetColumns(tableMapper.Header).ToArray();
            IColumnNode[] columnNodes = (from columnMapper
                                           in tableMapper.GetColumnMappers()
                                        where !columnNames.Contains(columnMapper.ColumnName)
                                       select columnMapper.ToColumnNode()).ToArray();
            //若实体元数据解析器中所有属性Mapping的列都在当前Table中,直接退出
            if (columnNodes.Length == 0)
                return;
            //获取数据库架构名
            string schemaName = tableMapper.Header.SchemaName ?? this.DbHelper.DefaultSchemaName;
            //获取表名
            string tableName = tableMapper.Header.TableName;
            //创建Alter Table ColumnsSQL
            string commandText = this.CommandFactory.GetAlterTableAddColumnsCommandText(schemaName, tableName, columnNodes);
            //生成并执行Alter Table Columns语句，为当前实体Mapping的Table添加为注册的列
            this.DbHelper.ExecuteUpdate(commandText);
        }
    }
}
