using GR8Tech.Sport.TestUtils.HttpClientFactory.Abstractions;
using GR8Tech.Sport.TestUtils.HttpClientFactory.Implementation;

namespace Tests;

public class TestServices
{
    public static IHttpClientsFactory HttpClientFactory { get; }
    
    static TestServices()
    {
        HttpClientFactory = new HttpClientsFactory();
    }
}