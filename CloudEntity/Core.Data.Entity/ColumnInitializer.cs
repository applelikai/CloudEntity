using CloudEntity.CommandTrees;
using CloudEntity.Data;
using CloudEntity.Mapping;
using System.Collections.Generic;
using System.Linq;

namespace CloudEntity.Core.Data.Entity
{
    /// <summary>
    /// 列元数据初始化器
    /// </summary>
    public abstract class ColumnInitializer
    {
        /// <summary>
        /// 获取当前Table下所有的列
        /// </summary>
        /// <param name="dbHelper">操作数据库的Helper对象</param>
        /// <param name="tableHeader">Table基本信息</param>
        /// <returns>当前Table下所有的列</returns>
        protected abstract IEnumerable<string> GetColumns(IDbHelper dbHelper, ITableHeader tableHeader);

        /// <summary>
        /// 为当前实体所Mapping的Table添加没有添加的列
        /// </summary>
        /// <param name="dbHelper">操作数据库的Helper对象</param>
        /// <param name="commandTreeFactory">创建CommandTree的工厂</param>
        /// <param name="tableMapper">Table元数据解析器</param>
        public void AlterTableAddColumns(IDbHelper dbHelper, ICommandTreeFactory commandTreeFactory, ITableMapper tableMapper)
        {
            //获取当前实体元数据解析器中某些属性Mapping的Column未包含在Table中的属性对应的ColumnMapper,并转换获取Column节点
            string[] columnNames = this.GetColumns(dbHelper, tableMapper.Header).ToArray();
            IColumnNode[] columnNodes = (from columnMapper
                                           in tableMapper.GetColumnMappers()
                                        where !columnNames.Contains(columnMapper.ColumnName)
                                       select columnMapper.ToColumnNode()).ToArray();
            //若实体元数据解析器中所有属性Mapping的列都在当前Table中,直接退出
            if (columnNodes.Length == 0)
                return;
            //获取数据库架构名
            string schemaName = tableMapper.Header.SchemaName ?? dbHelper.DefaultSchemaName;
            //获取表名
            string tableName = tableMapper.Header.TableName;
            //创建Alter Table Columns语句生成树
            ICommandTree alterTableAddColumnsTree = commandTreeFactory.GetAlterTableAddColumnsTree(schemaName, tableName, columnNodes);
            //生成并执行Alter Table Columns语句，为当前实体Mapping的Table添加为注册的列
            dbHelper.ExecuteUpdate(alterTableAddColumnsTree.Compile());
        }
    }
}
