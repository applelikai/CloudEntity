using System;
using System.Collections.Generic;

namespace CloudEntity.Mapping.Common
{
    /// <summary>
    /// Mapper容器
    /// 李凯 Apple_Li
    /// </summary>
    public class MapperContainer : IMapperContainer
    {
        /// <summary>
        /// 线程锁
        /// </summary>
        private object locker;
        /// <summary>
        /// table映射对象字典
        /// </summary>
        private IDictionary<Type, ITableMapper> tableMappers;

        /// <summary>
        /// 创建Mapper容器
        /// </summary>
        public MapperContainer()
        {
            this.locker = new object();
            this.tableMappers = new Dictionary<Type, ITableMapper>();
        }
        /// <summary>
        /// 获取当前实体的存储与表的映射关系的对象
        /// </summary>
        /// <param name="entityType">实体类型</param>
        /// <returns>当前实体的存储与表的映射关系的对象</returns>
        public virtual ITableMapper GetTableMapper(Type entityType)
        {
            if (this.tableMappers.ContainsKey(entityType))
                return this.tableMappers[entityType];
            throw new Exception(string.Format("Please register {0}'s tableMapper", entityType.Name));
        }
        /// <summary>
        /// 注册Table映射对象
        /// </summary>
        /// <param name="tableMapper">Table映射对象</param>
        public void RegisterMapper(ITableMapper tableMapper)
        {
            Start:
            //若当前类型的TableMapper存在，退出
            if (this.tableMappers.ContainsKey(tableMapper.EntityType))
                return;
            //进入临界区
            lock (this.locker)
            {
                //若当前类型的TableMapper不存在，注册
                if (!this.tableMappers.ContainsKey(tableMapper.EntityType))
                    this.tableMappers.Add(tableMapper.EntityType, tableMapper);
                //回到Start
                goto Start;
            }
        }
    }
}
