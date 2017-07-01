using CloudEntity.Mapping;

namespace CloudEntity.Internal.Mapping
{
    /// <summary>
    /// 表的基础数据
    /// </summary>
    internal class TableHeader : ITableHeader
    {
        /// <summary>
        /// 线程锁
        /// </summary>
        private object locker;
        /// <summary>
        /// 表的全名
        /// </summary>
        private string tableFullName;

        /// <summary>
        /// 架构名
        /// </summary>
        public string SchemaName { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 表的全名
        /// </summary>
        public string TableFullName
        {
            get
            {
                Start:
                //若tableFullName不为空，直接返回
                if (!string.IsNullOrEmpty(this.tableFullName))
                    return this.tableFullName;
                //进入临界区(单线程进入)
                lock (this.locker)
                {
                    //若tableFullName不为空,回到Start;
                    if (!string.IsNullOrEmpty(this.tableFullName))
                        goto Start;
                    //若为空则获取
                    if (string.IsNullOrEmpty(this.SchemaName))
                        this.tableFullName = this.TableName;
                    else
                        this.tableFullName = string.Format("{0}.{1}", this.SchemaName, this.TableName);
                    //回到Start
                    goto Start;
                }
            }
        }
        /// <summary>
        /// 表的别名(sql语句中使用)
        /// </summary>
        public string TableAlias { get; set; }

        /// <summary>
        /// 创建TableHeader
        /// </summary>
        public TableHeader()
        {
            this.locker = new object();
        }
    }
}
