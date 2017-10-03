using System;
using System.Linq.Expressions;

namespace CloudEntity.Mapping
{
    /// <summary>
    /// 设置当前对象列的映射关系的接口
    /// 李凯 Apple_Li
    /// </summary>
    /// <typeparam name="TEntity">实体</typeparam>
    public interface IColumnMapSetter<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// 设置列的映射
        /// </summary>
        /// <param name="selector">执行当前实体某属性</param>
        /// <param name="action">指定对该列的操作</param>
        /// <param name="columnName">列名</param>
        /// <param name="allowNull">是否允许为空</param>
        /// <returns>列信息设置器</returns>
        IColumnSetter Map(Expression<Func<TEntity, object>> selector, ColumnAction action = ColumnAction.InsertAndEdit, string columnName = null, bool allowNull = false);
    }
}
