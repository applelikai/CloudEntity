using System;
using System.Collections.Generic;

namespace CloudEntity.Mapping
{
    /// <summary>
    /// 实体和表的映射接口
    /// Apple_Li 李凯 15150598493
    /// </summary>
    public interface ITableMapper
    {
        /// <summary>
        /// 实体类型
        /// </summary>
        Type EntityType { get; }
        /// <summary>
        /// 对应的表的基本信息
        /// </summary>
        ITableHeader Header { get; }
        /// <summary>
        /// 主键列与属性的映射对象
        /// </summary>
        IColumnMapper KeyMapper { get; }
        
        /// <summary>
        /// 获取当前属性对应的ColumnMapper
        /// </summary>
        /// <param name="propertyName">属性名</param>
        /// <returns>当前属性对应的ColumnMapper</returns>
        IColumnMapper GetColumnMapper(string propertyName);
        /// <summary>
        /// 获取所有的列与属性的映射对象
        /// </summary>
        /// <returns>所有的列与属性的映射对象</returns>
        IEnumerable<IColumnMapper> GetColumnMappers();
    }
}
