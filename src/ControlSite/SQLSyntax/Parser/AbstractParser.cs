using System;
using System.Collections.Generic;
using System.Text;
using DistDBMS.ControlSite.SQLSyntax.Object;

namespace DistDBMS.ControlSite.SQLSyntax.Parser
{
    abstract class AbstractParser
    {
        /// <summary>
        /// 上一个Parse操作后获得的结构
        /// </summary>
        public abstract object LastResult { get; }

        /// <summary>
        /// 上一个错误
        /// </summary>
        public SqlSyntaxError LastError { get { return error; } }
        protected SqlSyntaxError error;

        /// <summary>
        /// 解析Sql语句
        /// </summary>
        /// <param name="sql">Sql语句</param>
        /// <returns>true,如果解析正确;false,如果解析错误</returns>
        /// <remarks>如果true,对应的结构可从LastResult中获得</remarks>
        public abstract bool Parse(string sql);

        /// <summary>
        /// 就这个结构，填写一致性信息
        /// </summary>
        /// <returns>如果解析正确,返回true</returns>
        public abstract bool FillLocalConsistency();
        
        public AbstractParser() 
        {
            error = new SqlSyntaxError();
        }
    }
}
