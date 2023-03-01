using System;
using System.Collections.Generic;

namespace CloudEntity.Mapping.Common
{
    /// <summary>
    /// 视图映射设置类
    /// [作者：Apple_Li 李凯 15150598493]
    /// </summary>
    public sealed class ViewMapSetter
    {
        /// <summary>
        /// table映射对象字典
        /// </summary>
        private IDictionary<Type, ITableMapper> _tableMappers;
        
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="tableMappers">table映射对象字典</param>
        public ViewMapSetter(IDictionary<Type, ITableMapper> tableMappers)
        {
            _tableMappers = tableMappers;
        }
        /// <summary>
        /// 设置实体与视图的映射关系
        /// </summary>
        /// <param name="entityType">实体类型</param>
        /// <param name="viewName">视图名称</param>
        /// <param name="tableAlias">表别名</param>
        /// <param name="schemaName">架构名</param>
        public void MapView(Type entityType, string viewName, string tableAlias = null, string schemaName = null)
        {
            // 获取视图Mapper对象
            ITableMapper tableMapper = new ViewMapper(entityType, viewName, tableAlias, schemaName);
            // 若之前已经注册了当前实体类型对应的Mapper对象
            if (_tableMappers.ContainsKey(entityType))
            {
                // 直接覆盖赋值
                _tableMappers[entityType] = tableMapper;
            }
            else
            {
                // 若之前没有注册，则直接添加
                _tableMappers.Add(entityType, tableMapper);
            }
        }
        /// <summary>
        /// 设置实体与视图的映射关系
        /// </summary>
        /// <param name="viewName">视图名称</param>
        /// <param name="tableAlias">表别名</param>
        /// <param name="schemaName">架构名</param>
        /// <typeparam name="TEntity">实体类型</typeparam>
        public void MapView<TEntity>(string viewName, string tableAlias = null, string schemaName = null)
            where TEntity : class
        {
            this.MapView(typeof(TEntity), viewName, tableAlias, schemaName);
        }
    }
}