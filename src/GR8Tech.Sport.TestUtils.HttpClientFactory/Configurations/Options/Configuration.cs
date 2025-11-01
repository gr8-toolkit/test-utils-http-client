namespace GR8Tech.Sport.TestUtils.HttpClientFactory.Configurations.Options;

internal sealed class Configuration
{
    public HttpFactoryOptions HttpFactorySettings { get; set; }
}

internal class HttpFactoryOptions
{
    public Dictionary<string, HttpClientOptions> HttpClients { get; set; } 
    public PollingSettings PollingSettings { get; set; }
    public SerializerOptions SerializerOptions { get; set; }
}

internal class HttpClientOptions
{
    public string Url { get; set; } 
    public bool UseCookieContainer { get; set; } 
    
    public Dictionary<string, string> Headers { get; set; }
}

internal class SerializerOptions
{
    public bool PropertyNameCaseInsensitive { get; set; }
}

internal class PollingSettings
{
    public int Retries { get; set; }
    public int TimeBetweenRetriesMs { get; set; }
}
