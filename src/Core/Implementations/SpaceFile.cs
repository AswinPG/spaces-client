using SpacesClient.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpacesClient.Core.Implementations
{
    public class SpaceFile : ICreateSpaceFile, ISpaceFile
    {
        public DateTime CreatedAt { get; set; }

        public string Key { get; set; }

        public Stream Stream { get; }

        public string ContentType { get; }

        public SpaceFile(string key, Stream stream, string contenttype)
        {
            Key = key;
            Stream = stream;
            ContentType = contenttype;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
