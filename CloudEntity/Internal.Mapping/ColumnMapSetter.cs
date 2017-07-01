using CloudEntity.Mapping;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace CloudEntity.Internal.Mapping
{
    /// <summary>
    /// 设置当前对象列的映射关系的类
    /// </summary>
    /// <typeparam name="TEntity">实体</typeparam>
    internal class ColumnMapSetter <TEntity> : IColumnMapSetter<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// 表的别名
        /// </summary>
        private string tableAlias;
        /// <summary>
        /// 列和属性的映射对象字典
        /// </summary>
        private IDictionary<string, IColumnMapper> columnMappers;

        /// <summary>
        /// 创建设置列与属性映射关系的对象
        /// </summary>
        /// <param name="tableAlias">表的别名</param>
        /// <param name="columnMappers">列与属性映射对象的字典</param>
        public ColumnMapSetter(string tableAlias, IDictionary<string, IColumnMapper> columnMappers)
        {
            Check.ArgumentNull(tableAlias, nameof(tableAlias));
            Check.ArgumentNull(columnMappers, nameof(columnMappers));
            this.tableAlias = tableAlias;
            this.columnMappers = columnMappers;
        }
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
        public void Map
        (
            Expression<Func<TEntity, object>> selector,
            ColumnAction action = ColumnAction.InsertAndEdit,
            string columnName = null, 
            string dataType = null, 
            int? length = null, 
            bool useAlias = false, 
            string columnAlias = null
        )
        {
            //获取并检查属性
            PropertyInfo property = selector.Body.GetProperty();
            if (!Check.IsCanMapping(property))
                return;
            if (this.columnMappers.ContainsKey(property.Name))
                return;
            //创建ColumnMapper
            IColumnMapper columnMapper = new ColumnMapper(property)
            {
                ColumnAction = action,
                ColumnName = columnName ?? property.Name,
                ColumnFullName = string.Format("{0}.{1}", this.tableAlias, columnName ?? property.Name),
                DataType = dataType,
                Length = length,
                ColumnAlias = useAlias ? columnAlias ?? property.Name : string.Empty
            };
            //添加columnMapper
            this.columnMappers.Add(property.Name, columnMapper);
        }
    }
}
