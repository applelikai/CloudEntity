using CloudEntity.Mapping;
using CloudEntity.Mapping.Common;
using CloudEntity.Test.Entities.SysManagement;

namespace CloudEntity.Test.Mappers.SysManagement;

/// <summary>
/// 角色的Mapper类
/// </summary>
public class RoleMapper : TableMapper<Role>
{
    /// <summary>
    /// 获取Mapping的Table信息
    /// </summary>
    /// <returns>获取Table信息</returns>
    protected override ITableHeader GetHeader()
    {
        return base.GetHeader("Sys_Roles");
    }
    /// <summary>
    /// 设置属性映射
    /// </summary>
    /// <param name="setter">属性映射设置器</param>
    protected override void SetColumnMappers(IColumnMapSetter<Role> setter)
    {
        setter.Map(r => r.RoleId, ColumnAction.PrimaryAndInsert).Length(36);
        setter.Map(r => r.RoleName, allowNull: false).Length(25);
        setter.Map(r => r.Remark).Length(100);
        setter.Map(r => r.CreatedTime, ColumnAction.Default);
        setter.Map(r => r.LastUpdateDate, ColumnAction.EditAndDefault);
    }
}
