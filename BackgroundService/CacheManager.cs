using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;

namespace BackgroundService
{
    public abstract class CacheManager
    {
        public static bool EnableCaching { get { return true; } }  //  CMSProvider.GetSetting("system.enableCache", "true").ToLower() == "true";
        public static int CacheDuration { get { return 300; } } // RDL.Convert.StrToInt(CMSProvider.GetSetting("system.cacheDuration", "30"), 30);

        public static System.Web.Caching.Cache Cache
        {
            get { return HttpContext.Current.Cache; }
        }

        public static void PurgeCacheItems(string prefix, System.Web.Caching.Cache cache = null)
        {
            prefix = prefix.ToLower();
            List<string> itemsToRemove = new List<string>();
            cache = cache ?? Cache;

            IDictionaryEnumerator enumerator = cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Key.ToString().ToLower().StartsWith(prefix))
                    itemsToRemove.Add(enumerator.Key.ToString());
            }

            foreach (string itemToRemove in itemsToRemove)
                cache.Remove(itemToRemove);
        }

        public static void CacheData(string key, object data, HttpContext context = null)
        {
            var cache = context == null ? CacheManager.Cache : context.Cache;

            if (EnableCaching && data != null)
            {
                cache.Insert(key, data, null,
                   DateTime.Now.AddMinutes(CacheDuration), TimeSpan.Zero);
            }
        }

        // version without using settings. else - stack overflow;
        public static void CacheData(string key, object data, int duration)
        {
            if (data != null)
            {
                CacheManager.Cache.Insert(key, data, null,
                   DateTime.Now.AddMinutes(duration), TimeSpan.Zero);
            }
        }
    }

    public static class Caching
    {
        private static int DurationInMinutes = 30;
        /// <summary>
        /// A generic method for getting and setting objects to the memory cache.
        /// </summary>
        /// <typeparam name="T">The type of the object to be returned.</typeparam>
        /// <param name="cacheItemName">The name to be used when storing this object in the cache.</param>
        /// <param name="cacheTimeInMinutes">How long to cache this object for.</param>
        /// <param name="objectSettingFunction">A parameterless function to call if the object isn't in the cache and you need to set it.</param>
        /// <returns>An object of the type you asked for</returns>
        public static T Get<T>(string cacheItemName)
        {
            ObjectCache cache = MemoryCache.Default;
            var cachedObject = cache[cacheItemName];
            if (cachedObject != null)
                return (T)cachedObject;
            return default(T);
        }

        public static T Set<T>(string cacheItemName, Func<T> objectSettingFunction)
        {
            ObjectCache cache = MemoryCache.Default;
            var cachedObject = objectSettingFunction();
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(DurationInMinutes);
            cache.Set(cacheItemName, cachedObject, policy);
            return (T)cachedObject;
        }
        public static void Set<T>(string cacheItemName, T obj)
        {
            MemoryCache.Default.Set(cacheItemName, obj, new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(DurationInMinutes) });
        }
    }
}