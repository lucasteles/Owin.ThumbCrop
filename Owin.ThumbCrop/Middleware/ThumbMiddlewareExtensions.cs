using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Owin.ThumbCrop.ImageSource;
using System;
using System.Linq;

namespace Owin.ThumbCrop
{
    public static class ThumbMiddlewareExtensions
    {
        public static IApplicationBuilder UseThumbCrop(this IApplicationBuilder builder, Action<ThumbCropConfig> config)
        {
            var _config = new ThumbCropConfig();
            config?.Invoke(_config);
            return UseThumbCrop(builder, _config);
        }

        public static IApplicationBuilder UseThumbCrop(this IApplicationBuilder builder, ThumbCropConfig config = null)
        {
            if (config == null)
                config = new ThumbCropConfig();

            if (config.CacheManager == null && config.UseCache)
                config.CacheManager = new InMemoryCacheManager();

            if (config.ImageSources == null || !config.ImageSources.Any())
                config.ImageSources = new[] { new LocalFileImageSource() };


            builder.MapWhen(
             context => context.Request.Path.ToString().EndsWith(config.UrlPattern),

             appBranch =>
             {
                 var hostingEnvironment = (IHostingEnvironment)builder.ApplicationServices.GetService(typeof(IHostingEnvironment));

                 config.BasePath = hostingEnvironment.WebRootPath;

                 appBranch.UseMiddleware<ThumbMiddleware>(config);
             });

            return builder;
        }

    }
}
