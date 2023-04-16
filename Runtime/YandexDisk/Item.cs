using Newtonsoft.Json;

namespace YandexDiskSDK
{
    public class Item
    {
        public string Path { get; private set; }
        public string Type { get; private set; }
        public string Name { get; private set; }
        public long Size { get; private set; }
        public string UrlToDownloadFile { get; private set; }

        [JsonConstructor] 
        public Item(string path, string type, string name, long size, string file)
        {
            Path = path;
            Type = type;
            Name = name;
            Size = size;
            UrlToDownloadFile = file;
        }
    }
}