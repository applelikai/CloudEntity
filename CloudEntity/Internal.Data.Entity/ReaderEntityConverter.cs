using CloudEntity.Data.Entity;
using CloudEntity.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CloudEntity.Internal.Data.Entity
{
    /// <summary>
    /// DataReader到实体对象的转换器类
    /// [作者：Apple_Li 李凯 15150598493]
    /// </summary>
    internal class ReaderEntityConverter
    {
        /// <summary>
        /// 当前实体类型的存取对象
        /// </summary>
        private readonly ObjectAccessor _entityAccessor;
        /// <summary>
        /// 列映射对象数组
        /// </summary>
        private readonly IColumnMapper[] _columnMappers;
        /// <summary>
        /// 关联实体对象的转换器对象字典
        /// </summary>
        private readonly IDictionary<string, ReaderEntityConverter> _linkConverters;

        /// <summary>
        /// 获取查询的列名对应的所有ColumnMapper对象列表
        /// </summary>
        /// <param name="tableMapper">当前实体类型的Mapper对象</param>
        /// <param name="columnNames">查询列名数组</param>
        /// <returns>查询的列名对应的所有ColumnMapper对象列表</returns>
        private IEnumerable<IColumnMapper> GetColumnMappers(ITableMapper tableMapper, string[] columnNames)
        {
            // 遍历当前实体类型所有ColumnMapper列表
            foreach (IColumnMapper columnMapper in tableMapper.GetColumnMappers())
            {
                // 获取查询列名
                string selectName = columnMapper.ColumnAlias ?? columnMapper.ColumnName;
                // 若查询的列名数组包含当前的查询列名
                if (columnNames.Contains(selectName))
                {
                    // 则直接获取当前的ColumnMapper
                    yield return columnMapper;
                }
            }
        }
        /// <summary>
        /// 为实体对象设置基础类型属性的值
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="reader">数据流</param>
        private void SetBasePropertyValues(object entity, IDataReader reader)
        {
            // 遍历属性与列名映射对象数组
            foreach (IColumnMapper columnMapper in _columnMappers)
            {
                // 获取查询列的列名
                string selectColumnName = columnMapper.ColumnAlias ?? columnMapper.ColumnName;
                // 获取值
                object value = reader[selectColumnName];
                // 若当前列值为空,则跳过,不赋值
                if (value is DBNull)
                    continue;
                // 为实体对象当前属性赋值
                _entityAccessor.SetValue(columnMapper.Property.Name, entity, value);
            }
        }
        /// <summary>
        /// 为实体对象设置关联类型属性的值
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="reader">数据流</param>
        private void SetLinkPropertyValues(object entity, IDataReader reader)
        {
            // 遍历关联实体对象的转换器对象字典
            foreach (KeyValuePair<string, ReaderEntityConverter> converMap in _linkConverters)
            {
                // 创建关联对象作为关联实体对象属性的值
                object value = converMap.Value.Convert(reader);
                // 为实体对象当前关联对象属性赋值
                _entityAccessor.SetValue(converMap.Key, entity, value);
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="columnNames">查询列名数组</param>
        /// <param name="entityType">实体类型</param>
        /// <param name="tableMapper">实体类型的Mapper对象</param>
        public ReaderEntityConverter(string[] columnNames, Type entityType, ITableMapper tableMapper)
        {
            // 获取实体类型访问对象
            _entityAccessor = ObjectAccessor.GetAccessor(entityType);
            // 获取查询的列名对应的所有ColumnMapper对象数组
            _columnMappers = this.GetColumnMappers(tableMapper, columnNames).ToArray();
            // 初始化转关联属性名称与换器映射字典
            _linkConverters = new Dictionary<string, ReaderEntityConverter>();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="columnNames">查询列名数组</param>
        /// <param name="entityType">实体类型</param>
        /// <param name="mapperContainer">Mapper容器</param>
        public ReaderEntityConverter(string[] columnNames, Type entityType, IMapperContainer mapperContainer)
        {
            // 获取实体类型访问对象
            _entityAccessor = ObjectAccessor.GetAccessor(entityType);

            // 获取当前实体类型的Mapper对象
            ITableMapper tableMapper = mapperContainer.GetTableMapper(entityType);
            // 获取查询的列名对应的所有ColumnMapper对象数组
            _columnMappers = this.GetColumnMappers(tableMapper, columnNames).ToArray();

            // 初始化转关联属性名称与换器映射字典
            _linkConverters = new Dictionary<string, ReaderEntityConverter>();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="columnNames">查询列名数组</param>
        /// <param name="entityType">实体类型</param>
        /// <param name="mapperContainer">Mapper容器</param>
        /// <param name="propertyLinkers">关联对象属性链接列表</param>
        public ReaderEntityConverter(string[] columnNames, Type entityType, IMapperContainer mapperContainer, IEnumerable<PropertyLinker> propertyLinkers)
            : this(columnNames, entityType, mapperContainer)
        {
            // 遍历关联对象属性链接列表
            foreach (PropertyLinker propertyLinker in propertyLinkers)
            {
                // 获取转换器对象
                ReaderEntityConverter converter = new ReaderEntityConverter(columnNames, propertyLinker.Property.PropertyType, mapperContainer, propertyLinker.RelationalLinkers);
                // 添加到关联对象对应的转换器映射字典
                _linkConverters.Add(propertyLinker.Property.Name, converter);
            }
        }
        /// <summary>
        /// 转换数据流当前行获取实体对象
        /// </summary>
        /// <param name="reader">数据流</param>
        /// <returns>实体对象</returns>
        public object Convert(IDataReader reader)
        {
            // 创建当前实体对象
            object entity = _entityAccessor.CreateInstance();
            // 为实体对象设置基础类型属性的值
            this.SetBasePropertyValues(entity, reader);
            // 为实体对象设置关联类型属性的值
            this.SetLinkPropertyValues(entity, reader);
            // 获取实体对象
            return entity;
        }
    }
}
