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

    // Dictionary to hold the screens for easy access
    private Dictionary<string, GameObject> screens;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize dictionary and add all screens
        screens = new Dictionary<string, GameObject>()
        {
            { "stock", stockScreen },
            { "widget", widgetScreen },
            { "settings", settingsScreen },
            { "assist", assistScreen }
        };

        // Deactivate all screens at the start
        DeactivateAllScreens();
    }

    // Update is called once per frame
    void Update()
    {

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