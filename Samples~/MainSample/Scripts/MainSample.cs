using TMPro;
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
    [Header("Response result")]
    [SerializeField] private TextMeshProUGUI _resultText;

    private void Start()
    {
        _openAuthorizationPage.onClick.AddListener(_diskClient.OpenAuthorizePage);
        _applyTokenButton.onClick.AddListener(ApplyToken);
        _getPersonInfoButton.onClick.AddListener(GetPersonInfo);
        _getDiskInfoButton.onClick.AddListener(GetDiskInfo);
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
        _diskClient.GetInfoDisk().RunAsyncOnMainThread((diskInfo) =>
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

    private void GetFolderInfo()
    {

    }
}
