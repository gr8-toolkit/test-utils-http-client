using System.Collections.Concurrent;
using System.Net;
using GR8Tech.Sport.TestUtils.HttpClientFactory.Abstractions;
using GR8Tech.Sport.TestUtils.HttpClientFactory.Configurations;
using GR8Tech.Sport.TestUtils.HttpClientFactory.Configurations.Options;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace GR8Tech.Sport.TestUtils.HttpClientFactory.Implementation;

public sealed class HttpClientsFactory : IHttpClientsFactory
{
    private readonly HttpFactoryOptions? _options;

    private readonly ConcurrentDictionary<string, ISmartHttpClient> _httpClients = new();
    
    private readonly ILogger _logger;

    public HttpClientsFactory()
    {
        _options = SettingsProvider.Configuration.Get<Configuration>()!.HttpFactorySettings;
        
        _logger = SettingsProvider.Logger.ForContext<HttpClientsFactory>();

        if (_options?.HttpClients is not null)
        {
            foreach (var client in _options.HttpClients)
            {
                var handler = new HttpClientHandler();

                if (client.Value.UseCookieContainer)
                {
                    handler.CookieContainer = new CookieContainer();
                    handler.UseCookies = true;
                }

                var httpClient = new HttpClient(handler)
                {
                    BaseAddress = new Uri(client.Value.Url)
                };

                if (client.Value.Headers is not null)
                {
                    foreach (var header in client.Value.Headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                _httpClients[client.Key] = new SmartHttpClient(httpClient);
            }
        }
    }
    
    public void AddClient(string clientName, HttpClient client)
    {
        if (_httpClients.ContainsKey(clientName))
        {
            _logger.Error("HttpClient with Name: {clientName}, already exist in http factory and can't be added again to factory.", clientName);
            return;
        }
        if (!_httpClients.TryAdd(clientName, new SmartHttpClient(client)))
        {
            _logger.Error("Unable to add HttpClient: {clientName}", clientName);
        }
    }
    
    public void RemoveClient(string clientName)
    {
        if (!_httpClients.ContainsKey(clientName))
        {
            _logger.Error("HttpClient with Name: {clientName} is not exist in http factory and can't be removed.", clientName);
            return;
        }
        if (_httpClients.ContainsKey(clientName))
        {
            _httpClients[clientName].Dispose();
        }
        if (!_httpClients.TryRemove(clientName, out ISmartHttpClient customHttpClientValue))
        {
            _logger.Error("Unable to remove HttpClient: {clientName}", clientName);
        }
    }

    public ISmartHttpClient GetClient(string name)
    {
        if (!_httpClients.ContainsKey(name))
        {
            _logger.Error("Unable find HttpClient with name: {name} in HttpClientsFactory", name);
            throw new Exception($"Unable find HttpClient with name: {name} in HttpClientsFactory");
        }
        
        return _httpClients[name];
    }
    
    public void Dispose()
    {
        foreach (var client in _httpClients.Values)
        {
            client.Dispose();
        }
        _httpClients.Clear();
    }
}
