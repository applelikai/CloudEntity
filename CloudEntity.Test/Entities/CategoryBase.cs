using System;
using System.Collections.Generic;
using System.Text;

namespace CloudEntity.Test.Entities
{
    /// <summary>
    /// 分类
    /// </summary>
    public abstract class CategoryBase
    {
        /// <summary>
        /// 分类id
        /// </summary>
        public string CategoryId { get; set; }
        /// <summary>
        /// 分类名称
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// 录入时间
        /// </summary>
        public DateTime? CreatedTime { get; set; }
    }
}
