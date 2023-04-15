<<<<<<< HEAD
using Newtonsoft.Json;
using System.Collections.Generic;

namespace YandexDiskSDK
{
    public class FolderInfo
    {
        Embedded embedded { get; set; }

        public List<FileInfo> Files => embedded.Files;

        [JsonConstructor]
        internal FolderInfo(Embedded _embedded)
        {
            embedded = _embedded;
        }

        internal class Embedded
        {
            public List<FileInfo> Files { get; set; }

            [JsonConstructor]
            public Embedded(List<FileInfo> items)
            {
                Files = items;
            }
        }
    }
=======
using Newtonsoft.Json;
using System.Collections.Generic;

namespace YandexDiskSDK
{
    public class FolderInfo
    {
        Embedded embedded { get; set; }

        public List<FileInfo> Files => embedded.Files;

        [JsonConstructor]
        internal FolderInfo(Embedded _embedded)
        {
            embedded = _embedded;
        }

        internal class Embedded
        {
            public List<FileInfo> Files { get; set; }

            [JsonConstructor]
            public Embedded(List<FileInfo> items)
            {
                Files = items;
            }
        }
    }
>>>>>>> 54935b7afcc8c9ace832f3baf9523de90599e286
}