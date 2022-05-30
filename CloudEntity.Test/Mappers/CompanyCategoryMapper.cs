using CloudEntity.Mapping;
using CloudEntity.Mapping.Common;
using CloudEntity.Test.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudEntity.Test.Mappers
{
    /// <summary>
    /// 公司分类的Mapper类
    /// </summary>
    internal class CompanyCategoryMapper : TableMapper<CompanyCategory>
    {
        /// <summary>
        /// 获取Mapping的Table信息
        /// </summary>
        /// <returns>获取Table信息</returns>
        protected override ITableHeader GetHeader()
        {
            return base.GetHeader("Inc_CompanyCategories");
        }
        /// <summary>
        /// 设置属性映射
        /// </summary>
        /// <param name="setter">设置器</param>
        protected override void SetColumnMappers(IColumnMapSetter<CompanyCategory> setter)
        {
            setter.Map(c => c.CategoryId, ColumnAction.PrimaryAndInsert);
        }
    }
}
