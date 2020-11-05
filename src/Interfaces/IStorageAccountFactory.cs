//using Microsoft.Azure.Storage.Blob;
using Azure.Storage.Blobs;
using System.Threading.Tasks;

namespace BlobStoreFactory.Interfaces
{
    public interface IStorageAccountFactory
    {

        Task<BlobContainerClient> BlobContainer(string container);

        string AccountKey { get; }
    }
}
