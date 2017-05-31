
using System.Security;
using System.Security.Permissions;

namespace Han.DbLight.Oracle
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using System.Linq.Expressions;

    using Han.Log;
    using Han.DbLight.Mapper;
    using Han.DbLight.TableMetadata;
    using global::Oracle.ManagedDataAccess.Client;
    using Infrastructure;

    /// <summary>
    /// 通过企业库实现的oracleDatabase
    /// ExecuteSqlString 不支持 Transcop(由于ExecuteSqlString内部使用企业库方法，无法使用command执行，也许可以通过ParameterMapperAdapter的command实现事物支持）
    /// 
    /// </summary>
    public class OracleQuerySession : IQuerySession
    {
        #region Fields

        protected DbProviderFactory dbProviderFactory;

        protected DatabaseInfo databaseInfo;

        protected DbParameterCreater dbParameterCreater;

        protected SqlLog sqlLog;

        #endregion

        #region Constructors and Destructors

        public OracleQuerySession(DatabaseInfo databaseInfo)
        {
            this.databaseInfo = databaseInfo;

            this.dbParameterCreater = new DbParameterCreater();
            #region"连接字符串解密"


            //苗建龙 修改
            //var connectionStrings = ConfigurationManager.ConnectionStrings["shenHeDb"].ConnectionString;
            var connectionStrings = ConfigHelper.Instance.Get(databaseInfo.ConnectionStringName)
                ?? ConfigHelper.Instance.GetSection("DBConnectionString")[databaseInfo.ConnectionStringName];
            //var connectionStrings = ConfigurationManager.AppSettings["shenHeDb"].ToString();
            
            var connStr = new Infrastructure.SymmetricMethod().Decrypto(connectionStrings);
            if (!string.IsNullOrEmpty(connStr))
            {
                connectionString = connStr;
            }
            else
            {
                connectionString = connectionStrings;
            }

            //dbProviderFactory = DbProviderFactories.GetFactory("Oracle.ManagedDataAccess.Client");
            #endregion

            this.sqlLog = new SqlLog(databaseInfo);
        }

        #endregion


        private string connectionString;
        #region Public Properties

        public string ConnectionString
        {
            get
            {
                return connectionString;
            }
        }

        public DatabaseInfo DatabaseInfo
        {
            get
            {
                return this.databaseInfo;
            }
        }

        public DbProviderFactory DbProviderFactory
        {
            get
            {
                return this.dbProviderFactory;
            }
        }
        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>

        //private void TestODp()
        //{
        //    using (OracleConnection oracleCon = new OracleConnection(""))
        //    {
        //        oracleCon.Open();
        //        using (OracleCommand oracleCom = oracleCon.CreateCommand())
        //        {
        //            oracleCom.CommandType = CommandType.Text;
        //            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //            {
        //                //从 DataSet 逐条检索数据并且存储到 Oracle 数据表 Employee 
        //                string sqlStr = String.Format(
        //                    "INSERT INTO EMPLOYEE(EMPID,EMPNAME,EMPSEX,EMPSALARY) " +
        //                    "VALUES({0},'{1}','{2}',{3})",
        //                    ds.Tables[0].Rows[i][0],
        //                    ds.Tables[0].Rows[i][1],
        //                    ds.Tables[0].Rows[i][2],
        //                    ds.Tables[0].Rows[i][3]);
        //                oracleCom.CommandText = sqlStr;

        //                oracleCom.ExecuteNonQuery();
        //            }
        //        }
        //    }
        //}
        public int ExecuteNonQuery(string commandText, params object[] parameterValues)
        {
            //commandText = commandText.Replace(":", databaseInfo.SqlDialect.DbParameterConstant);
            IDictionary<string, object> dbp = this.databaseInfo.DbTypeConverter.ToDicParams(parameterValues);
            int result = ExecuteNonQuery(commandText, dbp);
            return result;
        }

        public int ExecuteNonQuery(string commandText, IDictionary<string, object> dbParameters)
        {
            // commandText = commandText.Replace(":", databaseInfo.SqlDialect.DbParameterConstant);
            DbCommand command = this.CreateCommand(commandText);
            this.databaseInfo.DbTypeConverter.AddParams(command, dbParameters);
            Logger.Log(Level.Debug, this.sqlLog.GetLogSql(commandText, dbParameters));
            if (null != Transaction.Current)
            {
                command.Connection = Transaction.Current.DbTransactionWrapper.DbTransaction.Connection;
                command.Transaction = Transaction.Current.DbTransactionWrapper.DbTransaction;
                return command.ExecuteNonQuery();
            }
            else
            {
                //由企业库处理分布式事务
                using (var conn = OpenConnection())
                {
                    command.Connection = conn;
                    return command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public IDataReader ExecuteReader(string commandText)
        {
            // 事务没测试
            DbCommand command = this.CreateCommand(commandText);
            Logger.Log(Level.Debug, commandText);
            if (null != Transaction.Current)
            {
                command.Connection = Transaction.Current.DbTransactionWrapper.DbTransaction.Connection;
                command.Transaction = Transaction.Current.DbTransactionWrapper.DbTransaction;
                return command.ExecuteReader();
            }
            else
            {
                //由企业库处理分布式事务
                using (var conn = OpenConnection())
                {
                    command.Connection = conn;
                    return command.ExecuteReader();
                }
            }
        }

        public T ExecuteScalar<T>(string commandText, params object[] parameterValues)
        {
            // commandText = commandText.Replace(":", databaseInfo.SqlDialect.DbParameterConstant);
            IDictionary<string, object> dbp = this.databaseInfo.DbTypeConverter.ToDicParams(parameterValues);
            return ExecuteScalar<T>(commandText, dbp);
        }

        public T ExecuteScalar<T>(string commandText, IDictionary<string, object> dbParameters)
        {
            DbCommand command = this.CreateCommand(commandText);
            this.databaseInfo.DbTypeConverter.AddParams(command, dbParameters);
            Logger.Log(Level.Debug, this.sqlLog.GetLogSql(commandText, dbParameters));
            object result = null;
            if (null != Transaction.Current)
            {
                command.Connection = Transaction.Current.DbTransactionWrapper.DbTransaction.Connection;
                command.Transaction = Transaction.Current.DbTransactionWrapper.DbTransaction;
                result = command.ExecuteScalar();
            }
            else
            {
                //由企业库处理分布式事务
                using (var conn = OpenConnection())
                {
                    command.Connection = conn;
                    result = command.ExecuteScalar();
                }

            }
            return DbHelper.ConvertFromDbVal<T>(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        public IEnumerable<TResult> ExecuteSqlString<TResult>(string commandText, params object[] parameterValues)
            where TResult : new()
        {
            // commandText = commandText.Replace(":", databaseInfo.SqlDialect.DbParameterConstant);
            Mapper.IRowMapper<TResult> rowMapper = this.GetRowMapper<TResult>();
            return this.ExecuteSqlString(commandText, rowMapper, parameterValues);
        }

        public IEnumerable<TResult> ExecuteSqlString<TResult>(string commandText, IDictionary<string, object> dbParameters)
            where TResult : new()
        {
            //commandText = commandText.Replace(":", databaseInfo.SqlDialect.DbParameterConstant);
            Mapper.IRowMapper<TResult> rowMapper = this.GetRowMapper<TResult>();
            return this.ExecuteSqlString(commandText, rowMapper, dbParameters);
        }

        public IEnumerable<TResult> ExecuteSqlString<TResult>(string commandText, Mapper.IRowMapper<TResult> rowMapper, params object[] parameterValues)
        {
            IDictionary<string, object> dbp = this.databaseInfo.DbTypeConverter.ToDicParams(parameterValues);

            return ExecuteSqlString(commandText, rowMapper, dbp);

        }
        public IEnumerable<TResult> ExecuteSqlString<TResult>(string commandText, Mapper.IRowMapper<TResult> rowMapper, IDictionary<string, object> dbParameters)
        {
            rowMapper.TablesInfo = this.DatabaseInfo.TablesInfo;
            DbCommand command = this.CreateCommand(commandText);
            if (dbParameters != null && dbParameters.Count != 0)
            {
                //IDictionary<string, object> dbParameters = this.databaseInfo.DbTypeConverter.ToDicParams(parameterValues);
                this.databaseInfo.DbTypeConverter.AddParams(command, dbParameters);
                Logger.Log(Level.Debug, this.sqlLog.GetLogSql(commandText, dbParameters));
            }
            else
            {
                Logger.Log(Level.Debug, commandText);
            }
            List<TResult> data = new List<TResult>();
            if (null != Transaction.Current)
            {
                command.Connection = Transaction.Current.DbTransactionWrapper.DbTransaction.Connection;
                command.Transaction = Transaction.Current.DbTransactionWrapper.DbTransaction;
                using (var reader = command.ExecuteReader() as OracleDataReader)
                {
                    //reader.FetchSize = reader.RowSize * 2000;
                    if (reader.HasRows)
                    {
                        var r = reader.Cast<IDataRecord>();
                        foreach (var dataRecord in r)
                        {
                            var t = rowMapper.MapRow(dataRecord);
                            data.Add(t);
                        }
                    }
                }
            }
            else
            {
                //由企业库处理分布式事务
                using (var conn = OpenConnection())
                {
                    command.Connection = conn;
                    using (var reader = command.ExecuteReader() as OracleDataReader)
                    {
                        Log.Logger.Log(Level.Debug, reader.RowSize.ToString());
                        Log.Logger.Log(Level.Debug, reader.FetchSize.ToString());
                        //reader.FetchSize = reader.RowSize * 2000;
                        if (reader.HasRows)
                        {
                            var r = reader.Cast<IDataRecord>();
                            foreach (var dataRecord in r)
                            {
                                var t = rowMapper.MapRow(dataRecord);
                                data.Add(t);
                            }

                        }
                    }
                }
            }
            return data;
        }

        /// <summary>
        /// 创建Command,connection 未指定
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private OracleCommand CreateCommand(string query)
        {
            var oraclCommand = new OracleConnection(connectionString).CreateCommand();

            oraclCommand.CommandText = query;
            oraclCommand.BindByName = true;

            return oraclCommand;
        }
        public DbConnection CreateConnection()
        {
            return new OracleConnection(connectionString);
        }
        public OracleConnection OpenConnection()
        {
            var conn = new OracleConnection(connectionString);
            conn.Open();

            return conn as OracleConnection;
        }

        #endregion

        #region Methods

        private Mapper.IRowMapper<TResult> GetRowMapper<TResult>() where TResult : new()
        {
            Type type = typeof(TResult);
            Mapper.IRowMapper<TResult> rowMapper;
            TableAttribute tableAttr = type.GetCustomAttributes(true).OfType<TableAttribute>().FirstOrDefault();
            if (tableAttr == null)
            {
                rowMapper = new PropertyEqualColumnMapper<TResult>();
            }
            else
            {
                rowMapper = new ColumnAttributeMapper<TResult>(this.DatabaseInfo.TablesInfo);
            }
            return rowMapper;
        }

        #endregion
    }
}