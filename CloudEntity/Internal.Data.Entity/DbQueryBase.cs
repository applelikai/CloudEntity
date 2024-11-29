using CloudEntity.CommandTrees;
using CloudEntity.Data;
using CloudEntity.Data.Entity;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CloudEntity.Internal.Data.Entity
{
    /// <summary>
    /// 数据查询基础类
    /// [作者：Apple_Li 李凯 15150598493]
    /// </summary>
    internal class DbQueryBase : IDbBase, IParameterSetter
    {
        /// <summary>
        /// sql表达式节点集合
        /// </summary>
        private readonly IList<INodeBuilder> _nodebuilders;
        /// <summary>
        /// sql参数集合
        /// </summary>
        private readonly IList<IDbDataParameter> _sqlParameters;

        /// <summary>
        /// SQL命令工厂
        /// </summary>
        protected ICommandFactory CommandFactory { get; private set; }
        /// <summary>
        /// 操作数据库的DbHelper
        /// </summary>
        protected IDbHelper DbHelper { get; private set; }

        /// <summary>
        /// sql表达式节点集合
        /// </summary>
        public IEnumerable<INodeBuilder> NodeBuilders
        {
            get { return _nodebuilders; }
        }
        /// <summary>
        /// sql参数集合
        /// </summary>
        public IEnumerable<IDbDataParameter> Parameters
        {
            get { return _sqlParameters; }
        }

        /// <summary>
        /// 移除为某父节点类型的sql表达式节点
        /// </summary>
        /// <param name="parentType">父节点类型</param>
        protected void RemoveNodeBuilders(SqlType parentType)
        {
            // 为避免foreach遍历列表时执行删除会出现异常 使用for循环遍历，为了确保删除干净，从后到前执行删除
            for (int i = _nodebuilders.Count - 1; i >= 0; i --)
            {
                // 若当前sql表达式节点满足条件
                if (_nodebuilders[i].ParentNodeType == parentType)
                {
                    // 则删除当前下标的sql表达式节点
                    _nodebuilders.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 创建操作数据库的基础对象
        /// </summary>
        /// <param name="commandFactory">SQL命令工厂</param>
        /// <param name="dbHelper">操作数据库的DbHelper</param>
        public DbQueryBase(ICommandFactory commandFactory, IDbHelper dbHelper)
        {
            // 初始化
            _nodebuilders = new List<INodeBuilder>();
            _sqlParameters = new List<IDbDataParameter>();
            // 赋值
            this.CommandFactory = commandFactory;
            this.DbHelper = dbHelper;
        }
        /// <summary>
        /// 获取参数名为此名称开头的次数
        /// </summary>
        /// <param name="name">准参数名</param>
        /// <returns>此名称开头的次数</returns>
        public int GetStartWithTimes(string name)
        {
            return _sqlParameters.Count(p => p.ParameterName.StartsWith(name));
        }
        /// <summary>
        /// 获取新建的sql参数名
        /// </summary>
        /// <param name="name">准参数名（若此参数名已使用则添加数字）</param>
        /// <returns>sql参数名</returns>
        public string GetParameterName(string name)
        {
            // 获取此名称开头的次数
            int times = _sqlParameters.Count(p => p.ParameterName.StartsWith(name));
            // 获取新参数名（名称 + 开头次数）
            return $"{name}{times.ToString()}";
        }
        /// <summary>
        /// 添加sql表达式节点
        /// </summary>
        /// <param name="nodeBuilder">sql表达式节点</param>
        public void AddNodeBuilder(INodeBuilder nodeBuilder)
        {
            _nodebuilders.Add(nodeBuilder);
        }
        /// <summary>
        /// 添加sql表达式节点列表
        /// </summary>
        /// <param name="nodeBuilders">sql表达式节点列表</param>
        public void AddNodeBuilders(IEnumerable<INodeBuilder> nodeBuilders)
        {
            foreach (INodeBuilder nodeBuilder in nodeBuilders)
                _nodebuilders.Add(nodeBuilder);
        }
        /// <summary>
        /// 添加sql参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="value">参数值</param>
        public void AddSqlParameter(string name, object value)
        {
            // 若包含同名参数，退出
            if (_sqlParameters.Any(p => p.ParameterName.Equals(name)))
                return;
            // 创建sql参数
            IDbDataParameter sqlParameter = this.DbHelper.CreateParameter(name, value);
            // 添加sql参数
            _sqlParameters.Add(sqlParameter);
        }
        /// <summary>
        /// 添加sql参数
        /// </summary>
        /// <param name="sqlParameter">sql参数</param>
        public void AddSqlParameter(IDbDataParameter sqlParameter)
        {
            // 若包含同名参数，退出
            if (_sqlParameters.Any(p => p.ParameterName.Equals(sqlParameter.ParameterName)))
                return;
            // 添加sql参数
            _sqlParameters.Add(sqlParameter);
        }
        /// <summary>
        /// 添加sql参数列表
        /// </summary>
        /// <param name="sqlParameters">sql参数列表</param>
        public void AddSqlParameters(IEnumerable<IDbDataParameter> sqlParameters)
        {
            // 遍历sql参数
            foreach (IDbDataParameter sqlParameter in sqlParameters)
            {
                // 若包含同名参数，跳过
                if (_sqlParameters.Any(p => p.ParameterName.Equals(sqlParameter.ParameterName)))
                    continue;
                // 添加sql参数
                _sqlParameters.Add(sqlParameter);
            }
        }
    }
}
