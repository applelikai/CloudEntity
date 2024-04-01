using CloudEntity.Data.Entity;
using CloudEntity.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CloudEntity.Internal.Data.Entity
{
    /// <summary>
    /// DataReader到映射对象的转换器类
    /// [作者：Apple_Li 李凯 15150598493]
    /// </summary>
    /// <typeparam name="TModel">映射对象类型</typeparam>
    internal class ReaderModelConverter<TModel>
        where TModel : class, new()
    {
        /// <summary>
        /// Mapper容器
        /// </summary>
        private readonly IMapperContainer _mapperContainer;
        /// <summary>
        /// 映射类型的存取对象
        /// </summary>
        private readonly ObjectAccessor _modelAccessor;
        /// <summary>
        /// 属性名称数组
        /// </summary>
        private readonly string[] _propertyNames;
        /// <summary>
        /// 查询列名数组
        /// </summary>
        private readonly string[] _columnNames;
        /// <summary>
        /// 实体类型与映射类型部分属性映射字典
        /// </summary>
        private readonly IDictionary<string, string> _entityModelMaps;
        /// <summary>
        /// 属性名和映射列名映射字典
        /// </summary>
        private readonly IDictionary<string, string> _propertyColumnMaps;

        /// <summary>
        /// 加载属性名和映射列名映射字典
        /// </summary>
        /// <param name="tableMapper">当前实体类型的Mapper对象</param>
        private void LoadPropertyColumnMaps(ITableMapper tableMapper)
        {
            // 遍历当前实体类型所有ColumnMapper列表
            foreach (IColumnMapper columnMapper in tableMapper.GetColumnMappers())
            {
                // 获取查询列名
                string selectName = columnMapper.ColumnAlias ?? columnMapper.ColumnName;
                // 若查询的列名数组不包含当前的查询列名，跳过
                if (!_columnNames.Contains(selectName))
                    continue;

                // 获取属性名，默认与实体对象属性一致
                string propertyName = columnMapper.Property.Name;
                // 若当前属性包含映射属性名，则获取映射属性名
                if (_entityModelMaps.ContainsKey(propertyName))
                    propertyName = _entityModelMaps[propertyName];
                // 若映射对象属性名不包含当前属性名，跳过
                if (!_propertyNames.Contains(propertyName))
                    continue;

                // 若字典中已包含当前属性名，跳过
                if (_propertyColumnMaps.ContainsKey(propertyName))
                    continue;
                // 添加属性名和映射列名映射
                _propertyColumnMaps.Add(propertyName, selectName);
            }
        }
        /// <summary>
        /// 加载属性名和映射列名映射字典
        /// </summary>
        /// <param name="tableMapper">当前实体类型的Mapper对象</param>
        /// <param name="propertyLinkers">关联对象属性链接列表</param>
        private void LoadPropertyColumnMaps(ITableMapper tableMapper, IEnumerable<PropertyLinker> propertyLinkers)
        {
            // 加载当前实体属性名和映射列名映射字典
            this.LoadPropertyColumnMaps(tableMapper);
            // 遍历关联对象属性链接列表
            foreach (PropertyLinker propertyLinker in propertyLinkers)
            {
                // 获取关联对象类型的Mapper对象
                ITableMapper linkMapper = _mapperContainer.GetTableMapper(propertyLinker.Property.PropertyType);
                // 加载关联对象类型属性名和映射列名映射字典
                this.LoadPropertyColumnMaps(linkMapper, propertyLinker.RelationalLinkers);
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="columnNames">查询列名数组</param>
        /// <param name="entityType">实体类型</param>
        /// <param name="mapperContainer">Mapper容器</param>
        /// <param name="propertyLinkers">关联对象属性链接列表</param>
        public ReaderModelConverter(string[] columnNames, Type entityType, IMapperContainer mapperContainer, IEnumerable<PropertyLinker> propertyLinkers)
        {
            // 初始化Mapper容器
            _mapperContainer = mapperContainer;
            // 初始化映射类型的存取对象
            _modelAccessor = ObjectAccessor.GetAccessor(typeof(TModel));
            // 初始化映射类型属性名称数组
            _propertyNames = _modelAccessor.GetPropertyNames().ToArray();
            // 初始化查询列名数组
            _columnNames = columnNames;
            // 初始化实体类型与映射类型部分属性映射字典
            _entityModelMaps = new Dictionary<string, string>();
            // 初始化属性名和映射列名映射字典
            _propertyColumnMaps = new Dictionary<string, string>();

            // 获取当前实体类型的Mapper对象
            ITableMapper tableMapper = mapperContainer.GetTableMapper(entityType);
            // 加载属性名和映射列名映射字典
            this.LoadPropertyColumnMaps(tableMapper, propertyLinkers);
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="columnNames">查询列名数组</param>
        /// <param name="entityType">实体类型</param>
        /// <param name="mapperContainer">Mapper容器</param>
        /// <param name="propertyLinkers">关联对象属性链接列表</param>
        /// <param name="entityModelMaps">实体类型与映射类型部分属性映射字典</param>
        public ReaderModelConverter(string[] columnNames, Type entityType, IMapperContainer mapperContainer, IEnumerable<PropertyLinker> propertyLinkers, IDictionary<string, string> entityModelMaps)
        {
            // 初始化Mapper容器
            _mapperContainer = mapperContainer;
            // 初始化映射类型的存取对象
            _modelAccessor = ObjectAccessor.GetAccessor(typeof(TModel));
            // 初始化映射类型属性名称数组
            _propertyNames = _modelAccessor.GetPropertyNames().ToArray();
            // 初始化查询列名数组
            _columnNames = columnNames;
            // 初始化实体类型与映射类型部分属性映射字典
            _entityModelMaps = entityModelMaps;
            // 初始化属性名和映射列名映射字典
            _propertyColumnMaps = new Dictionary<string, string>();

            // 获取当前实体类型的Mapper对象
            ITableMapper tableMapper = mapperContainer.GetTableMapper(entityType);
            // 加载属性名和映射列名映射字典
            this.LoadPropertyColumnMaps(tableMapper, propertyLinkers);
        }
        /// <summary>
        /// 转换数据流当前行获取映射对象
        /// </summary>
        /// <param name="reader">数据流</param>
        /// <returns>映射对象</returns>
        public TModel Convert(IDataReader reader)
        {
            // 创建映射对象
            TModel model = new TModel();
            // 遍历属性名和映射列名映射字典
            foreach (KeyValuePair<string, string> propertyColumnMap in _propertyColumnMaps)
            {
                // 获取值
                object value = reader[propertyColumnMap.Value];
                // 若当前列值为空,则跳过,不赋值
                if (value is DBNull)
                    continue;
                // 为映射对象当前属性赋转换值(若值类型不一致则先转换)
                _modelAccessor.SetConvertValue(propertyColumnMap.Key, model, value);
            }
            // 获取最终的映射对象
            return model;
        }
    }
}
