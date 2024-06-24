using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Widgets : MonoBehaviour
{
    [SerializeField] private TMP_Text m_Wifi;
    [SerializeField] private TMP_Text m_Battery;
    [SerializeField] private TMP_Text m_Time;

    // Update is called once per frame
    void Update()
    {
        m_Wifi.text = "WiFi: " + GetWifiSignalStrengthAndroid() + "%";
        m_Battery.text = "Battery: " + GetBatteryLevelAndroid() + "%";
        m_Time.text = "Time: " + GetCurrentTime();
    }

    int GetBatteryLevelAndroid()
    {
        try
        {
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    using (var batteryManager = currentActivity.Call<AndroidJavaObject>("getSystemService", "batterymanager"))
                    {
                        int level = batteryManager.Call<int>("getIntProperty", 4); // 4 is BATTERY_PROPERTY_CAPACITY
                        return level;
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Failed to read battery level: " + e.Message);
            return -1;
        }
    }

    int GetWifiSignalStrengthAndroid()
    {
        int rssi = -1;
        try
        {
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    using (var wifiManager = currentActivity.Call<AndroidJavaObject>("getSystemService", "wifi"))
                    {
                        using (var wifiInfo = wifiManager.Call<AndroidJavaObject>("getConnectionInfo"))
                        {
                            rssi = wifiInfo.Call<int>("getRssi");
                            Debug.Log("Raw RSSI: " + rssi); // Log the raw RSSI value
                            rssi = ConvertRssiToPercentage(rssi);
                        }
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Failed to read Wi-Fi signal strength: " + e.Message);
        }
        return rssi;
    }

    int ConvertRssiToPercentage(int rssi)
    {
        int quality;
        if (rssi <= -100)
            quality = 0;
        else if (rssi >= -50)
            quality = 100;
        else
            quality = 2 * (rssi + 100);

        return quality;
    }

    string GetCurrentTime()
    {
        // return System.DateTime.Now.ToString("HH:mm:ss");
        return System.DateTime.Now.ToString("HH:mm");
    }
}
