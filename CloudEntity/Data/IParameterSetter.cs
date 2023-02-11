namespace CloudEntity.Data
{
    /// <summary>
    /// Sql参数设置接口
    /// </summary>
    public interface IParameterSetter
    {
        /// <summary>
        /// 获取参数名为此名称开头的次数
        /// </summary>
        /// <param name="name">准参数名</param>
        /// <returns>此名称开头的次数</returns>
        int GetStartWithTimes(string name);
        /// <summary>
        /// 获取新建的sql参数名
        /// </summary>
        /// <param name="name">准参数名（若此参数名已使用则添加数字）</param>
        /// <returns>sql参数名</returns>
        string GetParameterName(string name);
        /// <summary>
        /// 添加sql参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="value">参数值</param>
        void AddSqlParameter(string name, object value);
    }
}