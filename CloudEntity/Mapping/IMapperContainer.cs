using System;

namespace CloudEntity.Mapping
{
    /// <summary>
    /// Mapper容器
    /// 李凯 Apple_Li
    /// </summary>
    public interface IMapperContainer
    {
        /// <summary>
        /// 获取当前实体的存储与表的映射关系的对象
        /// </summary>
        /// <param name="entityType">实体类型</param>
        /// <returns>当前实体的存储与表的映射关系的对象</returns>
        ITableMapper GetTableMapper(Type entityType);
    }
}
