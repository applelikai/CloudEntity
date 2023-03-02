using CloudEntity.CommandTrees;
using CloudEntity.Data;
using CloudEntity.Mapping;

namespace CloudEntity.Core.Data.Entity
{
    /// <summary>
    /// 初始化器
    /// 李凯 Apple_Li
    /// </summary>
    public abstract class DbInitializer
    {
        /// <summary>
        /// 创建DbHelper对象
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <returns>DbHelper对象</returns>
        public abstract DbHelper CreateDbHelper(string connectionString);
        /// <summary>
        /// 创建获取sql的命令工厂
        /// </summary>
        /// <returns>获取sql的命令工厂</returns>
        public abstract ICommandFactory CreateCommandFactory();
        /// <summary>
        /// 创建Mapper容器
        /// </summary>
        /// <returns>Mapper容器</returns>
        public abstract IMapperContainer CreateMapperContainer();
        /// <summary>
        /// 创建Table初始化器
        /// </summary>
        /// <param name="dbHelper">数据库操作对象</param>
        /// <param name="commandFactory">SQL命令工厂</param>
        /// <returns>Table初始化器</returns>
        public virtual TableInitializer CreateTableInitializer(IDbHelper dbHelper, ICommandFactory commandFactory)
        {
            return null;
        }
        /// <summary>
        /// 创建列初始化器
        /// </summary>
        /// <param name="dbHelper">数据库操作对象</param>
        /// <param name="commandFactory">SQL命令工厂</param>
        /// <returns>列初始化器</returns>
        public virtual ColumnInitializer CreateColumnInitializer(IDbHelper dbHelper, ICommandFactory commandFactory)
        {
            return null;
        }
    }
}
