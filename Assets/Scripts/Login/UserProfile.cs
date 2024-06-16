using System;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class UserProfile
{
    public string Token { get; set; }
    public string ProfileName { get; set; }
    public int Age { get; set; }
    public string Email { get; set; }
    public string Office { get; set; }
    public string Bio { get; set; }
    public string Thread { get; set; }

    private static string filePath = Path.Combine(Application.persistentDataPath, "UserProfile.json");

    public void Save()
    {
        JObject userProfile = new JObject
        {
            { "token", Token },
            { "profileName", ProfileName },
            { "age", Age },
            { "email", Email },
            { "office", Office },
            { "bio", Bio },
            { "thread", Thread }
        };

        File.WriteAllText(filePath, userProfile.ToString());
    }

    public static UserProfile Load()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            JObject userProfile = JObject.Parse(json);
            return new UserProfile
            {
                Token = userProfile["token"].ToString(),
                ProfileName = userProfile["profileName"].ToString(),
                Age = (int)userProfile["age"],
                Email = userProfile["email"].ToString(),
                Office = userProfile["office"].ToString(),
                Bio = userProfile["bio"].ToString(),
                Thread = userProfile["thread"].ToString()
            };
        }
        return new UserProfile();
    }

    public void Clear()
    {
        Token = "";
        ProfileName = "";
        Age = 0;
        Email = "";
        Office = "";
        Bio = "";
        Thread = "";
        Save();
    }

    public void UpdateProfile(string token, string profileName, int age, string email, string office, string bio, string thread)
    {
        Token = token;
        ProfileName = profileName;
        Age = age;
        Email = email;
        Office = office;
        Bio = bio;
        Thread = thread;
        Save();
    }
}
