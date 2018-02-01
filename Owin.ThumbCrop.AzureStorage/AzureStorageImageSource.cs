using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Threading.Tasks;

namespace Owin.ThumbCrop.ImageSource
{
    public class AzureStorageImageSource : IImageSource
    {
        private readonly CloudBlobContainer _container;

        public AzureStorageImageSource(string connectionString, string containerName)
        {
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            _container = blobClient.GetContainerReference(containerName);
        }

        public async Task<(bool Success, string FileName, Stream FileStream)> TryGetImageStream(string requestUrl, ThumbCropConfig config)
        {
            var filePath = ImageSourceTools.CleanFilePath(requestUrl, config.UrlPattern);

            var fileName = Path.GetFileName(filePath);

            var blob = _container.GetBlobReference(fileName);

            if (!(await blob.ExistsAsync()))
                return await ImageSourceTools.None;

            var memoryStream = new MemoryStream();
            await blob.DownloadToStreamAsync(memoryStream);
            memoryStream.Position = 0;
            return (true, fileName, memoryStream);

        }
    }
}
