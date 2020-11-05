using System;
using System.Collections.Generic;
using System.Text;

namespace BlobStoreFactory.Models
{
    public class BlobItemModel
    {

        public string Name { get; set; }

        public IDictionary<string, string> Metadata { get; set; }
    }
}
