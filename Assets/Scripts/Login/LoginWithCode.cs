using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    [SerializeField] TMP_InputField _login_InputField;
    [SerializeField] Button _login_Button;
    [SerializeField] TMP_Text _login_Status;
    [SerializeField] My_Profile _profile;

    [System.Serializable]
    public class User
    {
        public string username;
        public string email;
        public string office;
        public int age;
        public string bio;
        public string loginCode;
        public string conversionTheard;
        public string profile_picture;
    }

    [System.Serializable]
    public class LoginResponse
    {
        public string token;
        public User user;
    }

    IEnumerator LoginWithCode()
    {
        // URL of the API
        string url = "http://13.235.128.23:8000/api/login_with_code/";

        // JSON body data
        string jsonBody = "{\"loginCode\":\"" + _login_InputField.text + "\"}";

        // Create a UnityWebRequest object
        UnityWebRequest request = new UnityWebRequest(url, "POST");

        // Set the upload handler with the JSON body
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonBody);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);

        // Set the download handler
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        // Set the content type
        request.SetRequestHeader("Content-Type", "application/json");

        // Send the request
        yield return request.SendWebRequest();

        // Check for errors
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
            _login_Status.text = request.error;
            _login_Status.color = Color.red;
            yield break;
        }

        _login_Status.text = "Login Success";
        _login_Status.color = Color.green;

        // Parse the JSON response
        LoginResponse response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);

        // Accessing user components
        Debug.Log("Token: " + response.token);
        Debug.Log("Username: " + response.user.username);
        Debug.Log("Email: " + response.user.email);
        Debug.Log("Age: " + response.user.age);
        Debug.Log("Office: " + response.user.office);
        Debug.Log("Bio: " + response.user.bio);
        Debug.Log("Conversion Thread: " + response.user.conversionTheard);
        Debug.Log("Login Code: " + response.user.loginCode);
        Debug.Log("Profile Picture: " + response.user.profile_picture);

        // Adding it to the my profile scriptable object;
        _profile.m_Token = response.token;
        _profile.m_ProfileName = response.user.username;
        _profile.m_Email = response.user.email;
        _profile.m_Age = response.user.age;
        _profile.m_Bio = response.user.bio;
        _profile.m_Office = response.user.office;
        _profile.m_Thread = response.user.conversionTheard;

        // Process the response as needed
        if (request.result == UnityWebRequest.Result.Success)
        {
            PlayerPrefs.SetInt("_LogIn", 1);
            //StartCoroutine(SceneAsync());
        }
    }

    IEnumerator SceneAsync()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadSceneAsync("MainScene");
    }

    // Example usage
    void Start()
    {
        _login_Button.onClick.AddListener(() =>
        {
            StartCoroutine(LoginWithCode());
        });
    }
}
