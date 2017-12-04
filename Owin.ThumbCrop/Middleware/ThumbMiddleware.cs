using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.Primitives;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Owin.ThumbCrop
{
    public class ThumbMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ThumbCropConfig _config;
        public ThumbMiddleware(RequestDelegate next, ThumbCropConfig config)
        {
            _next = next;
            _config = config;
        }

        public async Task Invoke(HttpContext context)
        {
            var ret = await ProcessRequest(context);

            if (!ret)
                await _next.Invoke(context);
        }

        private async Task<bool> ProcessRequest(HttpContext context)
        {
            var refpoint = ReferencePoint.MiddleCenter;
            var crop = false;
            var width = 100;
            var height = 100;


            if (context.Request.Query.TryGetValue(nameof(refpoint), out var refpointValue))
                Enum.TryParse(refpointValue.ToString(), out refpoint);

            if (context.Request.Query.TryGetValue(nameof(crop), out var cropValue))
                bool.TryParse(cropValue.ToString(), out crop);

            if (context.Request.Query.TryGetValue(nameof(width), out var widthValue))
                int.TryParse(widthValue.ToString(), out width);

            if (context.Request.Query.TryGetValue(nameof(height), out var heigtValue))
                int.TryParse(heigtValue.ToString(), out height);

            var filePath = CleanFilePath(context.Request.Path);

            if (string.IsNullOrWhiteSpace(filePath))
                filePath = Path.Combine(_config.BasePath, _config.NotFoundFile);
            else
                filePath = Path.Combine(_config.BasePath, filePath);

            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                return false;

            var size = new Size(width, height);
            using (var img = Image.Load(filePath))
            {
                var image = crop ?
                    Thumbnail.Create(img, size, refpoint) :
                    Thumbnail.Resize(img, size);

                using (image)
                {
                    var fileName = Path.GetFileName(filePath);
                    var stream = Thumbnail.Convert(image);
                    context.Response.ContentType = "image/png";
                    var data = stream.ToArray();
                    await context.Response.Body.WriteAsync(data, 0, data.Length);
                    context.Response.Body.Flush();

                    //if (configuration.Thumb.EnableCache
                    //    && context.Cache[fileName] == null
                    //)
                    //{
                    //    var cachePriority = CacheItemPriority.Normal;
                    //    if (configuration.Thumb.CachePriority > 0)
                    //    {
                    //        Enum.TryParse(configuration.Thumb.CachePriority.ToString(), out cachePriority);
                    //    }

                    //    context.Cache.Add(
                    //        fileName,
                    //        image,
                    //        null,
                    //        DateTime.Now.AddMilliseconds(configuration.Thumb.Expiration),
                    //        TimeSpan.FromMilliseconds(configuration.Thumb.SlidingExpiration),
                    //        cachePriority,
                    //        null
                    //    );
                    //}

                    return true;
                }
            }
        }

        private string CleanFilePath(string filePath)
        {
            if (filePath == null) return null;

            var fileName = Path.GetFileName(filePath);

            if (!string.IsNullOrWhiteSpace(fileName))
            {
                var fileNameFragments = fileName.Split('.');
                if (fileNameFragments.Length > 1)
                {
                    filePath = filePath.Replace(
                        fileName,
                        $"{fileNameFragments[0]}.{fileNameFragments[1]}")
                        .Replace("/", "\\");
                    filePath = filePath.Remove(0, 1);
                }
            }

            return filePath;
        }
    }
}