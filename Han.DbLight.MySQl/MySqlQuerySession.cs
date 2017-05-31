/* ***********************************************
 * author :  韩奎奎
 * function: 
 * history:  created by 韩奎奎 2017/4/25 12:25:18
 * ***********************************************/

namespace Han.DbLight.MySQl
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Mapper;
    using Infrastructure;
    using MySql.Data.MySqlClient;
    using Log;
    using TableMetadata;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MySqlQuerySession : IQuerySession
    {

        protected DatabaseInfo databaseInfo;

        protected DbProviderFactory dbProviderFactory;

        protected DbParameterCreater dbParameterCreater;

        protected SqlLog sqlLog;
        private string connectionString;

        public string ConnectoinString
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

        public MySqlQuerySession(DatabaseInfo databaseInfo)
        {
            this.databaseInfo = databaseInfo;

            this.dbParameterCreater = new DbParameterCreater();

            connectionString = ConfigHelper.Instance.Get(databaseInfo.ConnectionStringName) ??
                ConfigHelper.Instance.GetSection("DBConnectionString")[databaseInfo.ConnectionStringName];

            this.sqlLog = new SqlLog(databaseInfo);
        }

        public DbConnection CreateConnection()
        {
            return new MySqlConnection(connectionString);
        }

        public int ExecuteNonQuery(string commandText, IDictionary<string, object> dbParams)
        {
            DbCommand command = this.CreateCommand(commandText);

            this.databaseInfo.DbTypeConverter.AddParam(command, dbParams);

            Logger.Log(Level.Debug, this.sqlLog.GetLogSql(commandText, dbParams));
            if (Transaction.Current != null)
            {
                command.Connection = Transaction.Current.DbTransactionWrapper.DbTransaction.Connection;
                command.Transaction = Transaction.Current.DbTransactionWrapper.DbTransaction;
                return command.ExecuteNonQuery();
            }
            else
            {
                using (var conn = this.OpenConnection())
                {
                    command.Connection = conn;
                    return command.ExecuteNonQuery();
                }
            }
        }

        public int ExecuteNonQuery(string commandText, params object[] parameterValues)
        {
            IDictionary<string, object> dbp = this.databaseInfo.DbTypeConverter.ToDicParams(parameterValues);

            return ExecuteNonQuery(commandText, dbp);
        }

        public IDataReader ExecuteReader(string commandText)
        {
            throw new NotImplementedException();
        }

        public T ExecuteScalar<T>(string commandText, params object[] parameterValues)
        {
            throw new NotImplementedException();
        }

        public T ExecuteScalar<T>(string commandText, IDictionary<string, object> dbParameters)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TResult> ExecuteSqlString<TResult>(string sqlString, IDictionary<string, object> dbParams) where TResult : new()
        {
            IRowMapper<TResult> rowMapper = this.GetRowMapper<TResult>();
            return this.ExecuteSqlString(sqlString, rowMapper, dbParams);

        }

        public IEnumerable<TResult> ExecuteSqlString<TResult>(string sqlString, params object[] parameterValues) where TResult : new()
        {
            IRowMapper<TResult> rowMapper = this.GetRowMapper<TResult>();
            return ExecuteSqlString(sqlString, rowMapper, parameterValues);
        }

        public IEnumerable<TResult> ExecuteSqlString<TResult>(string commandText, IRowMapper<TResult> rowMapper, IDictionary<string, object> dbParams)
        {
            rowMapper.TablesInfo = this.DatabaseInfo.TablesInfo;
            DbCommand command = this.CreateCommand(commandText);
            if (dbParams != null && dbParams.Count != 0)
            {
                this.databaseInfo.DbTypeConverter.AddParam(command, dbParams);
                Logger.Log(Level.Debug, this.sqlLog.GetLogSql(commandText, dbParams));
            }
            else
            {
                Logger.Log(Level.Debug, commandText);
            }

            List<TResult> result = new List<TResult>();
            if (Transaction.Current != null)
            {
                command.Connection = Transaction.Current.DbTransactionWrapper.DbTransaction.Connection;
                command.Transaction = Transaction.Current.DbTransactionWrapper.DbTransaction;
                using (var reader = command.ExecuteReader() as MySqlDataReader)
                {
                    if (reader.HasRows)
                    {
                        var record = reader.Cast<IDataRecord>();
                        foreach (var r in record)
                        {
                            var t = rowMapper.MapRow(r);
                            result.Add(t);
                        }
                    }
                }
            }
            else
            {
                using (var conn = OpenConnection())
                {
                    command.Connection = conn;
                    using (var reader = command.ExecuteReader() as MySqlDataReader)
                    {
                        //Logger.Log(Level.Debug, reader.RowSize.ToString());
                        //Logger.Log(Level.Debug, reader.FetchSize.ToString());
                        if (reader.HasRows)
                        {
                            var record = reader.Cast<IDataRecord>();
                            foreach (var r in record)
                            {
                                var t = rowMapper.MapRow(r);
                                result.Add(t);
                            }
                        }
                    }
                }
            }

            return result;
        }

        public IEnumerable<TResult> ExecuteSqlString<TResult>(string commandText, IRowMapper<TResult> rowMapper, params object[] parameterValues)
        {
            IDictionary<string, object> dbp = this.databaseInfo.DbTypeConverter.ToDicParams(parameterValues);

            return ExecuteSqlString(commandText, rowMapper, dbp);
        }

        public MySqlConnection OpenConnection()
        {
            var conn = new MySqlConnection(connectionString);
            conn.Open();

            return conn;
        }

        private MySqlCommand CreateCommand(string query)
        {
            var mySqlCommand = this.CreateConnection().CreateCommand();

            mySqlCommand.CommandText = query;
            // oralceCommand.bindByName = true;

            return mySqlCommand as MySqlCommand;
        }

        private IRowMapper<TResult> GetRowMapper<TResult>() where TResult : new()
        {
            Type type = typeof(TResult);
            IRowMapper<TResult> rowMapper;
            TableAttribute attributes = type.GetCustomAttributes(true).OfType<TableAttribute>().FirstOrDefault();

            if (attributes == null)
            {
                rowMapper = new PropertyEqualColumnMapper<TResult>();
            }
            else
            {
                rowMapper = new ColumnAttributeMapper<TResult>(this.DatabaseInfo.TablesInfo);
            }

            return rowMapper;

        }

    }
}
