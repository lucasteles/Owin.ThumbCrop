using System.IO;
using System.Threading.Tasks;

namespace Owin.ThumbCrop.ImageSource
{
    public static class ImageSourceTools
    {

        public static string CleanFilePath(string filePath, string urlPattern)
        {
            if (filePath == null) return null;

            var fileName = Path.GetFileName(filePath);

            if (!string.IsNullOrWhiteSpace(fileName))
            {
                var fileNameFragments = fileName.Split('.');
                if (fileNameFragments.Length > 1)
                {
                    filePath = filePath.Substring(0, filePath.LastIndexOf(urlPattern))
                        .Replace("/", "\\");
                    filePath = filePath.Remove(0, 1);
                }
            }

            return filePath;
        }

        public static Task<(bool Success, string FileName, Stream FileStream)> None => Task.FromResult<(bool Success, string FileName, Stream FileStream)>((false, string.Empty, Stream.Null));


    }
}
