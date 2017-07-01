using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CloudEntity.CommandTrees.Commom
{
    /// <summary>
    /// sql节点集
    /// </summary>
    public class BuilderCollection : IBuilderCollection
    {
        private string titleLeftSpace;              //标题左空格
        private string bodyLeftSpace;               //主体左空格
        private string bodyRightSpace;              //主体右空格
        private string lastRightSpace;              //主体尾空格
        private IList<ISqlBuilder> sqlBuilders;     //sql节点集合

        /// <summary>
        /// sql节点集合
        /// </summary>
        private IList<ISqlBuilder> SqlBuilders
        {
            get { return this.sqlBuilders ?? (this.sqlBuilders = new List<ISqlBuilder>()); }
        }
        /// <summary>
        /// 节点数量
        /// </summary>
        public int Count
        {
            get { return this.SqlBuilders.Count; }
        }
        /// <summary>
        /// 标题左空格
        /// </summary>
        public string TitleLeftSpace
        {
            set { this.titleLeftSpace = value; }
        }
        /// <summary>
        /// 主体左空格
        /// </summary>
        public string BodyLeftSpace
        {
            set { this.bodyLeftSpace = value; }
        }
        /// <summary>
        /// 主体右空格
        /// </summary>
        public string BodyRightSpace
        {
            set { this.bodyRightSpace = value; }
        }
        /// <summary>
        /// 主体尾空格
        /// </summary>
        public string LastRightSpace
        {
            set { this.lastRightSpace = value; }
        }

        /// <summary>
        /// 创建sql节点集
        /// </summary>
        public BuilderCollection() { }
        /// <summary>
        /// 创建sql节点集
        /// </summary>
        /// <param name="titleLeftSpace">标题左空格</param>
        /// <param name="bodyLeftSpace">主体左空格</param>
        /// <param name="bodyRightSpace">主体右空格</param>
        /// <param name="lastRightSpace">主体尾空格</param>
        public BuilderCollection(string titleLeftSpace, string bodyLeftSpace, string bodyRightSpace, string lastRightSpace)
        {
            this.titleLeftSpace = titleLeftSpace;   //标题左空格
            this.bodyLeftSpace = bodyLeftSpace;     //主体左空格
            this.bodyRightSpace = bodyRightSpace;   //主体右空格
            this.lastRightSpace = lastRightSpace;   //主体尾空格
        }
        /// <summary>
        /// 添加新的sql子节点
        /// </summary>
        /// <param name="sqlBuilder">sql子节点</param>
        public void Append(ISqlBuilder sqlBuilder)
        {
            this.SqlBuilders.Add(sqlBuilder);
        }
        /// <summary>
        /// 按索引插入新的Sql节点
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="sqlBuilder">Sql节点</param>
        public void Insert(int index, ISqlBuilder sqlBuilder)
        {
            this.SqlBuilders.Insert(index, sqlBuilder);
        }
        /// <summary>
        /// 构建sql
        /// </summary>
        /// <param name="commandText">待构建的sql</param>
        public virtual void Build(StringBuilder commandText)
        {
            for (int i = 0; i < this.SqlBuilders.Count; i++)
            {
                commandText.Append(i == 0 ? this.titleLeftSpace : this.bodyLeftSpace);
                this.SqlBuilders[i].Build(commandText);
                commandText.Append((i + 1) == this.SqlBuilders.Count ? this.lastRightSpace : this.bodyRightSpace);
            }
        }

        /// <summary>
        /// 获取sqlBuilder迭代器
        /// </summary>
        /// <returns>sqlBuilder迭代器</returns>
        public IEnumerator<ISqlBuilder> GetEnumerator()
        {
            foreach (ISqlBuilder sqlBuilder in this.SqlBuilders)
                yield return sqlBuilder;
        }
        /// <summary>
        /// 获取ISqlBuilder迭代器
        /// </summary>
        /// <returns>ISqlBuilder迭代器</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
