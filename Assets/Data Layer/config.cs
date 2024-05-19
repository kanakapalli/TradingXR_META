public class ConfigurationManager
{
    private static ConfigurationManager _instance;
    public static ConfigurationManager Instance => _instance ?? (_instance = new ConfigurationManager());

    public string BaseUrl { get; set; } = "http://13.232.85.119:8000";

    public string Token { get; set; } = "e06474ebf35f368e4b58abeb2b52aa8017ea43a6";

    private ConfigurationManager() { }
}