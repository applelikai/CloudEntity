using CloudEntity.Mapping;
using CloudEntity.Mapping.Common;
using CloudEntity.Test.Models;
using System;

namespace CloudEntity.Test.Mappers;

/// <summary>
/// Mapper容器
/// </summary>
public class InnerMapperContainer : MapperContainerBase
{
    /// <summary>
    /// 创建Table元数据解析器
    /// </summary>
    /// <param name="entityType">实体类型</param>
    /// <returns>Table元数据解析器</returns>
    protected override ITableMapper CreateTableMapper(Type entityType)
    {
        //Mapper类型全名
        string targetNameSpace = entityType.Namespace.Replace("Entities", "Mappers");
        string targetMapperTypeName = string.Format("{0}.{1}Mapper", targetNameSpace, entityType.Name);
        //创建Mapper对象
        return Activator.CreateInstance(Type.GetType(targetMapperTypeName)) as ITableMapper;
    }
    /// <summary>
    /// 设置视图与实体的映射关系
    /// </summary>
    /// <param name="setter">视图与实体的映射关系的设置器</param>
    protected override void SetViewMappers(ViewMapSetter setter)
    {
        setter.MapView<WechatUser>("Cus_WechatUsers");
    }
}
