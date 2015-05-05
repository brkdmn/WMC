using System;
using System.Collections.Generic;

namespace ServerCache
{
    public enum EnumCacheProviders
    {
        InMemoryCache,
        AppFabricCache
    }

    public interface ICacheProvider
    {
        List<T> GetCacheItem<T>(string key);
        void RefreshCacheItem(string key);
        Int64 GetCacheVersion();
        List<object> GetCacheDiff(Int64 inTs);
    }
}
