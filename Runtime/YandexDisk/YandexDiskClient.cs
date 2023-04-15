using Newtonsoft.Json;
using System;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace YandexDiskSDK
{
    public class YandexDiskClient : MonoBehaviour
    {
        [SerializeField] private string _clientId = "<<Enter your Id>>";

        public bool IsAuthorized { get; private set; } = false;

        private readonly string _domenYandexDisk = "https://cloud-api.yandex.net/v1/disk/";
        private string _token;

        public async void OpenAuthorizePage()
        {
            HttpClient client = new();

            HttpRequestMessage request = new(HttpMethod.Get, $"https://oauth.yandex.ru/authorize?response_type=token&client_id={_clientId}&redirect_uri=https://oauth.yandex.ru/verification_code");
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            Application.OpenURL(response.RequestMessage.RequestUri.ToString());
        }

        public async Task<PersonData> Authorize(string token)
        {
            _token = token;
            PersonData personData = await GetPersonInfo();

            if (personData != null)
            {
                IsAuthorized = true;
                return personData;
            }
            else
            {
                IsAuthorized = false;
                return null;
            }
        }

        public async Task<PersonData> GetPersonInfo()
        {
            HttpClient client = new();

            HttpRequestMessage request = new(HttpMethod.Get, "https://login.yandex.ru/info?format=json");

            request.Headers.Clear();
            request.Headers.Add("Authorization", $"OAuth {_token}");

            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    Debug.LogError("Authorization failed");
                    return null;
                }

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    Debug.LogError("Bad request");
                    return null;
                }

                Debug.LogError("Failed to get person info.");
                return null;
            }

            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            PersonData personData = JsonConvert.DeserializeObject<PersonData>(responseBody);

            return personData;
        }

        public async Task<DiskInfo> GetInfoDisk()
        {
            HttpClient client = new();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, _domenYandexDisk);

            request.Headers.Add("Authorization", $"OAuth {_token}");

            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                Debug.LogError("Failed to retrieve disk information. Check Authorization");
                return null;
            }

            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            DiskInfo diskInfo = JsonConvert.DeserializeObject<DiskInfo>(responseBody);
            return diskInfo;
        }

        public async Task<FolderInfo> GetContentOfFolder(string folderPath)
        {
            HttpClient client = new();
            string pathUrlEncoding = HttpUtility.UrlEncode(folderPath);
            string url = _domenYandexDisk + $"resources?path={pathUrlEncoding}&fields=_embedded.items.name,_embedded.items.type,_embedded.items.path,_embedded.items.file&limit=1000";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Add("Authorization", $"OAuth {_token}");

            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                Debug.LogError("Failed to retrieve information from the folder. Check Authorization");
                return null;
            }

            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            FolderInfo folderInfo = JsonConvert.DeserializeObject<FolderInfo>(responseBody);
            return folderInfo;
        }

        public async Task<FileInfo[]> GetFilesInFolder(string folderPath)
        {
            FolderInfo folderInfo = await GetContentOfFolder(folderPath);
            return folderInfo.Files.ToArray();
        }

        public async Task<ResponseStatus> CreateFolder(string path)
        {
            HttpClient client = new();
            string pathUrlEncoding = HttpUtility.UrlEncode(path);
            string url = _domenYandexDisk + $"resources?path={pathUrlEncoding}";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, url);

            request.Headers.Add("Authorization", $"OAuth {_token}");

            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                Debug.LogError("The file at the given path already exists");
                return ResponseStatus.FileExists;
            }

            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            return ResponseStatus.Success;
        }

        public async Task<LoadFileInfo> UploadFile(string sourcePath, string targetPath, bool overwrite = false)
        {
            if (!File.Exists(sourcePath))
            {
                Debug.LogError($"File {sourcePath} not found");
                return new LoadFileInfo(sourcePath, targetPath, ResponseStatus.FileNotExists);
            }

            string[] strSplit = sourcePath.Split(new char[] { '\\', '/' });
            string fileName = strSplit[strSplit.Length - 1];

            UrlResponse urlResponse = await GetUrlToUploadFile(targetPath + "/" + fileName, overwrite);

            if (urlResponse.Status != ResponseStatus.Success)
            {
                if (urlResponse.Status == ResponseStatus.FileExists)
                {
                    Debug.LogError($"File {targetPath + "/" + fileName} already exists");
                    return new LoadFileInfo(sourcePath, targetPath, urlResponse.Status);
                }

                if (urlResponse.Status == ResponseStatus.Unauthorized)
                {
                    Debug.LogError("Authorization failed");
                    return new LoadFileInfo(sourcePath, targetPath, ResponseStatus.Unauthorized);
                }

                if (urlResponse.Status == ResponseStatus.Failed)
                {
                    Debug.LogError("Failed to get file upload link");
                    return new LoadFileInfo(sourcePath, targetPath, urlResponse.Status);
                }
            }

            using (WebClient client = new WebClient())
            {
                Uri uri = new(urlResponse.Url);
                client.UploadFileAsync(uri, sourcePath);
            }

            return new LoadFileInfo(sourcePath, targetPath, ResponseStatus.Success);
        }

        private async Task<UrlResponse> GetUrlToUploadFile(string targetPath, bool overwrite)
        {
            string urlEncoding = HttpUtility.UrlEncode(targetPath);

            HttpClient client = new();
            HttpRequestMessage request = new(HttpMethod.Get, _domenYandexDisk + $"resources/upload?path={urlEncoding}&overwrite={overwrite}");

            request.Headers.Add("Authorization", $"OAuth {_token}");

            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return new UrlResponse(ResponseStatus.Unauthorized, string.Empty);

            if (response.StatusCode == HttpStatusCode.Conflict)
                return new UrlResponse(ResponseStatus.FileExists, string.Empty);

            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            Dictionary<string, string> dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseBody);

            if (!dict.TryGetValue("href", out string url))
                return new UrlResponse(ResponseStatus.Failed, string.Empty);

            return new UrlResponse(ResponseStatus.Success, url); ;
        }

        public async Task<LoadFileInfo> DownloadFile(FileInfo fileInfo, string targetPath, bool overwrite = false)
        {
            if (!overwrite)
                if (File.Exists(targetPath + "/" + fileInfo.Name))
                {
                    Debug.LogError($"The file is already in the given path: {targetPath}/{fileInfo.Name}");
                    return new LoadFileInfo(fileInfo.Path, targetPath + "/" + fileInfo.Name, ResponseStatus.FileExists);
                }

            using WebClient client = new();
            Uri uri = new(fileInfo.UrlToDownloadFile);

            await client.DownloadFileTaskAsync(uri, targetPath + "/" + fileInfo.Name);

            return new LoadFileInfo(fileInfo.Path, targetPath, ResponseStatus.Success);
        }

        public async Task<LoadFileInfo> DownloadFile(string sourcePath, string targetPath, bool overwrite = false)
        {
            UrlResponse urlResponse = await GetUrlToDownloadFile(sourcePath);

            if (urlResponse.Status != ResponseStatus.Success)
            {
                if (urlResponse.Status == ResponseStatus.FileNotExists)
                {
                    Debug.LogError($"File not found: {sourcePath}");
                    return new LoadFileInfo(sourcePath, targetPath, urlResponse.Status);
                }

                if (urlResponse.Status == ResponseStatus.Unauthorized)
                {
                    Debug.Log("Authorization failed");
                    return new LoadFileInfo(sourcePath, targetPath, urlResponse.Status);
                }

                if (urlResponse.Status == ResponseStatus.Failed)
                {
                    Debug.LogError($"Failed to get download link {sourcePath}");
                    return new LoadFileInfo(sourcePath, targetPath, urlResponse.Status);
                }
            }

            string[] strSplit = sourcePath.Split(new char[] { '\\', '/' });
            string fileName = strSplit[strSplit.Length - 1];

            if (!overwrite)
                if (File.Exists(targetPath + "/" + fileName))
                {
                    Debug.LogError($"The file at the given path already exists: {targetPath}/{fileName}");
                    return new LoadFileInfo(sourcePath, targetPath, ResponseStatus.FileExists);
                }

            using WebClient client = new();
            Uri uri = new(urlResponse.Url);

            client.DownloadFileAsync(uri, targetPath + "/" + fileName);

            return new LoadFileInfo(sourcePath, targetPath, ResponseStatus.Success);
        }

        public async Task<UrlResponse> GetUrlToDownloadFile(string path)
        {
            string urlEncoding = HttpUtility.UrlEncode(path);

            HttpClient client = new();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, _domenYandexDisk + $"resources/download?path={urlEncoding}");

            request.Headers.Add("Authorization", $"OAuth {_token}");

            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return new UrlResponse(ResponseStatus.Unauthorized, string.Empty);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return new UrlResponse(ResponseStatus.FileNotExists, string.Empty);

            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            Dictionary<string, string> dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseBody);

            if (!dict.TryGetValue("href", out string url))
                return new UrlResponse(ResponseStatus.Failed, string.Empty);

            return new UrlResponse(ResponseStatus.Success, url);
        }

        public async Task<LoadFileInfo[]> UploadFiles(string[] sourcePaths, string targetFolderPath, bool overwrite = false)
        {
            List<Task<LoadFileInfo>> tasks = new();

            foreach (var source in sourcePaths)
            {
                tasks.Add(UploadFile(source, targetFolderPath, overwrite));
            }

            return await Task.WhenAll(tasks);
        }

        public async Task<LoadFileInfo[]> DownloadFiles(string[] sourcePaths, string targetFolderPath, bool overwrite = false)
        {
            List<Task<LoadFileInfo>> tasks = new();

            foreach (var source in sourcePaths)
            {
                tasks.Add(DownloadFile(source, targetFolderPath, overwrite));
            }

            return await Task.WhenAll(tasks);
        }

        public async Task<LoadFileInfo[]> DownloadFiles(FileInfo[] files, string targetFolderPath, bool overwrite = false)
        {
            List<Task<LoadFileInfo>> tasks = new();

            foreach (var file in files)
            {
                tasks.Add(DownloadFile(file, targetFolderPath, overwrite));
            }

            return await Task.WhenAll(tasks);
        }
    }

    public enum ResponseStatus
    {
        Unauthorized,
        Success,
        Failed,
        FileExists,
        FileNotExists
    }

    public class UrlResponse
    {
        private ResponseStatus _status;
        private string _url;

        public ResponseStatus Status { get { return _status; } }
        public string Url { get { return _url; } }

        public UrlResponse(ResponseStatus status, string url)
        {
            _status = status;
            _url = url;
        }
    }

    public class LoadFileInfo
    {
        private string _sourcePath;
        private string _targetPath;
        private ResponseStatus _status;

        public string SourcePath { get { return _sourcePath; } }
        public string TargetPath { get { return _targetPath; } }
        public ResponseStatus Status { get { return _status; } }

        public LoadFileInfo(string sourcePath, string targetPath, ResponseStatus status)
        {
            _sourcePath = sourcePath;
            _targetPath = targetPath;
            _status = status;
        }
    } 
}