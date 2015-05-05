using System.Configuration;
using System.Data.Entity;
using ServerCache;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Transactions;

namespace ServerInfra.Classes
{
    public class BusinessBase : IBusinessBase
    {

        private Dictionary<string, ObjectCtxManager> objectContexts;

        protected bool isRoot;

        protected BusinessBase()
        {


        }

        // connectionName key'ine sahip bir entry app.config connectionstrings grubunda olacak. 
        protected ObjectCtxManager GetObjectContext<T>(string connectionName) where T : DbContext
        {
            var connStr = ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
            var ctxManager = new ObjectCtxManager();
            ctxManager.SetObjectContext((T)Activator.CreateInstance(typeof(T), new object[] { connStr })); 
            return ctxManager;
        }


        protected TransactionScope CreateTransactionScope(TransactionScopeOption transactionScope = TransactionScopeOption.Required)
        {
            return TransactionUtils.CreateTransactionScope(transactionScope);
        }


        protected T CreateBSObject<T>() where T : BusinessBase
        {
            T oBusinessObject = (T)Activator.CreateInstance(typeof(T));
            oBusinessObject.objectContexts = objectContexts;
            return oBusinessObject;
        }



        #region IBusinessBase Members

        public List<T> CacheHelper<T>() where T : class
        {
            string defaultConnectionName = objectContexts.Keys.ToList()[0];
            return this.CacheHelperWithConnection<T>(defaultConnectionName);
        }

        public List<T> CacheHelperWithConnection<T>(string connectionName) where T : class
        {

            //ObjectSet<T> dest = objectContexts[connectionName].EFContext.CreateObjectSet<T>();
            DbSet<T> dest = this.GetObjectContext<DbContext>(connectionName).EFContext.Set<T>();
            return dest.ToList();
        }

        public List<T> GetCacheItem<T>(string key) where T : class
        {
            return (List<T>)CacheProvider.Provider.GetCacheItem<T>(key);
        }



        #endregion
        public void RefreshCacheItem(string key)
        {
            CacheProvider.Provider.RefreshCacheItem(key);
        }

        public long GetCacheVersion()
        {
            return CacheProvider.Provider.GetCacheVersion();
        }

        public List<object> GetCacheDiff(long inTs)
        {
            return CacheProvider.Provider.GetCacheDiff(inTs);
        }
    }
}
