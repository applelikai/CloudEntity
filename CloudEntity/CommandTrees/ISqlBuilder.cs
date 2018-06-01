using System.Text;

namespace CloudEntity.CommandTrees
{
    /// <summary>
    /// Sql节点
    /// Apple_Li 李凯
    /// </summary>
    public interface ISqlBuilder
    {
        /// <summary>
        /// 拼接sql语句
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        void Build(StringBuilder commandText);
    }
}
