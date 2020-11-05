using Azure.Storage.Blobs;
using BlobStoreFactory.DependencyInjection;
using BlobStoreFactory.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlobStoreFactory.Repo
{
    public class StorageAccountFactory : IStorageAccountFactory
    {
        private BlobServiceClient _blobServiceClient = null;
        private readonly StoreConf _config;
        private static object locObject = new object();

        public StorageAccountFactory(StoreConf config)
        {
            string accountKeyStatement = config.Connectionstring?.Split(';').First(c => c.StartsWith("AccountKey"));
            AccountKey = accountKeyStatement?.Split('=').Last();
            _config = config;
        }


        public string AccountKey { get; }

        public virtual async Task<BlobContainerClient> BlobContainer(string blob)
        {
            if (_blobServiceClient == null)
            {
                lock (locObject)
                {
                    if (_blobServiceClient == null)
                    {
                        _blobServiceClient = new BlobServiceClient(_config.Connectionstring);
                    }

                }

            }
            var blobContainer = _blobServiceClient.GetBlobContainerClient(blob);
            if (_config.CreateCollectionsIfNotExist)
            {
                if (_config.IsCollectionPrivate)
                {
                    await blobContainer.CreateIfNotExistsAsync();
                }
                else
                {
                    await blobContainer.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
                }
            }

            return blobContainer;
        }


    }
}
