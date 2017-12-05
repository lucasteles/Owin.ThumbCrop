using System;

namespace Owin.ThumbCrop
{
    public class ThumbCropConfig
    {
        internal string BasePath { get; set; }
        public int DefaultWidth { get; set; } = 100;
        public int DefaultHeight { get; set; } = 100;
        public bool UseCache { get; set; } = false;
        public string UrlPattern { get; set; } = ".thumb.axd";

        public IThumbCacheManager CacheManager { get; set; }

        public string NotFoundFile { get; set; }
        public TimeSpan CacheExpireTime { get; set; } = TimeSpan.FromHours(1);
    }
}
