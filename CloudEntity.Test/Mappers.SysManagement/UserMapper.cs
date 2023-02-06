using CloudEntity.Mapping;
using CloudEntity.Mapping.Common;
using CloudEntity.Test.Entities.SysManagement;

namespace CloudEntity.Test.Mappers.SysManagement;

/// <summary>
/// 用户的Mapper类
/// </summary>
public class UserMapper : TableMapper<User>
{
    /// <summary>
    /// 获取Table元数据
    /// </summary>
    /// <returns>Table元数据</returns>
    protected override ITableHeader GetHeader()
    {
        return base.GetHeader("Sys_Users");
    }
    /// <summary>
    /// 设置属性映射
    /// </summary>
    /// <param name="setter">属性映射设置器</param>
    protected override void SetColumnMappers(IColumnMapSetter<User> setter)
    {
        setter.Map(u => u.UserId, ColumnAction.PrimaryAndInsert).Length(36);
        setter.Map(u => u.UserName, ColumnAction.Insert).Length(25);
        setter.Map(u => u.Password, ColumnAction.Insert).Length(50);
        setter.Map(u => u.LogoUrl).Length(50);
        setter.Map(u => u.RoleId, allowNull: false).Length(36);
        setter.Map(u => u.RealName).Length(25);
        setter.Map(u => u.PhoneNumber).Length(25);
        setter.Map(u => u.IdNumber).Length(25);
        setter.Map(u => u.CreatedTime, ColumnAction.Default);
    }
}
