using System.IO;
using System.Threading.Tasks;

namespace Owin.ThumbCrop.ImageSource
{
    public class LocalFileImageSource : IImageSource
    {
        public Task<(bool Success, string FileName, Stream FileStream)> TryGetImageStream(string requestUrl, ThumbCropConfig config)
        {
            var filePath = ImageSourceTools.CleanFilePath(requestUrl, config.UrlPattern);

            filePath = string.IsNullOrWhiteSpace(filePath)
                ? Path.Combine(config.BasePath, config.NotFoundFile)
                : Path.Combine(config.BasePath, filePath);

            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                return ImageSourceTools.None;

            return Task.FromResult<(bool Success, string FileName, Stream FileStream)>(
                    (true,
                    Path.GetFileName(filePath),
                    File.OpenRead(filePath))
                  );
        }

    }
}
