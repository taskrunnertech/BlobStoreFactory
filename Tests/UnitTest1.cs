using Azure.Storage.Blobs;
using BlobStoreFactory;
using BlobStoreFactory.DependencyInjection;
using BlobStoreFactory.Repo;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace BlobStoreFactory.UnitTest
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            IServiceCollection services = new ServiceCollection();
            var conf1 = new StoreConf { Connectionstring = "AccountKey=Test1" };
            services.AddBlobStorage<IMyClass1, MyClass1>(conf1);
            var conf2 = new StoreConf { Connectionstring = "AccountKey=Test2" };
            services.AddBlobStorage<IMyClass2, MyClass2>(conf2);
            
            var saf1 = A.Fake<StorageAccountFactory>();


            var bcc1 = A.Fake<BlobContainerClient>();
            A.CallTo(() => saf1.BlobContainer("container1")).Returns(Task.FromResult(bcc1));
            A.CallTo(() => bcc1.Uri).Returns(new Uri("http://localhost/uri1"));


            var storeFactoryFake1 = A.Fake<AccountFactory<IMyClass1>>();
            A.CallTo(() => storeFactoryFake1.Value).Returns(saf1);
            services.AddSingleton<AccountFactory<IMyClass1>>(s => storeFactoryFake1);



            var saf2 = A.Fake<StorageAccountFactory>();

            var bcc2 = A.Fake<BlobContainerClient>();
            A.CallTo(() => saf2.BlobContainer("container2")).Returns(Task.FromResult(bcc2));
            A.CallTo(() => bcc2.Uri).Returns(new Uri("http://localhost/uri2"));

            var storeFactoryFake2 = A.Fake<AccountFactory<IMyClass2>>(); 
            A.CallTo(() => storeFactoryFake2.Value).Returns(saf2);
            services.AddSingleton<AccountFactory<IMyClass2>>(s => storeFactoryFake2);

            var sp = services.BuildServiceProvider();

            var url1 = await sp.GetService<IMyClass1>().Url("container1");
            Assert.Equal("http://localhost/uri1", url1);

            var url2 = await sp.GetService<IMyClass2>().Url("container2");
            Assert.Equal("http://localhost/uri2", url2);
        }
    }

    public interface IMyClass1
    {
        Task<string> Url(string containerName);
    }
    public class MyClass1 : IMyClass1
    {
        private readonly IBlob _blob;

        public MyClass1(BlobFactory<IMyClass1> blobFactory)
        {
            _blob = blobFactory.Value;
        }

        public async Task<string> Url(string containerName)
        {
            return await _blob.BlobUrl(containerName);
        }
    }

    public interface IMyClass2
    {
        Task<string> Url(string containerName);
    }
    public class MyClass2 : IMyClass2
    {
        private readonly IBlob _blob;

        public MyClass2(BlobFactory<IMyClass2> blobFactory)
        {
            _blob = blobFactory.Value;

        }

        public async Task<string> Url(string containerName)
        {
            return await _blob.BlobUrl(containerName);
        }
    }

}
