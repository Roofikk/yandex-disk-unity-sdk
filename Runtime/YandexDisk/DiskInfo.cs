using Newtonsoft.Json;

namespace YandexDiskSDK
{
    public class DiskInfo
    {
        public long MaxFileSize { get; private set; }
        public long PaidMaxFileSize { get; private set; }
        public long TotalSpace { get; private set; }
        //Занимаемый размер в корзине
        public long TrashSize { get; private set; }
        public long UsedSpace { get; private set; }
        public PersonData User { get; private set; }

        [JsonConstructor]
        public DiskInfo(long max_file_size, long paid_max_file_size, long total_space, long trash_size, long used_space, PersonData user)
        {
            MaxFileSize = max_file_size;
            PaidMaxFileSize = paid_max_file_size;
            TotalSpace = total_space;
            TrashSize = trash_size;
            UsedSpace = used_space;
            User = user;
        }
    }
}