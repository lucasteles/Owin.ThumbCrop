using Microsoft.AspNetCore.Http;
using System;

namespace Owin.ThumbCrop
{
    public interface IThumbCacheManager
    {
        void Put(byte[] file, TimeSpan expireTime, HttpContext context);
        bool TryGet(HttpContext context, out byte[] file);
    }
}
