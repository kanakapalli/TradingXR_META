using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Palm_Menu_Controller : MonoBehaviour
{
    // Reference to the screens
    public GameObject stockScreen;
    public GameObject widgetScreen;
    public GameObject loginScreen;
    public GameObject settingsScreen;
    public GameObject assistScreen;

    [SerializeField] GameObject[] login_settings = new GameObject[2];

    // Dictionary to hold the screens for easy access
    private Dictionary<string, GameObject> screens;

    private int m_Logged_In;

    private void Awake()
    {
        m_Logged_In = PlayerPrefs.GetInt("_LogIn");
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize dictionary and add all screens
        screens = new Dictionary<string, GameObject>()
        {
            { "stock", stockScreen },
            { "widget", widgetScreen },
            { "login", loginScreen },
            { "settings", settingsScreen },
            { "assist", assistScreen }
        };

        // Deactivate all screens at the start
        DeactivateAllScreens();
        ConfirmLogin();
    }

    private void ConfirmLogin()
    {
        if(m_Logged_In == 1)
        {
            //Already Logged In
            login_settings[0].SetActive(false);
            login_settings[1].SetActive(true);
        }
        else
        {
            //Need To Be Logged In
            login_settings[0].SetActive(true);
            login_settings[1].SetActive(false);
        }
    }

    // Deactivate all screens
    public void DeactivateAllScreens()
    {
        foreach (var screen in screens.Values)
        {
            screen.SetActive(false);
        }
    }

    // Activate or deactivate screen based on button input
    public void ToggleScreen(string screenName)
    {
        if (screens.ContainsKey(screenName))
        {
            bool isActive = screens[screenName].activeSelf;
            // First, deactivate all screens
            DeactivateAllScreens();

            // Toggle the specified screen
            if (!isActive)  // If it was inactive, activate it
            {
                screens[screenName].SetActive(true);
            }
            // If it was active, it remains deactivated as all screens were deactivated
        }
        else
        {
            Debug.LogError("Invalid screen name specified");
        }
    }
}