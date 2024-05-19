using UnityEngine;

public static class TokenManager
{
    private const string TokenKey = "UserToken"; // Key used to save the token in PlayerPrefs

    // Saves the token to PlayerPrefs
    public static void SaveToken(string token)
    {
        PlayerPrefs.SetString(TokenKey, token);
        PlayerPrefs.Save(); // Ensure PlayerPrefs are saved to disk immediately
    }

    // Retrieves the token from PlayerPrefs
    public static string GetToken()
    {
        return PlayerPrefs.GetString(TokenKey, ""); // Return empty string if token is not found
    }

    // Method to clear the token from PlayerPrefs
    public static void ClearToken()
    {
        PlayerPrefs.DeleteKey(TokenKey);
        PlayerPrefs.Save(); // Ensure changes are saved to disk immediately
    }
}
