using CloudEntity.Mapping.Common;
using System;
using System.Collections.Generic;
using System.Text;
using CloudEntity.Mapping;

namespace CloudEntity.Test.MySqlClient
{
    /// <summary>
    /// Mapper容器
    /// </summary>
    public class InnerMapperContainer : MapperContainerBase
    {
        /// <summary>
        /// 创建Mapper对象
        /// </summary>
        /// <param name="entityType">实体类型</param>
        /// <returns>Mapper对象</returns>
        protected override ITableMapper CreateTableMapper(Type entityType)
        {
            Type mapperType = Type.GetType(string.Format("CloudEntity.Test.Mappers.{0}Mapper", entityType.Name));
            return Activator.CreateInstance(mapperType) as ITableMapper;
        }
    }
}
