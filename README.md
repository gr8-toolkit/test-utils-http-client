# HttpFactory client

Author: Oleh Remishevskyi


[[_TOC_]]

## TL;DR
This repository contains primary the source code of the NuGet package:
- GR8Tech.Sport.TestUtils.HttpClientFactory

Here is the functionality of HttpFactory client.
Which give us an ultimate Library for working with Http.

This project provides you:

* NuGets:
    * HttpClientFactory

## Main interfaces and classes

### IHttpClientsFactory

Use this interface to Add, Remove and Get http client from Factory.
You got class which which implements this interface:

Methods:

* `public void RemoveClient(string clientName);`
* `public void AddClient(string clientName, HttpClient client);`
* `public ICustomHttpClient SendTo(string name);`


### ICustomHttpClient

Use this interface to work with clients that exist in factory.
Available methods for Post, Put, Patch, Get, Delete and methods with smart waiters for condition on Post and Get
You got class which implements this interface:

Methods:

* `public Task<HttpResponseMessage> Post<T>(string endpoint, T payload);`
* `public Task<HttpResponseMessage> Post(string endpoint);`
* `public Task<HttpResponseMessage> Post(string endpoint, string filePath);`
* `public Task<HttpResponseMessage> Put<T>(string endpoint, T payload);`
* `public Task<HttpResponseMessage> Patch<T>(string endpoint, T payload);`
* `public Task<HttpResponseMessage> Get(string endpoint);`
* `public Task<HttpResponseMessage> Delete(string endpoint);`
* `public Task<T> PostAsyncAndWaitFor<T>(string endpoint, object payload, Func<T, bool> condition);`
* `public Task<T> GetAsyncAndWaitFor<T>(string endpoint, Func<T, bool> condition);`
* `public Task<HttpResponseMessage> GetAsyncAndWaitForStatusCode(string endpoint, HttpStatusCode expectedCode);`
* 
### HttpExtensions methods 

Use this extensions methods to work with returned data from clients.
Available methods for ValidateStatusCode and Deserialize.

Methods:

* `public static async Task<string> ValidateStatusCodeAsync(this Task<HttpResponseMessage> httpResponseMessage, HttpStatusCode statusCode = HttpStatusCode.OK);`
* `public static async Task<T> Deserialize<T>(this Task<string> content);`
* `public static async Task<T> Deserialize<T>(this Task<HttpResponseMessage> httpResponseMessage);`


## Settings file

To configure work with HttpClientFactory you have a flexible options test-settings.json.

* You can specify list of HttpClients. Each client should have parameter "Name" and "Url" it's required. Also it is possible to add specific dictionary of headers to each client.
* You can specify SerializerOptions. For now only PropertyNameCaseInsensitive.
* You can specify PollingSettings to set custom retry policy.

Full example of HttpClientFactory section, if you need to configure everything

```json
{
  "HttpFactoryOptions": {
    "PollingSettings": {
      "Retries": 60,
      "TimeBetweenRetriesMs": "1000"
    },
    "SerializerOptions": {
      "PropertyNameCaseInsensitive": true
    },
    "HttpClients": [
      {
        "Name": "Generator",
        "Url": "http://localhost:5099/",
        "Headers": {
          "User-Agent": "YourAppUserAgent",
          "Accept": "application/json",
          "Authorization": "Bearer someAuthTokenExample0987 }"
        }
      },
      {
        "Name": "Jsonplaceholder",
        "Url": "https://jsonplaceholder.typicode.com/"
      }
    ]
  }
}
```

## Examples how to set up and use

```csharp
public class TestServices
{
    public static IHttpClientsFactory HttpClientFactory { get; }
    
    static TestServices()
    {
        //Created all clients from configuraton faile
        HttpClientFactory = new HttpClientsFactory();
    }
}
```

```csharp

[SetUpFixture]
[Parallelizable(ParallelScope.All)]
public class AssemblySetUp
{
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        // Adding custom client in run
        var fakeStoreClient = new HttpClient { BaseAddress = new Uri("https://fakestoreapi.com/") };        
        TestServices.HttpClientFactory.AddClient(HttpNames.Fakestoreapi, fakeStoreClient);
    }

    [OneTimeTearDown]
    public void BaseTearDown()
    {
        //Disposing specific client in factory.
        TestServices.HttpClientFactory.RemoveClient(HttpNames.Fakestoreapi);
        
        //Disposing all factory
        TestServices.HttpClientFactory?.Dispose();
    }
}


```
## Examples how to use in tests

```csharp
[TestFixture]
public class Tests
{

    [Test]
    public async Task SendRequest_GetValidateStatusCodeAndDeserializeString_ShouldBeCorrectObjectInResponse()
    {
        
        var resp = await TestServices.HttpClientFactory
            .SendTo(HttpNames.Jsonplaceholder)
            .Get("/posts/1/comments")
            .ValidateStatusCodeAsync()
            .Deserialize<List<UserComment>>();

        var expectedData =
            JsonReadHelper.GetJsonDataFromFile<List<UserComment>>("ExpectedData/ExpectedUserComments.json");
            
        resp.Should().BeEquivalentTo(expectedData);
    }
    
    [Test]
    public async Task SendRequest_GetAndDeserializeContent_ShouldBeCorrectObjectInResponse()
    {
        
        var resp = await TestServices.HttpClientFactory
            .SendTo(HttpNames.Jsonplaceholder)
            .Get("/posts/1/comments")
            .Deserialize<List<UserComment>>();

        var expectedData =
            JsonReadHelper.GetJsonDataFromFile<List<UserComment>>("ExpectedData/ExpectedUserComments.json");
            
        resp.Should().BeEquivalentTo(expectedData);
    }
    
    [Test]
    public async Task SendRequest_GetWaitForStatusCode_ShouldBeCorrectStatusCode()
    {
        
        var resp = await TestServices.HttpClientFactory
            .SendTo(HttpNames.Jsonplaceholder)
            .GetAsyncAndWaitForStatusCode("/posts/1/comments", HttpStatusCode.OK);
        
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task SendRequest_GetWaitForPredicate_ShouldWaitForExpectedObject()
    {
        
        var resp = await TestServices.HttpClientFactory
            .SendTo(HttpNames.Jsonplaceholder)
            .GetAsyncAndWaitFor<List<UserComment>>("/posts/1/comments", 
                x => x.Any(comment => comment.Email == "Eliseo@gardner.biz"));

        var actualComment = resp.FirstOrDefault(x => x.Email == "Eliseo@gardner.biz");

        actualComment.Should().NotBeNull();
    }
    
    [Test]
    public async Task AddClientInRun_SendRequest_RemoveClientInRun()
    {
        var fakeStoreClient = new HttpClient { BaseAddress = new Uri("https://fakestoreapi.com/") };
        
        TestServices.HttpClientFactory.AddClient(HttpNames.Fakestoreapi, fakeStoreClient);

        var resp = await TestServices.HttpClientFactory
            .SendTo(HttpNames.Fakestoreapi)
            .Get("/products/1")
            .ValidateStatusCodeAsync();
        
        TestServices.HttpClientFactory.RemoveClient(HttpNames.Fakestoreapi);
    }
}



```