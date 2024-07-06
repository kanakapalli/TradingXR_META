using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;

public class LoginManager : MonoBehaviour
{
    [SerializeField] TMP_InputField _login_InputField;
    [SerializeField] Button _login_Button;
    [SerializeField] TMP_Text _login_Status;
    [SerializeField] ParticleSystem _login_Particle;
    [SerializeField] Palm_Menu_Controller _menu_Controller;
    [SerializeField] GameObject _loginButton;
    [SerializeField] GameObject _settingsButton;
    [SerializeField] Settings _settings;

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
        string url = "http://13.235.128.23:8000/api/login_with_code/";
        string jsonBody = "{\"loginCode\":\"" + _login_InputField.text + "\"}";
        UnityWebRequest request = new UnityWebRequest(url, "POST");

        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonBody);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
            _login_Status.text = request.error;
            _login_Status.color = Color.red;
            yield break;
        }

        _login_Status.text = "Login Success - Restarting Everything";
        _login_Status.color = Color.green;

        LoginResponse response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);

        UserProfile profile = UserProfile.Load();
        profile.UpdateProfile(
            response.token,
            response.user.username,
            response.user.age,
            response.user.email,
            response.user.office,
            response.user.bio,
            response.user.conversionTheard
        );

        PlayerPrefs.SetInt("_LogIn", 1);
        StartCoroutine(SceneAsync());
    }

    IEnumerator SceneAsync()
    {
        StartCoroutine(PlayParticles());
        yield return new WaitForSeconds(1f);
        //SceneManager.LoadSceneAsync("MainScene");
        _menu_Controller.DeactivateAllScreens();
        _menu_Controller.ConfirmLogin();
        _menu_Controller.ToggleScreen("settings");
        _settings.LoadSettingsData();
        _loginButton.SetActive(false);
        _settingsButton.SetActive(true);
    }

    void Start()
    {
        _login_Particle.gameObject.SetActive(false);

        _login_Button.onClick.AddListener(() =>
        {
            StartCoroutine(LoginWithCode());
        });
    }

    IEnumerator PlayParticles()
    {
        _login_Particle.gameObject.SetActive(true);
        _login_Particle.Play();
        yield return new WaitForSeconds(5f);
        _login_Particle.gameObject.SetActive(false);
    }
}
