

namespace Han.DbLight
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq.Expressions;

    using Han.DbLight.Mapper;

    /// <summary>
    ///轻量级数据访问接口,每个数据库操作创建新的conncecion
    /// </summary>
    public interface IQuerySession
    {
        #region Public Properties

        DatabaseInfo DatabaseInfo { get; }

        //DbCommand GetSqlStringCommand(string query);
        /// <summary>
        ///  Gets the DbProviderFactory used by the database instance.
        /// </summary>
        DbProviderFactory DbProviderFactory { get; }

        #endregion

        #region Public Methods and Operators

        DbConnection CreateConnection();

        //DataSet ExecuteDataSet(string commandText);

        //DataSet ExecuteDataSet(string commandText, IDictionary<string, object> dbParams);

        /// <summary>
        /// 执行sql。返回值尽量比不用。返回值依赖于数据库设置
        /// </summary>
        /// <param name="commandText">数据库执行的sql语句，如果使用参数化查询参数名为0.1,2... </param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        int ExecuteNonQuery(string commandText, params object[] parameterValues);

        int ExecuteNonQuery(string commandText, IDictionary<string, object> dbParams);

     

        /// <summary>
        ///         Executes the commandText interpreted as specified by the commandType and
        ///     returns an System.Data.IDataReader through which the result can be read.
        ///      It is the responsibility of the caller to close the connection and reader
        ///     when finished.
        /// </summary>
        
        /// <param name="commandText"></param>
        /// <returns></returns>
        IDataReader ExecuteReader(string commandText);

        T ExecuteScalar<T>(string commandText, IDictionary<string, object> dbParameters);

        T ExecuteScalar<T>(string commandText, params object[] parameterValues);

        /// <summary>
        /// 执行数据库操作，返回结果集
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="sqlString">参数化sql，参数名为0,1.2...</param>
        /// <param name="parameterValues">传给数据库的查询参数</param>
        /// <returns></returns>
        /// <remarks>
        /// 根据TResult推断使用的结果集映射，如果TResult包含TableAttribute使用ColumnAttributeMapper，否则使用PropertyAsColumnMapper
        /// </remarks>
        IEnumerable<TResult> ExecuteSqlString<TResult>(string sqlString, params object[] parameterValues) where TResult : new();

        /// <summary>
        /// 执行数据库操作，返回结果集
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="sqlString">sql语句，可包含查询参数</param>
        /// <param name="dbParams">传给数据库的查询参数，查询参数名称值对，使用DictionaryParameterMapper映射到数据库参数</param>
        /// <returns>返回结果</returns>
        /// <remarks>
        /// 根据TResult推断使用的结果集映射，如果TResult包含TableAttribute使用ColumnAttributeMapper，否则使用PropertyAsColumnMapper
        /// </remarks>
        IEnumerable<TResult> ExecuteSqlString<TResult>(string sqlString, IDictionary<string, object> dbParams) where TResult : new();

        /// <summary>
        /// 执行数据库操作，返回结果集
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="commandText">数据库执行的sql语句，如果使用参数化查询参数名为0.1,2... </param>
        /// <param name="rowMapper">数据库列到TResult的映射方式</param>
        /// <param name="parameterValues">查询参数值
        /// <remarks>如果不传，则执行非参数化查询，sqlstring不能包含查询参数；
        /// 如果不为空参数值依次替换sqlstring中的：1，：2等值,:为不同数据库的分隔符如， @ ms sql
        /// </remarks>
        /// </param>
        /// <returns></returns>
        /// <code>
        /// sql: where x.hisid = :0
        /// parameterValues:45
        /// </code>
        IEnumerable<TResult> ExecuteSqlString<TResult>(string commandText, IRowMapper<TResult> rowMapper, params object[] parameterValues);

        /// <summary> Execute the database operation synchronously, returning the System.Collections.Generic.IEnumerable&gt;TResult&lt;
        ///     sequence containing the resulting objects.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="commandText">参数化sql</param>
        /// <param name="rowMapper">行映射</param>
        /// <param name="dbParams">参数值,使用DictionaryParameterMapper映射<see cref="DictionaryParameterMapper"/></param>
        /// <returns></returns>
        IEnumerable<TResult> ExecuteSqlString<TResult>(string commandText, IRowMapper<TResult> rowMapper, IDictionary<string, object> dbParams);

        //List<TResult> ExecuteSqlStringInTran<TResult>(string commandText, Mapper.IRowMapper<TResult> rowMapper,

        //                                                     params object[] parameterValues);

        #endregion
    }
}