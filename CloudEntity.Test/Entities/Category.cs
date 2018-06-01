namespace CloudEntity.Test.Entities
{
    /// <summary>
    /// 会员类型
    /// </summary>
    public class Category
    {
        /// <summary>
        /// 会员类型编号
        /// </summary>
        public int? CategoryId { get; set; }
        /// <summary>
        /// 会员类型描述
        /// </summary>
        public string CategoryName { get; set; }

        public Category() { }
        public Category(string categoryName) 
        {
            this.CategoryName = categoryName;
        }
    }
}
