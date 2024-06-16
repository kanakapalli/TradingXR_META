using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class Settings : MonoBehaviour
{
    [SerializeField] private TMP_Text m_ProfileName;
    [SerializeField] private Button m_Logout_Button;

    private int m_logged_in;

    private void Awake()
    {
        m_logged_in = PlayerPrefs.GetInt("_LogIn");
        m_Logout_Button.onClick.AddListener(() =>
        {
            PlayerPrefs.SetInt("_LogIn", 0);
            UserProfile profile = UserProfile.Load();
            profile.Clear();
            SceneManager.LoadSceneAsync("MainScene");
        });
    }

    private void Start()
    {
        StartCoroutine(LoadDataIfLoggedIn());
    }

    private IEnumerator LoadDataIfLoggedIn()
    {
        if (m_logged_in == 1)
        {
            UserProfile profile = UserProfile.Load();
            yield return new WaitForSeconds(.2f);
            m_ProfileName.text = profile.ProfileName;
            Debug.Log(m_ProfileName.text);
        }
    }
}
