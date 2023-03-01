using CloudEntity.Internal.Mapping;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CloudEntity.Mapping.Common
{
    /// <summary>
    /// 实体和视图的Mapper类
    /// </summary>
    public class ViewMapper : TableMapper
    {
        /// <summary>
        /// 视图名称
        /// </summary>
        private string _viewName;
        /// <summary>
        /// 表别名
        /// </summary>
        private string _tableAlias;
        /// <summary>
        /// 架构名
        /// </summary>
        private string _schemaName;

        /// <summary>
        /// 获取表的基本信息
        /// </summary>
        /// <returns>表的基本信息</returns>
        protected override ITableHeader GetHeader()
        {
            return new TableHeader()
            {
                SchemaName = _schemaName,
                TableName = _viewName,
                TableAlias = _tableAlias ?? this.EntityType.Name.ToLower()
            };
        }
        /// <summary>
        /// 加载ColumnMapper字典，彻底填充ColumnMapper
        /// </summary>
        /// <param name="columnMappers">ColumnMapper字典</param>
        protected override void LoadColumnMappers(IDictionary<string, IColumnMapper> columnMappers)
        {
            //遍历所有属性,加载ColumnMapper字典
            foreach (PropertyInfo property in base.EntityType.GetRuntimeProperties())
            {
                //若当前属性不满足Mapping条件，本次循环
                if (!Check.IsCanMapping(property))
                    continue;
                //添加只做查询的ColumnMapper
                columnMappers.Add(property.Name, new ColumnMapper(property)
                {
                    ColumnName = property.Name,
                    ColumnAction = ColumnAction.Select
                });
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="entityType">实体类型</param>
        /// <param name="viewName">视图名称</param>
        /// <param name="tableAlias">表别名</param>
        /// <param name="schemaName">架构名</param>
        public ViewMapper(Type entityType, string viewName, string tableAlias = null, string schemaName = null)
         : base(entityType)
        {
            // 非空检查
            Check.ArgumentNull(viewName, nameof(viewName));
            // 赋值
            _viewName = viewName;
            _tableAlias = tableAlias;
            _schemaName = schemaName;
        }
    }
}