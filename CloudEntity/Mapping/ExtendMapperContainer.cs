using System.Linq.Expressions;
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
        /// <param name="memberExpression">指定对象成员的表达式</param>
        /// <returns>Column元数据解析器</returns>
        public static IColumnMapper GetColumnMapper(this IMapperContainer mapperContainer, MemberExpression memberExpression)
        {
            //获取当前实体类型的Table元数据解析器
            ITableMapper tableMapper = mapperContainer.GetTableMapper(memberExpression.Expression.Type);
            //获取Column元数据解析器
            return tableMapper.GetColumnMapper(memberExpression.Member.Name);
        }
    }
}
