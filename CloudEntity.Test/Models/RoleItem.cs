using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudEntity.Test.Models
{
    /// <summary>
    /// 角色项类
    /// </summary>
    public class RoleItem
    {
        /// <summary>
        /// 角色id
        /// </summary>
        public string RoleId { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        public RoleItem() { }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="roleId">角色id</param>
        public RoleItem(string roleId)
        {
            this.RoleId = roleId;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="roleId">角色id</param>
        /// <param name="roleName">角色名称</param>
        public RoleItem(string roleId, string roleName)
        {
            this.RoleId = roleId;
            this.RoleName = roleName;
            this.Remark = roleName;
        }
    }
}
