using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;

namespace Owin.ThumbCrop
{
    public static class ThumbMiddlewareExtensions
    {
        public static IApplicationBuilder UseThumbMiddleware(this IApplicationBuilder builder, Action<ThumbCropConfig> config)
        {
            var _config = new ThumbCropConfig();
            config?.Invoke(_config);
            return UseThumbMiddleware(builder, _config);
        }

        public static IApplicationBuilder UseThumbMiddleware(this IApplicationBuilder builder, ThumbCropConfig config = null)
        {
            if (config == null)
                config = new ThumbCropConfig();

            if (config.CacheManager == null && config.UseCache)
                config.CacheManager = new InMemoryCacheManager();

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
