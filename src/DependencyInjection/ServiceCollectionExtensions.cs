using BlobStoreFactory.DependencyInjection;
using BlobStoreFactory.Repo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlobStoreFactory
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBlobStorage<TClient, TImplementation>(this IServiceCollection services, IConfigurationSection config)
          where TClient : class
          where TImplementation : class, TClient
        {

            var storeConf = new StoreConf();
            config.Bind(storeConf);

            return services.AddBlobStorage<TClient, TImplementation>(storeConf);
        }

        public static IServiceCollection AddBlobStorage<TClient, TImplementation>(this IServiceCollection services, StoreConf storeConf)
          where TClient : class
          where TImplementation : class, TClient
        {

            services.AddScoped<TClient, TImplementation>();


            services.AddSingleton(s => new AccountFactory<TClient>(storeConf));
            services.AddSingleton(s => new BlobFactory<TClient>(s.GetService<AccountFactory<TClient>>().Value));


            return services;
        }

    }

    public class AccountFactory<T>
    {
        public AccountFactory()
        {

        }
        internal AccountFactory(StoreConf config)
        {

            Value = new StorageAccountFactory(config);
        }

        public virtual StorageAccountFactory Value { get; }

    }

    public class BlobFactory<T>
    {

        internal BlobFactory(StorageAccountFactory storageFactory)
        {

               Value = new BlobStorageRepository(storageFactory);
        }

        public IBlob Value { get; }

    }
}
