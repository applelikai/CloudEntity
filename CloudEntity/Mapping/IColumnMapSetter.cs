using System;
using System.Linq.Expressions;

namespace CloudEntity.Mapping
{
    /// <summary>
    /// 设置当前对象列的映射关系的接口
    /// 李凯 Apple_Li
    /// </summary>
    /// <typeparam name="TEntity">实体</typeparam>
    public interface IColumnMapSetter <TEntity>
        where TEntity : class
    {
        /// <summary>
        /// 设置列的映射
        /// </summary>
        /// <param name="selector">执行当前实体某属性</param>
        /// <param name="action">指定对该列的操作</param>
        /// <param name="columnName">列名</param>
        /// <param name="dataType">数据类型(如 VARCHAR INT等),可以不设置</param>
        /// <param name="length">类型长度</param>
        /// <param name="columnAlias">是否使用别名查询(默认为false)</param>
        /// <param name="useAlias">列的别名(使用别名查询时，此值为空，则取属性名为别名)</param>
        void Map
        (
            Expression<Func<TEntity, object>> selector,
            ColumnAction action = ColumnAction.InsertAndEdit,
            string columnName = null,
            string dataType = null,
            int? length = null,
            bool useAlias = false,
            string columnAlias = null
        );
    }
}
