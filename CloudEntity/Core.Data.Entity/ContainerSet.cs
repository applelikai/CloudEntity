using CloudEntity.Data.Entity;
using System.Collections.Generic;

namespace CloudEntity.Core.Data.Entity
{
    /// <summary>
    /// 数据容器集
    /// </summary>
    internal class ContainerSet
    {
        /// <summary>
        /// 线程锁
        /// </summary>
        private object locker;
        /// <summary>
        /// 初始化器
        /// </summary>
        private DbInitializer initializer;
        /// <summary>
        /// 数据容器字典
        /// </summary>
        private IDictionary<string, IDbContainer> containers;

        /// <summary>
        /// 创建容器集合
        /// </summary>
        /// <param name="dbInitializer">初始化器</param>
        public ContainerSet(DbInitializer dbInitializer)
        {
            this.locker = new object();
            this.initializer = dbInitializer;
            this.containers = new Dictionary<string, IDbContainer>();
        }
        /// <summary>
        /// 获取数据容器
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <returns>数据容器</returns>
        public IDbContainer GetContainer(string connectionString)
        {
            Start:
            //若字典中存在当前连接的数据容器，直接返回
            if (this.containers.ContainsKey(connectionString))
                return this.containers[connectionString];
            //进入临界区
            lock (this.locker)
            {
                //若字典中不存在当前连接的数据容器,则添加
                if (!this.containers.ContainsKey(connectionString))
                    this.containers.Add(connectionString, new DbContainer(connectionString, this.initializer));
                //回到Start
                goto Start;
            }
        }
    }
}
