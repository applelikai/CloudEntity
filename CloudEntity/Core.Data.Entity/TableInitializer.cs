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
            //获取ColumnNodes
            IEnumerable<IColumnNode> columnNodes = tableMapper.GetColumnMappers().Select(m => m.ToColumnNode());
            //获取建表语句生成树
            ICommandTree buildTableTree = commandTreeFactory.CreateBuildTableTree(tableMapper.Header.TableFullName, columnNodes);
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
            return dbHelper.ExecuteUpdate(string.Concat("DROP TABLE ", tableHeader.TableFullName));
        }
        /// <summary>
        /// 重命名表
        /// </summary>
        /// <param name="dbHelper">操作数据库的Helper</param>
        /// <param name="tableHeader">Table头信息</param>
        /// <param name="oldTableName">旧的表名</param>
        /// <returns>受影响的行数</returns>
        public virtual int RenameTable(DbHelper dbHelper, ITableHeader tableHeader, string oldTableName)
        {
            return dbHelper.ExecuteUpdate(string.Format("ALTER TABLE {0} RENAME TO {1}", oldTableName, tableHeader.TableName));
        }
    }
}
