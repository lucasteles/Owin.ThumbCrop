using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System;
using System.Collections.Concurrent;

namespace Owin.ThumbCrop
{
    public class InMemoryCacheManager : IThumbCacheManager
    {
        private static ConcurrentDictionary<string, (byte[] file, DateTime created, TimeSpan expires)> _cache;

        public InMemoryCacheManager()
        {
            _cache = new ConcurrentDictionary<string, (byte[] file, DateTime created, TimeSpan expires)>();
        }

        public void Put(byte[] file, TimeSpan expireTime, HttpContext context) =>
                _cache.AddOrUpdate(context.Request.GetDisplayUrl(), (file, DateTime.UtcNow, expireTime), (a, b) => b);

        public bool TryGet(HttpContext context, out byte[] file)
        {
            file = null;
            if (_cache.TryGetValue(context.Request.GetDisplayUrl(), out var data))
            {
                if (DateTime.UtcNow - data.created >= data.expires)
                    return false;

                file = data.file;
                Put(file, data.expires, context);
                return true;
            }
            return false;

        }
    }
}
