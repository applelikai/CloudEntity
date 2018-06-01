using CloudEntity.Mapping;
using CloudEntity.Mapping.Common;
using CloudEntity.Test.Entities;

namespace CloudEntity.Test.Mappers
{
    /// <summary>
    /// 会员类型的Mapper类
    /// </summary>
    internal class CategoryMapper : TableMapper<Category>
    {
        /// <summary>
        /// 获取Mapping的Table信息
        /// </summary>
        /// <returns>获取Table信息</returns>
        protected override ITableHeader GetHeader()
        {
            return base.GetHeader("Categories", "memberSys");
        }
        /// <summary>
        /// 设置属性映射
        /// </summary>
        /// <param name="setter">设置器</param>
        protected override void SetColumnMappers(IColumnMapSetter<Category> setter)
        {
            setter.Map(c => c.CategoryId, ColumnAction.PrimaryAndIdentity, "CatId");
            setter.Map(c => c.CategoryName, ColumnAction.Insert, "CatName").DataType(length: 25);
        }
    }
}
