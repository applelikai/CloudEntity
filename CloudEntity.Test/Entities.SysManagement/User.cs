using System;

namespace CloudEntity.Test.Entities.SysManagement;

public class User
{
    /// <summary>
    /// 用户id
    /// </summary>
    public string UserId { get; set; }
    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; }
    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; }
    /// <summary>
    /// logo头像
    /// </summary>
    public string LogoUrl { get; set; }
    /// <summary>
    /// 角色id
    /// </summary>
    public string RoleId { get; set; }
    /// <summary>
    /// 角色
    /// </summary>
    public Role Role { get; set; }
    /// <summary>
    /// 真实姓名
    /// </summary>
    public string RealName { get; set; }
    /// <summary>
    /// 手机号码
    /// </summary>
    public string PhoneNumber { get; set; }
    /// <summary>
    /// 身份证
    /// </summary>
    public string IdNumber { get; set; }
    /// <summary>
    /// 录入时间
    /// </summary>
    public DateTime? CreatedTime { get; set; }

    /// <summary>
    /// 初始化
    /// </summary>
    public User() { }
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="userId">用户id</param>
    public User(string userId)
    {
        this.UserId = userId;
    }
}
