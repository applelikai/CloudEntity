using System;
using System.Collections.Generic;
using System.Text;

namespace CloudEntity.Test.Models
{
    public class User
    {
        /// <summary>
        /// 会员编号
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 会员姓名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 用户类型
        /// </summary>
        public string CategoryName { get; set; }
    }
}
