using Newtonsoft.Json;

namespace YandexDiskSDK
{
    public class FileInfo
    {
        public string Path { get; private set; }
        public string Type { get; private set; }
        public string Name { get; private set; }
        public string UrlToDownloadFile { get; private set; }

        [JsonConstructor]
        public FileInfo(string path, string type, string name, string file)
        {
            Path = path;
            Type = type;
            Name = name;
            UrlToDownloadFile = file;
        }
    }
}