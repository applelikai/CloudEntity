using System;

namespace CloudEntity.Test.Entities
{
    public class Member
    {
        /// <summary>
        /// 会员编号
        /// </summary>
        public int MemberId { get; set; }
        /// <summary>
        /// 会员姓名
        /// </summary>
        public string MemberName { get; set; }
        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public int Sex { get; set; }
        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime AddTime { get; set; }
        /// <summary>
        /// 积分
        /// </summary>
        public int Points { get; set; }
        /// <summary>
        /// 会员类型编号
        /// </summary>
        public int CategoryId { get; set; }
        /// <summary>
        /// 会员类型
        /// </summary>
        public Category MemberCategory { get; set; }

        /// <summary>
        /// 创建Member对象
        /// </summary>
        public Member() { }
        /// <summary>
        /// 创建Member对象
        /// </summary>
        /// <param name="memberId">成员ID</param>
        public Member(int memberId)
        {
            this.MemberId = memberId;
        }
    }
}
