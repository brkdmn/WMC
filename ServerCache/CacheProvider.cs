using System;
using System.Configuration;

namespace ServerCache
{
    public static class CacheProvider
    {
        private static ICacheProvider mInstance;
        private static CacheProviderSettings mProvSettings;

        public static ICacheProvider Provider
        {
            get { return mInstance; }
        }

        internal static CacheProviderSettings ProviderSettings
        {
            get { return mProvSettings; }
        }

        static CacheProvider()
        {
            //Debugger.Break();
            // initialize settings. 
            mProvSettings = (CacheProviderSettings)ConfigurationManager.GetSection("CacheProviderSettings");
            InitializeCacheProvider();
        }

        private static void InitializeCacheProvider()
        {
            if (mProvSettings.ProviderName == EnumCacheProviders.InMemoryCache)
            {
                mInstance = CustomInMemoryCache.GetProviderInstance;
            }
            else if (mProvSettings.ProviderName == EnumCacheProviders.AppFabricCache)
            {
                throw new NotImplementedException();
            }
            else
            {
                throw new ArgumentException();
            }


        }
    }
}
