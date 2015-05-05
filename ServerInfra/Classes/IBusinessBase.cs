using System;
using System.Collections.Generic;

namespace ServerInfra.Classes
{
    public interface IBusinessBase
    {
        List<T> CacheHelper<T>() where T : class;
        List<T> CacheHelperWithConnection<T>(string connectionName) where T : class;
        List<T> GetCacheItem<T>(string key) where T : class;
        Int64 GetCacheVersion();
        List<object> GetCacheDiff(Int64 inTs);
    }
}
