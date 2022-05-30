using System;
using System.Text;

namespace CloudEntity.CommandTrees.Commom.SqlClient
{
    /// <summary>
    /// sql row_number函数节点生成类
    /// 李凯 Apple_Li
    /// </summary>
    internal class RowNumberBuilder : ISqlBuilder
    {
        /// <summary>
        /// 排序节点的子节点集合
        /// </summary>
        private ISqlBuilder[] _sortBuilders;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="sortBuilders">排序节点集合</param>
        public RowNumberBuilder(ISqlBuilder[] sortBuilders)
        {
            _sortBuilders = sortBuilders;
        }
        /// <summary>
        /// 拼接sql语句
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        public void Build(StringBuilder commandText)
        {
            //拼接ROW_NUMBER函数
            commandText.Append("ROW_NUMBER()");
            commandText.Append(" OVER(ORDER BY");
            //拼接排序列
            for (int i = 0; i < _sortBuilders.Length; i++)
            {
                commandText.Append(" ");
                _sortBuilders[i].Build(commandText);
                if (i + 1 < _sortBuilders.Length)
                    commandText.Append(',');
            }
            //拼接结尾
            commandText.Append(")");
            commandText.Append(" RowNumber");
        }
    }
}
