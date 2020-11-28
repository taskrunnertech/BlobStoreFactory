# Introduction 
Blob Storage Helper

# Getting Started
```
services.AddBlobStorage<IService, Service>(ConfigurationSection1);
services.AddBlobStorage<IService2, Service2>(ConfigurationSection2);

# or 

services.AddBlobStorage<IService>(ConfigurationSection1); 

```

```
public class Service : IService
{
	private readonly IBlob _blob;
	Service (BlobFactory<IService> blobFactory)
	{
		_blob = blobFactory.Value;
	}
}
```

Configuration
```
	public class StoreConf
    {
        public string Connectionstring { get; set; }

        public bool CreateCollectionsIfNotExist { get; set; } = true;

        public bool IsCollectionPrivate { get; set; } = true;
    }
```

```
{
	"storage1":	{
		"Connectionstring": "",
		"CreateCollectionsIfNotExist":true,
		"IsCollectionPrivate": true
	}
}

```
