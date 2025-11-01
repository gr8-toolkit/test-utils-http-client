namespace GR8Tech.Sport.TestUtils.HttpClientFactory.Abstractions;

public interface IHttpClientsFactory : IDisposable
{
    /// <summary>
    /// Remove client from Dictionary in ClientsFactory.
    /// </summary>
    /// <param name="clientName"> Key of client </param>
    public void RemoveClient(string clientName);
    
    /// <summary>
    /// Add client from Dictionary in ClientsFactory.
    /// </summary>
    /// <param name="clientName"> Key of client </param>
    public void AddClient(string clientName, HttpClient client);
    
    /// <summary>
    /// Select client for request sending.
    /// </summary>
    /// <param name="name"> Key of client </param>
    /// <returns></returns>
    public ISmartHttpClient GetClient(string name);
}