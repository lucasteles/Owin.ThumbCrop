using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
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
            var ret = false;

            if (_config.UseCache && _config.CacheManager.TryGet(context, out var fileData))
            {
                ret = true;
                await WriteResponse(context, fileData);
            }
            else
                ret = await ProcessRequest(context);

            if (!ret)
                await _next.Invoke(context);
        }

        private async Task<bool> ProcessRequest(HttpContext context)
        {
            var refpoint = ReferencePoint.MiddleCenter;
            var crop = false;
            var width = 100;
            var height = 100;
            var flip = FlipType.None;


            if (context.Request.Query.TryGetValue(nameof(refpoint), out var refpointValue))
                Enum.TryParse(refpointValue.ToString(), out refpoint);

            if (context.Request.Query.TryGetValue(nameof(flip), out var flipValue))
                Enum.TryParse(flipValue.ToString(), out flip);

            if (context.Request.Query.TryGetValue(nameof(crop), out var _))
                crop = true;

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
                var image = img;

                if (flip != FlipType.None)
                    image = Thumbnail.Flip(image, flip);

                image = crop ?
                   Thumbnail.Create(image, size, refpoint) :
                   Thumbnail.Resize(image, size);

                ApplyFilters(image, context.Request.Query);

                using (image)
                {
                    var fileName = Path.GetFileName(filePath);
                    var stream = Thumbnail.Convert(image);
                    var data = stream.ToArray();
                    await WriteResponse(context, data);

                    _config.CacheManager.Put(
                        data,
                        _config.CacheExpireTime,
                        context
                    );

                    return true;
                }
            }
        }

        private async Task WriteResponse(HttpContext context, byte[] file)
        {
            context.Response.ContentType = "image/png";
            await context.Response.Body.WriteAsync(file, 0, file.Length);
            context.Response.Body.Flush();
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
                    filePath = filePath.Substring(0, filePath.LastIndexOf(_config.UrlPattern))
                        .Replace("/", "\\");
                    filePath = filePath.Remove(0, 1);
                }
            }

            return filePath;
        }

        private static void ApplyFilters(Image<Rgba32> image, IQueryCollection query)
        {
            var boolFilters = Thumbnail.GetImageBoolProcessors();
            foreach (var item in boolFilters)
                if (query.TryGetValue(item.Key, out var _))
                    Thumbnail.SetImageAttr(image, item.Value);

            var floatFilters = Thumbnail.GetImageFloatProcessors();
            foreach (var item in floatFilters)
                if (query.TryGetValue(item.Key, out var floatValue))
                    if (float.TryParse(floatValue, out var value))
                        Thumbnail.SetImageAttr(image, item.Value(value));
                    else
                        Thumbnail.SetImageAttr(image, item.Value(0));
        }
    }
}