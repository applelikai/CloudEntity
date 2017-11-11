using CloudEntity.Mapping;
using System;
using System.Collections.Generic;
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
        private IMapperContainer mapperContainer;

        /// <summary>
        /// 创建列获取器
        /// </summary>
        /// <param name="mapperContainer">mapper容器</param>
        public MapperColumnGetter(IMapperContainer mapperContainer)
        {
            this.mapperContainer = mapperContainer;
        }
        /// <summary>
        /// 获取列全名
        /// </summary>
        /// <param name="property">属性</param>
        /// <returns>列的全名(临时表名.列名)</returns>
        public string GetColumnFullName(PropertyInfo property)
        {
            return this.mapperContainer.GetColumnMapper(property).ColumnFullName;
        }
    }
}
