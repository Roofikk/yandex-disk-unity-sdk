using Newtonsoft.Json;
using System.Collections.Generic;

namespace YandexDiskSDK
{
    public class FolderInfo
    {
        Embedded embedded { get; set; }
        public string Name { get; private set; }
        public string Path { get; private set; }
        public List<FolderInfo> Folders => embedded.Folders;
        public List<FileInfo> Files => embedded.Files;

        [JsonConstructor]
        internal FolderInfo(Embedded _embedded, string name, string path)
        {
            embedded = _embedded;
            Name = name;
            Path = path;
        }
        
        public FolderInfo(Item item)
        {
            Name = item.Name;
            Path = item.Path;

            embedded = new();
        }

        internal class Embedded
        {
            public List<Item> Items { get; private set; }
            public List<FileInfo> Files { get; private set; }
            public List<FolderInfo> Folders { get; private set; }

            [JsonConstructor]
            public Embedded(List<Item> items)
            {
                Items = items;
                Files = new();
                Folders = new();

                foreach(var item in items)
                {
                    if (item.Type == "dir")
                    {
                        Folders.Add(new FolderInfo(item));
                    }

                    if (item.Type == "file")
                    {
                        Files.Add(new FileInfo(item));
                    }
                }
            }

            public Embedded()
            {
                Items = new();
                Files = new();
                Folders = new();
            }
        }
    }
}