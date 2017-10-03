using System;
using System.Collections.Generic;

namespace CloudEntity.Mapping.Common
{
    /// <summary>
    /// Mapper容器基类
    /// 李凯 Apple_Li
    /// </summary>
    public abstract class MapperContainerBase : IMapperContainer
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
        /// 创建TableMapper对象
        /// </summary>
        /// <param name="entityType">实体类型</param>
        /// <returns>当前实体的存储与表的映射关系的对象</returns>
        protected abstract ITableMapper CreateTableMapper(Type entityType);

        /// <summary>
        /// 创建Mapper容器
        /// </summary>
        public MapperContainerBase()
        {
            this.locker = new object();
            this.tableMappers = new Dictionary<Type, ITableMapper>();
        }
        /// <summary>
        /// 获取当前实体的存储与表的映射关系的对象
        /// </summary>
        /// <param name="entityType">实体类型</param>
        /// <returns>当前实体的存储与表的映射关系的对象</returns>
        public ITableMapper GetTableMapper(Type entityType)
        {
            Start:
            //若当前类型的TableMapper存在，直接返回
            if (this.tableMappers.ContainsKey(entityType))
                return this.tableMappers[entityType];
            //进入临界区
            lock (this.locker)
            {
                //若当前类型的TableMapper不存在，注册
                if (!this.tableMappers.ContainsKey(entityType))
                    this.tableMappers.Add(entityType, this.CreateTableMapper(entityType));
                //回到Start
                goto Start;
            }
        }
    }
}
