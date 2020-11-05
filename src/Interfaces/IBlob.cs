using BlobStoreFactory.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlobStoreFactory
{
    public interface IBlob
    {
        Task<string> Add(string container, string filename, byte[] content, string contentType = default, IDictionary<string, string> metaData = null);

        Task<string> Link(string container, string filename, int lifetime);

        Task Delete(string container, string filename);

        Task<string> BlobUrl(string container);

        Task<byte[]> Get(string container, string filename);

        Task<List<BlobItemModel>> GetAll(string containerName, string filename);

    }
}
