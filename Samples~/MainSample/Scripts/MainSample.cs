using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using YandexDiskSDK;

public class MainSample : MonoBehaviour
{
    [SerializeField] private YandexDiskClient _diskClient;

    [Space(10)]
    [SerializeField] private Button _openAuthorizationPage;

    [Space(10)]
    [Header("Token Setting")]
    [SerializeField] private TMP_InputField _tokenInputField;

    [Space(10)]
    [SerializeField] private Button _applyTokenButton;
    [SerializeField] private Button _getPersonInfoButton;
    [SerializeField] private Button _getDiskInfoButton;

    [Space(10)]
    [Header("Get Content of Folder")]
    [SerializeField] private TMP_InputField _folderPath;
    [SerializeField] private Toggle _recursiveGetContent;
    [SerializeField] private Button _getFolderContentButton;

    [Space(10)]
    [Header("Upload file")]
    [SerializeField] private TMP_InputField _folderPathToUploadInput;
    [SerializeField] private Button _uploadFileButton;

    [Space(10)]
    [Header("Download file")]
    [SerializeField] private TMP_InputField _pathFileToDownloadInput;
    [SerializeField] private Button _downloadFileButton;

    [Space(10)]
    [Header("Response result")]
    [SerializeField] private TextMeshProUGUI _resultText;

    private void Start()
    {
        _openAuthorizationPage.onClick.AddListener(() => { _diskClient.OpenAuthorizationPage(); });
        _applyTokenButton.onClick.AddListener(ApplyToken);
        _getPersonInfoButton.onClick.AddListener(GetPersonInfo);
        _getDiskInfoButton.onClick.AddListener(GetDiskInfo);
        _getFolderContentButton.onClick.AddListener(GetFolderContent);
        _uploadFileButton.onClick.AddListener(UploadFile);
        _downloadFileButton.onClick.AddListener(DownloadFile);
    }

    private void OnDestroy()
    {
        _openAuthorizationPage.onClick.RemoveAllListeners();
        _applyTokenButton.onClick.RemoveAllListeners();
        _getPersonInfoButton.onClick.RemoveAllListeners();
        _getDiskInfoButton.onClick.RemoveAllListeners();
        _getFolderContentButton.onClick.RemoveAllListeners();
        _uploadFileButton.onClick.RemoveAllListeners();
        _downloadFileButton.onClick.RemoveAllListeners();
    }

    private void ApplyToken()
    {
        _diskClient.Authorize(_tokenInputField.text).RunAsyncOnMainThread((person) =>
        {
            if (person == null)
            {
                _resultText.text = "Aurhorization failed";
                Debug.LogError("Authorization failed");
                return;
            }

            Debug.Log("Authorization success");
            Debug.Log($"Welcome, {person.FirstName} {person.LastName}");

            _resultText.text = "Authorization success.\nNow you can use SDK methods. Goodluck!\n\n";

            _resultText.text = $"First name: {person.FirstName}\n";
            _resultText.text += $"Last name: {person.LastName}\n";
            _resultText.text += $"Client id: {person.ClientId}\n";
            _resultText.text += $"Country: {person.Country}\n";
            _resultText.text += $"Email: {person.DefaultEmail}\n";
            _resultText.text += $"Login: {person.Login}\n";
            _resultText.text += "You can see other fields in the PersonData class";
        });
    }

    private void GetPersonInfo()
    {
        _diskClient.GetPersonInfo().RunAsyncOnMainThread((person) =>
        {
            if (person == null)
            {
                _resultText.text = "Aurhorization failed";
                Debug.LogError("Authorization failed");
                return;
            }

            _resultText.text = $"Person name: {person.Name}\n";
            _resultText.text += $"First name: {person.FirstName}\n";
            _resultText.text += $"Last name: {person.LastName}\n";
            _resultText.text += $"Client id: {person.ClientId}\n";
            _resultText.text += $"Country: {person.Country}\n";
            _resultText.text += $"Email: {person.DefaultEmail}\n";
            _resultText.text += $"Login: {person.Login}\n";
            _resultText.text += "You can see other fields in the PersonData class";

            Debug.Log($"Welcome, {person.FirstName} {person.LastName}");
        });
    }

    private void GetDiskInfo()
    {
        _diskClient.GetDiskInfo().RunAsyncOnMainThread((diskInfo) =>
        {
            if (diskInfo == null)
            {
                _resultText.text = "Failed to get disk information. Check Authorization";
                Debug.LogError("Failed to get disk information. Check Authorization");
                return;
            }

            _resultText.text = $"Used space: {diskInfo.UsedSpace} bytes\n";
            _resultText.text += $"Max file size for upload: {diskInfo.MaxFileSize} bytes\n";
            _resultText.text += $"Paid max file size for upload: {diskInfo.PaidMaxFileSize} bytes\n";
            _resultText.text += $"Total space: {diskInfo.TotalSpace} bytes\n";
            _resultText.text += $"Used space: {diskInfo.UsedSpace} bytes\n";
        });
    }

    private void GetFolderContent()
    {
        _diskClient.GetContentOfFolder(_folderPath.text, _recursiveGetContent.isOn).RunAsyncOnMainThread((folder) =>
        {
            if (folder == null)
            {
                return;
            }

            Debug.Log("Info has been received");
            _resultText.text = "Content has been received";
            _resultText.text += "\n-----------------------------------------\n";
            
            foreach (var f in folder.Folders)
            {
                _resultText.text += $"Type: dir\n";
                _resultText.text += $"Name: {f.Name}\n";
                _resultText.text += $"Path: {f.Path}\n\n";
            }

            foreach (var file in folder.Files)
            {
                _resultText.text += $"Type: file\n";
                _resultText.text += $"Name: {file.Name}\n";
                _resultText.text += $"Path: {file.Path}\n";
                _resultText.text += $"Size: {file.Size}\n";
                _resultText.text += $"Url to download file: {file.UrlToDownloadFile}\n\n";
            }
        });
    }

    private void UploadFile()
    {
        string path = EditorUtility.OpenFilePanel("Select file for upload", "", "");

        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("You don't choose the file");
            return;
        }

        _diskClient.UploadFile(path, _folderPathToUploadInput.text, true).RunAsyncOnMainThread((fileInfo) =>
        {
            if (fileInfo.Status == ResponseStatus.Success)
            {
                Debug.Log("File has been uploaded success");

                _resultText.text = "File has been uploaded success\n";
                _resultText.text += $"File uploaded from {fileInfo.SourcePath} to {fileInfo.TargetPath}\n";
            }
        });
    }

    private void DownloadFile()
    {
        string path = EditorUtility.OpenFolderPanel("Select folder to download", "", "");

        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("You don't choose folder");
            return;
        }

        _diskClient.DownloadFile(_pathFileToDownloadInput.text, path, true).RunAsyncOnMainThread((fileInfo) =>
        {
            if (fileInfo.Status == ResponseStatus.Success)
            {
                Debug.Log("File has been downloaded success");

                _resultText.text = "File has been downloaded success\n";
                _resultText.text += $"File downloaded from {fileInfo.SourcePath} to {fileInfo.TargetPath}\n";
            }
        });
    }
}