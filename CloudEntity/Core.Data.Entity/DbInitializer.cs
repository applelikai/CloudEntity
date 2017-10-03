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
        /// 创建列初始化器
        /// </summary>
        /// <returns>列初始化器</returns>
        public virtual ColumnInitializer CreateColumnInitializer()
        {
            return null;
        }
        /// <summary>
        /// 创建Table初始化器
        /// </summary>
        /// <returns>Table初始化器</returns>
        public virtual TableInitializer CreateTableInitializer()
        {
            return null;
        }
        /// <summary>
        /// 创建DbHelper对象
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <returns>DbHelper对象</returns>
        public abstract DbHelper CreateDbHelper(string connectionString);
        /// <summary>
        /// 获取创建sql命令生成树的工厂
        /// </summary>
        /// <returns>sql命令生成树的工厂</returns>
        public abstract ICommandTreeFactory CreateCommandTreeFactory();
        /// <summary>
        /// 创建Mapper容器
        /// </summary>
        /// <returns>Mapper容器</returns>
        public abstract IMapperContainer CreateMapperContainer();
    }
}
