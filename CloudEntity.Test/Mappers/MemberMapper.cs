using CloudEntity.Mapping;
using CloudEntity.Mapping.Common;
using CloudEntity.Test.Entities;

namespace CloudEntity.Test.Mappers
{
    /// <summary>
    /// Member的Mapper类
    /// </summary>
    internal class MemberMapper : TableMapper<Member>
    {
        /// <summary>
        /// 获取Mapping的Table信息
        /// </summary>
        /// <returns>获取Table信息</returns>
        protected override ITableHeader GetHeader()
        {
            return base.GetHeader("Members", "memberSys");
        }
        /// <summary>
        /// 设置属性映射
        /// </summary>
        /// <param name="setter">设置器</param>
        protected override void SetColumnMappers(IColumnMapSetter<Member> setter)
        {
            setter.Map(m => m.MemberId, ColumnAction.PrimaryAndIdentity);
            setter.Map(m => m.MemberName, length: 25);
            setter.Map(m => m.Sex, ColumnAction.Insert);
            setter.Map(m => m.AddTime, ColumnAction.Default);
            setter.Map(m => m.Points, ColumnAction.EditAndDefault);
            setter.Map(m => m.CategoryId, ColumnAction.Insert, "CatId");
        }
    }
}
