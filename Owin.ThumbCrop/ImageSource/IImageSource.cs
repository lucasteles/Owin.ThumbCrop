using System.IO;
using System.Threading.Tasks;

namespace Owin.ThumbCrop.ImageSource
{
    public interface IImageSource
    {
        Task<(bool Success, string FileName, Stream FileStream)> TryGetImageStream(string requestUrl, ThumbCropConfig config);
    }
}
