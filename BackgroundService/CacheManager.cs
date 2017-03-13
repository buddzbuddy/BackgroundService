using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
}