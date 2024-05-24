public class ConfigurationManager
{
    private static ConfigurationManager _instance;
    public static ConfigurationManager Instance => _instance ?? (_instance = new ConfigurationManager());

    public string BaseUrl { get; set; } = "http://13.235.128.23:8000";

    public string Token { get; set; } = "3e83129aa855b8235365798308b0058d807fb1ba";

    private ConfigurationManager() { }
}