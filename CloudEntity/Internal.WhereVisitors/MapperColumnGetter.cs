using CloudEntity.Mapping;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace CloudEntity.Internal.WhereVisitors
{
    /// <summary>
    /// Mapping列获取类
    /// </summary>
    internal class MapperColumnGetter : IColumnGetter
    {
        /// <summary>
        /// mapper容器
        /// </summary>
        private IMapperContainer _mapperContainer;

        /// <summary>
        /// 创建列获取器
        /// </summary>
        /// <param name="mapperContainer">mapper容器</param>
        public MapperColumnGetter(IMapperContainer mapperContainer)
        {
            _mapperContainer = mapperContainer;
        }
        /// <summary>
        /// 获取列全名
        /// </summary>
        /// <param name="memberExpression">指定属性表达式</param>
        /// <returns>列的全名(临时表名.列名)</returns>
        public string GetColumnFullName(MemberExpression memberExpression)
        {
            return _mapperContainer.GetColumnMapper(memberExpression).ColumnFullName;
        }
    }
}
