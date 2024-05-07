using UnityEngine;
using System.IO;

public static class UserManager
{
    private static string userDataPath = Path.Combine(Application.persistentDataPath, "userData.json");

    // Save User Data
    public static void SaveUserData(User user)
    {
        string json = JsonUtility.ToJson(user);
        File.WriteAllText(userDataPath, json);
        Debug.Log("User data saved");
    }

    // Load User Data
    public static User LoadUserData()
    {
        if (File.Exists(userDataPath))
        {
            string json = File.ReadAllText(userDataPath);
            User user = JsonUtility.FromJson<User>(json);
            Debug.Log("User data loaded");
            return user;
        }
        else
        {
            Debug.LogError("No user data found");
            return null;
        }
    }
}
