
namespace Han.DbLight
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.Common;
    using System.Linq;
    using System.Text;

    using IsolationLevel = System.Data.IsolationLevel;
    /// <summary>
    /// 非分布式事务
    /// http://www.cnblogs.com/artech/archive/2012/01/05/custom-transaction-scope.html
    /// </summary>
    public class TransactionScope: IDisposable
    {
        private Transaction transaction = Transaction.Current;
        private DbConnection connection;
        public bool Completed { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="querySession"></param>
        /// <param name="isolationLevel"></param>
        public TransactionScope(IQuerySession querySession,IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            if (null == transaction)
            {
                connection= querySession.CreateConnection();
                connection.Open();
                DbTransaction dbTransaction = connection.BeginTransaction(isolationLevel);
                Transaction.Current = new CommittableTransaction(dbTransaction);
            }
            else
            {
                Transaction.Current = transaction.DependentClone();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="querySession"></param>
        /// <param name="isolationLevel"></param>
        public TransactionScope(DbConnection conn, IsolationLevel isolationLevel = IsolationLevel.Unspecified)
        {
            if (null == transaction)
            {
                connection = conn;
                connection.Open();
                DbTransaction dbTransaction = connection.BeginTransaction(isolationLevel);
                Transaction.Current = new CommittableTransaction(dbTransaction);
            }
            else
            {
                Transaction.Current = transaction.DependentClone();
            }
        }

        public void Complete()
        {
            this.Completed = true;
        }
        public void Dispose()
        {
            Transaction current = Transaction.Current;
            Transaction.Current = transaction;
            if (!this.Completed)
            {
                try
                {
                    current.Rollback();
                }
                catch (Exception ex)
                {
                     #if DEBUG
                    Log.Logger.LogException(ex);
                    #endif
                }
              finally
                {
                    connection.Close();
                }
               
            }
            CommittableTransaction committableTransaction = current as CommittableTransaction;
            if (null != committableTransaction)
            {
                if (this.Completed)
                {
                    try
                    {
                        committableTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                         #if DEBUG
                        Log.Logger.LogException(ex);
                        #endif

                    }
                    finally
                    {
                        connection.Close();
                    }
                   
                   
                }
                committableTransaction.Dispose();
            }
        }

      
}
}
