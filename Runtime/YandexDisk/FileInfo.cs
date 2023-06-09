using Newtonsoft.Json;

namespace YandexDiskSDK
{
    public class FileInfo
    {
        public string Path { get; private set; }
        public string Name { get; private set; }
        public long Size { get; private set; }
        public string UrlToDownloadFile { get; private set; }

        [JsonConstructor]
        public FileInfo(string path, string name, long size, string file)
        {
            Path = path;
            Name = name;
            Size = size;
            UrlToDownloadFile = file;
        }

        public FileInfo(Item item)
        {
            Path = item.Path;
            Name = item.Name;
            Size = item.Size;
            UrlToDownloadFile = item.UrlToDownloadFile;
        }
    }
}