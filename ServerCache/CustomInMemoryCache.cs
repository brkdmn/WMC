using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Web;
using System.Web.Caching;
using System.Reflection;

namespace ServerCache
{
    internal class CustomInMemoryCache : ICacheProvider
    {
        private static CustomInMemoryCache mInstance = new CustomInMemoryCache();
        private const string MaxTsInServer = "MaxTsInServer";
        private static object lockObject = new object();

        public static CustomInMemoryCache GetProviderInstance
        {
            get { return mInstance; }
        }

        private static Hashtable ht;

        static CustomInMemoryCache()
        {
            InitMaxTS();
            try
            {
                ht = new Hashtable();
                foreach (CacheItem ci in CacheProvider.ProviderSettings.CacheItems)
                {
                    ht.Add(ci.CacheItemName, ci);
                    StaticGetCacheItem(ci.CacheItemName);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.EventLog.WriteEntry("CbeLight", ex.ToString(), System.Diagnostics.EventLogEntryType.Error  );
                throw ex;
            }
        }

        private static void InitMaxTS()
        {
            if (HttpRuntime.Cache[MaxTsInServer] == null)
            {
                System.Diagnostics.Debug.WriteLine("HttpRuntime.Cache(MaxTsInServer) initialized");
                HttpRuntime.Cache.Insert(MaxTsInServer,
                            0,
                            null,
                            DateTime.Now.AddYears(10),
                            System.Web.Caching.Cache.NoSlidingExpiration,
                            System.Web.Caching.CacheItemPriority.High,
                            null);
            }
        }

        private static Int64 SetMaxTs(IEnumerable cacheObjectList)
        {

            Int64 nMaxValue = 0;
            Int64 nValue = 0;
            byte[] tsBytes = null;
            foreach (object i in cacheObjectList)
            {
                tsBytes = (byte[])i.GetType().GetProperty("ts").GetValue(i, null);
                nValue = Convert.ToInt64(BitConverter.ToString(tsBytes).Replace("-", ""), 16);
                if ((nValue > nMaxValue))
                {
                    nMaxValue = nValue;
                }
            }
            return nMaxValue;
        }


        private static Int64 InsertCache(string key, CacheItem pCacheItem)
        {
            object ds = null;
            Int64 nMaxTs = 0;
            try
            {
                Type bsType = CacheProvider.ProviderSettings.ProjectBaseBusinessObject;
                object mProjectBaseClassInstance = bsType.InvokeMember(null, BindingFlags.CreateInstance, null, null, null);

                if (string.IsNullOrEmpty(pCacheItem.ConnectionName))
                {
                    MethodInfo method = bsType.GetMethod("CacheHelper");
                    MethodInfo generic = method.MakeGenericMethod(pCacheItem.EFObjectName);
                    ds = generic.Invoke(mProjectBaseClassInstance, null);
                }

                else
                {
                    MethodInfo method = bsType.GetMethod("CacheHelperWithConnection");
                    MethodInfo generic = method.MakeGenericMethod(pCacheItem.EFObjectName);
                    ds = generic.Invoke(mProjectBaseClassInstance, new object[] { pCacheItem.ConnectionName });
                }

                nMaxTs = SetMaxTs((IEnumerable)ds);
            }
            catch (TargetInvocationException tEx)
            {
                if (tEx.InnerException != null)
                {
                    throw new ApplicationException(tEx.InnerException.Message);
                }
                else
                {
                    throw new ApplicationException(tEx.Message);
                }
            }
            catch (ReflectionTypeLoadException rtex)
            {
                throw new ApplicationException(rtex.Message);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            // expirationInMinutes <= 0 sa does not expire .... 
            if (pCacheItem.MinutesToExpire <= 0)
            {
                HttpRuntime.Cache.Insert(key,
                    ds,
                    null,
                    System.Web.Caching.Cache.NoAbsoluteExpiration,
                    System.Web.Caching.Cache.NoSlidingExpiration,
                    System.Web.Caching.CacheItemPriority.High,
                    CacheItemRemoved);
            }
            else
            {
                HttpRuntime.Cache.Insert(key,
                    ds,
                    null,
                    DateTime.Now.AddMinutes(pCacheItem.MinutesToExpire),
                    System.Web.Caching.Cache.NoSlidingExpiration,
                    System.Web.Caching.CacheItemPriority.High,
                    CacheItemRemoved);
            }

            return nMaxTs;

        }
        public static void CacheItemRemoved(string key, object value, CacheItemRemovedReason removedReason)
        {
            System.Diagnostics.Debug.WriteLine(key + " dropped from cache");
            StaticGetCacheItem(key);
        }


        public static object StaticGetCacheItem(string key)
        {

            object ds = HttpRuntime.Cache[key];
            Int64 maxTs;
            if (ds != null)
            {
                System.Diagnostics.Debug.WriteLine("Cache hit " + key);
                return ds;
            }



            if ((ds == null) && (ht.ContainsKey(key)))
            {
                lock ((CacheItem)ht[key])
                {

                    ds = HttpRuntime.Cache[key];

                    if (ds != null)
                    {
                        System.Diagnostics.Debug.WriteLine("Cache hit " + key);
                        return ds;
                    }
                    System.Diagnostics.Debug.WriteLine("Cache miss " + key);
                    CacheItem ci = (CacheItem)ht[key];

                    maxTs = InsertCache(key, ci);
                    UpdateMaxTs(maxTs);
                    ds = HttpRuntime.Cache[key];
                    return ds;
                }
            }

            return null;

        }

        private static void UpdateMaxTs(Int64 maxTs)
        {
            InitMaxTS();
            System.Diagnostics.Debug.WriteLine("In cache {0} => input {1}", HttpRuntime.Cache[MaxTsInServer], maxTs);
            if (Convert.ToInt64(HttpRuntime.Cache[MaxTsInServer]) >= maxTs)
                return;

            lock (lockObject)
            {
                if (Convert.ToInt64(HttpRuntime.Cache[MaxTsInServer]) >= maxTs)
                    return;
                HttpRuntime.Cache.Insert(MaxTsInServer,
                        maxTs,
                        null,
                        DateTime.Now.AddYears(10),
                        System.Web.Caching.Cache.NoSlidingExpiration,
                        System.Web.Caching.CacheItemPriority.High,
                        null);
                System.Diagnostics.Debug.WriteLine("Written new cache {0} ", HttpRuntime.Cache[MaxTsInServer]);
            }

        }


        public static void StaticRefreshCacheItem(string key)
        {

            HttpRuntime.Cache.Remove(key);

        }

        public static Int64 StaticGetCacheVersion()
        {
            return (Int64)HttpRuntime.Cache[MaxTsInServer];
        }

        public static List<object> StaticGetCacheDiff(Int64 inTs)
        {
            List<object> lResult = new List<object>();

            foreach (string kvp in ht.Keys)
            {
                byte[] tsBytes = null;
                Int64 nValue = default(Int64);
                foreach (object item in (IEnumerable)StaticGetCacheItem(kvp))
                {
                    tsBytes = (byte[])item.GetType().GetProperty("ts").GetValue(item, null);
                    nValue = Convert.ToInt64(BitConverter.ToString(tsBytes).Replace("-", ""), 16);
                    if ((nValue > inTs))
                    {
                        lResult.Add(item);
                    }
                }
            }
            return lResult;


        }


        #region ICacheProvider Members

        public List<T> GetCacheItem<T>(string key)
        {
            object ds = CustomInMemoryCache.StaticGetCacheItem(key);

            return (List<T>)ds;
        }


        public void RefreshCacheItem(string key)
        {
            CustomInMemoryCache.StaticRefreshCacheItem(key);
        }

        public Int64 GetCacheVersion()
        {
            return StaticGetCacheVersion();
        }

        public List<object> GetCacheDiff(Int64 inTs)
        {
            return StaticGetCacheDiff(inTs);
        }

        #endregion
    }
}
