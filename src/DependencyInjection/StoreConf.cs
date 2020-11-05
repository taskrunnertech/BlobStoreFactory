using System;
using System.Collections.Generic;
using System.Text;

namespace BlobStoreFactory.DependencyInjection
{
    public class StoreConf
    {
        public string Connectionstring { get; set; }

        public bool CreateCollectionsIfNotExist { get; set; } = true;

        public bool IsCollectionPrivate { get; set; } = true;
    }
}
