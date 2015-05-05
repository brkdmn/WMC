using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Diagnostics;
using System.Transactions;

namespace WMC.Services.Web.API.Classes
{
    public class ObjectCtxManager : IDisposable
    {
        // Context creation call bazında 
        // Bu contextte transaction yokken transactionı ilk açan root olacak. 
        private bool isDisposed;

        public void Dispose()
        {
            if (!isDisposed)
            {
                contextLocalTransaction.Dispose();
                contextLocalConnection.Dispose();
                efContext.Dispose();
                isDisposed = true;

                System.Diagnostics.Debug.WriteLine("ObjectCtxManager disposed");
            }
        }

        public DbContext EFContext
        {
            get
            { return efContext; }
        }

        private DbContext efContext;
        private DbTransaction contextLocalTransaction;
        private DbConnection contextLocalConnection;

        public ObjectCtxManager() { }


        public void SetObjectContext(DbContext objCtx)
        {
            Debug.Assert(EFContext == null, "EF Context must be null");
            efContext = objCtx;
        }
        public TransactionScope GetTransaction(
          TransactionScopeOption option = TransactionScopeOption.RequiresNew)
        {
            return new TransactionScope(
                TransactionScopeOption.RequiresNew,
                new TransactionOptions
                {
                    IsolationLevel = IsolationLevel.ReadCommitted,
                    Timeout = TimeSpan.FromMinutes(10)
                });
        }
        public bool BeginTransaction()
        {
            if (contextLocalTransaction == null)
            {
                contextLocalConnection = EFContext.Database.Connection;
                if (contextLocalConnection.State == System.Data.ConnectionState.Closed)
                    contextLocalConnection.Open();

                contextLocalTransaction = contextLocalConnection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);

                // Distributed tx support
                if (System.Transactions.Transaction.Current != null)
                {
                    contextLocalConnection.EnlistTransaction(System.Transactions.Transaction.Current);
                }

                return true;
            }
            return false;
        }
        public void CommitTransaction()
        {
            contextLocalTransaction.Commit();
            contextLocalConnection.Close();
        }

        public void RollbackTransaction()
        {
            contextLocalTransaction.Rollback();
            contextLocalConnection.Close();
        }


    }

}
