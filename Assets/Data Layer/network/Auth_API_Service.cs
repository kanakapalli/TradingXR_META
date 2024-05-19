using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class AuthAPIService : MonoBehaviour
{
    // singleton instance 
    public static AuthAPIService instance { get; private set; } = new AuthAPIService();

    private AuthAPIService() { }

    public IEnumerator LoginWithCode(string loginCode, System.Action<User> onSuccess, System.Action<string> onError)
    {
        string url = $"{ConfigurationManager.Instance.BaseUrl}/api/login_with_code/";
        var requestBody = new
        {
            loginCode = loginCode
        };
        string json = JsonConvert.SerializeObject(requestBody);
        byte[] jsonToSend = new UTF8Encoding().GetBytes(json);

        using (UnityWebRequest webRequest = UnityWebRequest.PostWwwForm(url, "POST"))
        {
            webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                onError?.Invoke(webRequest.error);
            }
            else
            {
                try
                {
                    var responseText = webRequest.downloadHandler.text;
                    if (responseText.Contains("Invalid login code")) // Check if the error message is in the response
                    {
                        onError?.Invoke("Invalid login code provided.");
                    }
                    else
                    {
                        var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseText);
                        if (!string.IsNullOrEmpty(loginResponse.Token))
                        {
                            UserManager.SaveUserData(loginResponse.User);
                            TokenManager.SaveToken(loginResponse.Token); // Save token using TokenManager
                            onSuccess?.Invoke(loginResponse.User); // Execute onSuccess callback with User data
                        }
                        else
                        {
                            onError?.Invoke("Login failed, no token received.");
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    onError?.Invoke($"Failed to parse response: {ex.Message}");
                }
            }
        }
    }




}

[System.Serializable]
public class LoginResponse
{
    public string Token;
    public User User;
}
