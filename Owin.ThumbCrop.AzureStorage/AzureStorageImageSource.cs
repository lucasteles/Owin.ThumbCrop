using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Owin.ThumbCrop.ImageSource;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Owin.ThumbCrop.AzureStorage
{
    public class AzureStorageImageSource : IImageSource
    {
        private readonly string[] _containers;
        private readonly string _connectionString;

        public AzureStorageImageSource(string connectionString, params string[] allowedContainers)
        {
            _connectionString = connectionString;
            _containers = allowedContainers;

        }

        public async Task<(bool Success, string FileName, Stream FileStream)> TryGetImageStream(string requestUrl, ThumbCropConfig config)
        {

            var filePath = ImageSourceTools.CleanFilePath(requestUrl, config.UrlPattern);

            var fileName = Path.GetFileName(filePath);
            var containerName = Path.GetDirectoryName(filePath);

            if (!_containers.Contains(containerName))
                return await ImageSourceTools.None;

            var container = GetContainer(containerName);
            var blob = container.GetBlobReference(fileName);

            if (!(await blob.ExistsAsync()))
                return await ImageSourceTools.None;

            var memoryStream = new MemoryStream();
            await blob.DownloadToStreamAsync(memoryStream);
            memoryStream.Position = 0;
            return (true, fileName, memoryStream);

        }

        private CloudBlobContainer GetContainer(string containerName)
        {
            var storageAccount = CloudStorageAccount.Parse(_connectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            return blobClient.GetContainerReference(containerName);

        }
    }
}
