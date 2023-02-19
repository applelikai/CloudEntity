using CloudEntity.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CloudEntity.Internal.Data.Entity
{
    /// <summary>
    /// EntityAccessor扩展类
    /// </summary>
    internal static class ExtendObjectAccessor
    {
        /// <summary>
        /// Internal Extendable method: 读取DataReader当前行,为entity各个属性赋值
        /// </summary>
        /// <param name="objectAccessor">实体对象访问器</param>
        /// <param name="columnMappers">Column元数据解析器数组</param>
        /// <param name="reader">数据流</param>
        /// <param name="columnNames">查询结果列的数组</param>
        /// <param name="entity">实体对象</param>
        internal static void Fill(this ObjectAccessor objectAccessor, IEnumerable<IColumnMapper> columnMappers, IDataReader reader, string[] columnNames, object entity)
        {
            //遍历所有的PropertyMappers
            foreach (IColumnMapper columnMapper in columnMappers)
            {
                //获取查询列的列名
                string selectColumnName = columnMapper.ColumnAlias ?? columnMapper.ColumnName;
                //若查询结果列中不包含当前Property映射的列,跳过
                if (!columnNames.Contains(selectColumnName))
                    continue;
                //获取值
                object value = reader[selectColumnName];
                //若当前列值为空,则跳过,不赋值
                if (value is DBNull)
                    continue;
                //为entity当前属性赋值
                objectAccessor.SetValue(columnMapper.Property.Name, entity, value);
            }
        }
        /// <summary>
        /// Internal Extendable method: 创建实体对象
        /// </summary>
        /// <param name="objectAccessor">实体对象访问器</param>
        /// <param name="tableMapper">Table元数据解析器</param>
        /// <param name="reader">数据流</param>
        /// <param name="columnNames">查询结果列的数组</param>
        /// <returns>实体对象</returns>
        internal static object CreateEntity(this ObjectAccessor objectAccessor, ITableMapper tableMapper, IDataReader reader, string[] columnNames)
        {
            //创建对象
            object entity = objectAccessor.CreateInstance();
            //读取DataReader当前行,为entity各个属性赋值
            objectAccessor.Fill(tableMapper.GetColumnMappers(), reader, columnNames, entity);
            //返回对象
            return entity;
        }
        /// <summary>
        /// Internal Extendable method: 创建实体对象
        /// </summary>
        /// <param name="objectAccessor">实体对象访问器</param>
        /// <param name="tableMapper">Table元数据解析器</param>
        /// <param name="reader">数据流</param>
        /// <param name="columnNames">查询结果列的数组</param>
        /// <param name="accessorLinkers">对象关联属性数组</param>
        /// <returns>实体对象</returns>
        internal static object CreateEntity(this ObjectAccessor objectAccessor, ITableMapper tableMapper, IDataReader reader, string[] columnNames, AccessorLinker[] accessorLinkers)
        {
            //创建对象
            object entity = objectAccessor.CreateEntity(tableMapper, reader, columnNames);
            //遍历关联关系,为对象关联实体类型的属性赋值
            foreach (AccessorLinker accessor in accessorLinkers)
            {
                object value = accessor.EntityAccessor.CreateEntity(accessor.TableMapper, reader, columnNames, accessor.AccessorLinkers);
                objectAccessor.SetValue(accessor.PropertyName, entity, value);
            }
            //返回对象
            return entity;
        }
    }
}
