using Azure.Storage;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using BlobStoreFactory.Interfaces;
using BlobStoreFactory.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobStoreFactory.Repo
{
    public class BlobStorageRepository : IBlob
    {
        private readonly IStorageAccountFactory _factory;

        public BlobStorageRepository(IStorageAccountFactory factory)
        {
            _factory = factory;
        }


        public async Task<string> Add(string containerName, string filename, byte[] content, string contentType = null, IDictionary<string, string> metaData = null)
        {
            var container = await _factory.BlobContainer(containerName);

            var blobClient = container.GetBlobClient(filename);

            BlobHttpHeaders header = null;
            if (contentType != null)
            {
                header = new BlobHttpHeaders()
                {
                    ContentType = contentType
                };
            }
            
            await blobClient.UploadAsync(new MemoryStream(content), header, metadata: metaData);

            return $"{container.Uri}/{filename}";
        }


        public async Task Delete(string containerName, string filename)
        {
            var container = await _factory.BlobContainer(containerName);

            var blobClient = container.GetBlobClient(filename);

            await blobClient.DeleteIfExistsAsync();
        }


        public async Task<string> Link(string containerName, string filename, int lifetime)
        {
            var container = await _factory.BlobContainer(containerName);

            var blobClient = container.GetBlobClient(filename);

            //  Defines the resource being accessed and for how long the access is allowed.
            var blobSasBuilder = new BlobSasBuilder
            {
                //StartsOn = DateTime.UtcNow.Subtract(clockSkew),
                ExpiresOn = DateTime.UtcNow.AddMinutes(lifetime),
                BlobContainerName = containerName,
                BlobName = filename
            };

            //  Defines the type of permission.
            blobSasBuilder.SetPermissions(BlobSasPermissions.Read);

            //  Builds an instance of StorageSharedKeyCredential      
            var storageSharedKeyCredential = new StorageSharedKeyCredential(container.AccountName, _factory.AccountKey);

            //  Builds the Sas URI.
            BlobSasQueryParameters sasQueryParameters = blobSasBuilder.ToSasQueryParameters(storageSharedKeyCredential);

            return $"{container.Uri}/{filename}{sasQueryParameters}";
        }

        public async Task<string> BlobUrl(string container)
        {
            var blob = await _factory.BlobContainer(container);
            return blob.Uri.ToString();
        }

        public async Task<byte[]> Get(string containerName, string filename)
        {
            var container = await _factory.BlobContainer(containerName);

            var blockBlob = container.GetBlobClient(filename);

            using (var memStream = new MemoryStream())
            {
                await blockBlob.DownloadToAsync(memStream);
                return memStream.ToArray();
            }
        }

        public async Task<List<BlobItemModel>> GetAll(string containerName, string filename)
        {
            List<BlobItem> result = new List<BlobItem>();
            var container = await _factory.BlobContainer(containerName);
            string continuationToken = null;
            do
            {
                var resultSegment = container.GetBlobs(traits:BlobTraits.Metadata, prefix: filename)
                    .AsPages(continuationToken, null);

                foreach (Azure.Page<BlobItem> blobPage in resultSegment)
                {
                    result.AddRange(blobPage.Values.Where(b => !b.Deleted));

                    continuationToken = blobPage.ContinuationToken;
                }

            } while (continuationToken != "");

            return result.Select(b => new BlobItemModel
            {
                Name = b.Name,
                Metadata = b.Metadata
            }).ToList();
        }

    }
}
