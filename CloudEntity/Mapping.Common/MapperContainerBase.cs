using System;
using System.Collections.Generic;

namespace CloudEntity.Mapping.Common
{
    /// <summary>
    /// Mapper容器基类
    /// [作者：Apple_Li 李凯 15150598493]
    /// </summary>
    public abstract class MapperContainerBase : IMapperContainer
    {
        /// <summary>
        /// 线程锁
        /// </summary>
        private object _locker;
        /// <summary>
        /// table映射对象字典
        /// </summary>
        private IDictionary<Type, ITableMapper> _tableMappers;

        /// <summary>
        /// 创建TableMapper对象
        /// </summary>
        /// <param name="entityType">实体类型</param>
        /// <returns>当前实体的存储与表的映射关系的对象</returns>
        protected abstract ITableMapper CreateTableMapper(Type entityType);
        /// <summary>
        /// 设置视图与实体的映射关系
        /// </summary>
        /// <param name="setter">视图与实体的映射关系的设置器</param>
        protected virtual void SetViewMappers(ViewMapSetter setter) { }

        /// <summary>
        /// 创建Mapper容器
        /// </summary>
        public MapperContainerBase()
        {
            // 初始化
            _locker = new object();
            _tableMappers = new Dictionary<Type, ITableMapper>();
            // 设置视图与实体的映射关系
            ViewMapSetter viewMapSetter = new ViewMapSetter(_tableMappers);
            this.SetViewMappers(viewMapSetter);
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
            if (_tableMappers.ContainsKey(entityType))
                return _tableMappers[entityType];
            //进入临界区
            lock (_locker)
            {
                //若当前类型的TableMapper不存在，注册
                if (!_tableMappers.ContainsKey(entityType))
                    _tableMappers.Add(entityType, this.CreateTableMapper(entityType));
                //回到Start
                goto Start;
            }
        }
    }
}
