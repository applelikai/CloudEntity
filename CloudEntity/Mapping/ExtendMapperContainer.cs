using System.Reflection;

namespace CloudEntity.Mapping
{
    /// <summary>
    /// MapperContainer扩展类
    /// 李凯 Apple_Li
    /// </summary>
    public static class ExtendMapperContainer
    {
        /// <summary>
        /// 扩展方法: 获取Column元数据解析器
        /// </summary>
        /// <param name="mapperContainer">Mapper容器</param>
        /// <param name="memberInfo">对象成员</param>
        /// <returns>Column元数据解析器</returns>
        public static IColumnMapper GetColumnMapper(this IMapperContainer mapperContainer, MemberInfo memberInfo)
        {
            //获取当前实体类型的Table元数据解析器
            ITableMapper tableMapper = mapperContainer.GetTableMapper(memberInfo.DeclaringType);
            //获取Column元数据解析器
            return tableMapper.GetColumnMapper(memberInfo.Name);
        }
        /// <summary>
        /// 扩展方法: 获取Column元数据解析器
        /// </summary>
        /// <param name="mapperContainer">Mapper容器</param>
        /// <param name="property">属性</param>
        /// <returns>Column元数据解析器</returns>
        public static IColumnMapper GetColumnMapper(this IMapperContainer mapperContainer, PropertyInfo property)
        {
            //获取当前实体类型的Table元数据解析器
            ITableMapper tableMapper = mapperContainer.GetTableMapper(property.DeclaringType);
            //获取Column元数据解析器
            return tableMapper.GetColumnMapper(property.Name);
        }
    }
}
