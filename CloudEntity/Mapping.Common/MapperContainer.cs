using System;
using System.Collections.Generic;

namespace CloudEntity.Mapping.Common
{
    /// <summary>
    /// Mapper容器
    /// 李凯 Apple_Li
    /// </summary>
    public class MapperContainer : MapperContainerBase
    {
        /// <summary>
        /// 创建TableMapper的委托字典
        /// </summary>
        private IDictionary<Type, Func<ITableMapper>> tableMapperCreators;

        /// <summary>
        /// 创建TableMapper对象
        /// </summary>
        /// <param name="entityType">实体类型</param>
        /// <returns>当前实体的存储与表的映射关系的对象</returns>
        protected override ITableMapper CreateTableMapper(Type entityType)
        {
            //若当前实体类型对应的创建TableMapper的委托存在，直接获取
            if (this.tableMapperCreators.ContainsKey(entityType))
                return this.tableMapperCreators[entityType]();
            //若不存在则扔出异常
            throw new Exception(string.Format("Can not find entity type {0}", entityType.FullName));
        }

        /// <summary>
        /// 创建Mapper容器
        /// </summary>
        public MapperContainer()
        {
            this.tableMapperCreators = new Dictionary<Type, Func<ITableMapper>>();
        }
        /// <summary>
        /// 设置实体类型对应的TableMapper类型
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TMapper">TableMapper类型</typeparam>
        public void Map<TEntity, TMapper>()
            where TEntity : class
            where TMapper : TableMapper<TEntity>, new()
        {
            //若当前实体类型未映射ITableMapper类型
            if (!this.tableMapperCreators.ContainsKey(typeof(TEntity)))
                //则注册
                this.tableMapperCreators.Add(typeof(TEntity), () => new TMapper());
        }
    }
}
