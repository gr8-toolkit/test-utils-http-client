using System.Net;
using FluentAssertions;
using GR8Tech.Sport.TestUtils.HttpClientFactory.Helpers;
using Tests.Constants;
using Tests.Domain;
using Tests.Helpers;

namespace Tests;

[TestFixture]
public class Tests
{
    [Test]
    public async Task SendRequest_GetValidateStatusCodeAndDeserializeString_ShouldBeCorrectObjectInResponse()
    {
        var resp = await TestServices.HttpClientFactory
            .GetClient(HttpNames.Jsonplaceholder)
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
            .GetClient(HttpNames.Jsonplaceholder)
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
            .GetClient(HttpNames.Jsonplaceholder)
            .GetAsyncAndWaitForStatusCode("/posts/1/comments", HttpStatusCode.OK);
        
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task SendRequest_GetWaitForPredicate_ShouldWaitForExpectedObject()
    {
        var resp = await TestServices.HttpClientFactory
            .GetClient(HttpNames.Jsonplaceholder)
            .GetAsyncAndWaitFor<List<UserComment>>("/posts/1/comments", 
                x => x.Any(comment => comment.Email == "Eliseo@gardner.biz"));

        var actualComment = resp.FirstOrDefault(x => x.Email == "Eliseo@gardner.biz");

        actualComment.Should().NotBeNull();
    }
    
    [Test]
    public async Task SendRequest_GetWaitForPredicateWithCustomRetries_ShouldWaitForExpectedObject()
    {
        var resp = await TestServices.HttpClientFactory
            .GetClient(HttpNames.Jsonplaceholder)
            .GetAsyncAndWaitFor<List<UserComment>>("/posts/1/comments", 
                x => x.Any(comment => comment.Email == "Eliseo@gardner.biz"), retries: 2, timeBetweenRetriesMs: 2);

        var actualComment = resp.FirstOrDefault(x => x.Email == "Eliseo@gardner.biz");

        actualComment.Should().NotBeNull();
    }
    
    [Test]
    public async Task SendRequest_GetWaitForPredicateWithCustomDeserializer_ShouldWaitForExpectedObject()
    {
        var resp = await TestServices.HttpClientFactory
            .GetClient(HttpNames.Jsonplaceholder)
            .GetAsyncAndWaitFor<List<UserComment>>("/posts/1/comments", 
                x => x.Any(comment => comment.Email == "Eliseo@gardner.biz"), deserializer: new CustomDeserializer());

        var actualComment = resp.FirstOrDefault(x => x.Email == "Eliseo@gardner.biz");

        actualComment.Should().NotBeNull();
    }
    
    [Test]
    public async Task AddClientInRun_SendRequest_RemoveClientInRun()
    {
        var fakeStoreClient = new HttpClient { BaseAddress = new Uri("https://fakestoreapi.com/") };
        
        TestServices.HttpClientFactory.AddClient(HttpNames.Fakestoreapi, fakeStoreClient);

        await TestServices.HttpClientFactory
            .GetClient(HttpNames.Fakestoreapi)
            .Get("/products/1")
            .ValidateStatusCodeAsync();
        
        TestServices.HttpClientFactory.RemoveClient(HttpNames.Fakestoreapi);
    }
}