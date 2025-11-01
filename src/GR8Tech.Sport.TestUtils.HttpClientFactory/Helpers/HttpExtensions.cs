using System.Net;
using GR8Tech.Sport.TestUtils.HttpClientFactory.Abstractions;
using GR8Tech.Sport.TestUtils.HttpClientFactory.Implementation;

namespace GR8Tech.Sport.TestUtils.HttpClientFactory.Helpers;

public static class HttpExtensions
{
    private static IDeserializer _defaultDeserializer { get;}
    
    static HttpExtensions()
    {
        _defaultDeserializer = new Serializer();
    }
    
    /// <summary>
    /// This extension method to asynchronously validate status code from request
    /// </summary>
    /// <param name="httpResponseMessage"></param>
    /// <param name="statusCode"> expected statusCode </param>
    /// <returns> string </returns>
    /// <exception cref="ArgumentException"></exception>
    public static async Task<string> ValidateStatusCodeAsync(this Task<HttpResponseMessage> httpResponseMessage, 
        HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var response = await httpResponseMessage;
        var content = await response.Content.ReadAsStringAsync();

        if (response.StatusCode != statusCode)
        {
            throw new ArgumentException(
                $"\nAPI: {response.RequestMessage?.RequestUri?.AbsolutePath}" +
                $"\nExpected {statusCode}, but was {response.StatusCode}" +
                $"\nWith body: \n{content}");
        }

        return content;
    }

    /// <summary>
    /// This extension method to asynchronously Deserialize String from request after calling ValidateStatusCodeAsync.    
    /// </summary>
    /// <param name="content"> Task<string> that returns after validation of StatusCode</param>
    /// <typeparam name="T"> Type of object to Deserialize </typeparam>
    /// <returns> Object that set as T </returns>
    public static async Task<T> Deserialize<T>(this Task<string> content, IDeserializer deserializer = null)
    {
        return deserializer == null ? _defaultDeserializer.Deserialize<T>(await content) : deserializer.Deserialize<T>(await content);
    }
    
    /// <summary>
    /// This extension method to asynchronously Deserialize object from httpResponseMessage.   
    /// </summary>
    /// <param name="httpResponseMessage"> httpResponseMessage that return after sending general request. </param>
    /// <typeparam name="T"> Type of object to Deserialize </typeparam>
    /// <returns> Object that set as T  </returns>
    public static async Task<T> Deserialize<T>(this Task<HttpResponseMessage> httpResponseMessage, IDeserializer deserializer = null)
    {
        var response = await httpResponseMessage;
        var content = await response.Content.ReadAsStringAsync();
        
        return deserializer == null ? _defaultDeserializer.Deserialize<T>(content) : deserializer.Deserialize<T>(content);
    }
}