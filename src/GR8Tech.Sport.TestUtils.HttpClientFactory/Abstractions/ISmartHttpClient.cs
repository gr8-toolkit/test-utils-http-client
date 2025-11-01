using System.Net;
using System.Net.Http.Headers;

namespace GR8Tech.Sport.TestUtils.HttpClientFactory.Abstractions;

public interface ISmartHttpClient : IDisposable
{
    /// <summary>
    /// Adds a header to the HTTP client instance
    /// </summary>
    /// <param name="key">key for header</param>
    /// <param name="value">value for header </param>
    public void AddHeader(string key, string value);
    
    /// <summary>
    /// Removes a specific header from the HTTP client instance
    /// </summary>
    /// <param name="key">key for header to remove</param>
    public void RemoveHeader(string key);
    
    /// <summary>
    /// Removes all headers from the HTTP client instance
    /// </summary>
    public void RemoveAllHeaders();

    /// <summary>
    /// Returns all current headers from the HTTP client instance
    /// </summary>
    public HttpRequestHeaders ReturnActualHeaders();
    
    /// <summary>
    /// Sends an asynchronous POST request with serialized content body
    /// </summary>
    /// <param name="endpoint"> Endpoint for request </param>
    /// <param name="content"> Concrete object for body in request </param>
    /// <param name="headers"> Optional headers for request </param>
    /// <param name="serializer"> Optional serializer for content </param>
    /// <param name="mediaType"> Media type for content </param>
    /// <returns> HttpResponseMessage </returns>
    public Task<HttpResponseMessage> Post(string endpoint, object content, Dictionary<string, string> headers = null, ISerializer serializer = null, string mediaType = "application/json");
    
    /// <summary>
    /// Sends an asynchronous POST request without content body
    /// </summary>
    /// <param name="endpoint"> Endpoint for request </param>
    /// <param name="headers"> Optional headers for request </param>
    /// <returns> HttpResponseMessage </returns>
    public Task<HttpResponseMessage> Post(string endpoint, Dictionary<string, string> headers = null);
    
    /// <summary>
    /// Sends an asynchronous POST request with file upload as multipart form data
    /// </summary>
    /// <param name="endpoint"> Endpoint for request </param>
    /// <param name="filePath"> Location of json file for request </param>
    /// <param name="contentType"> Should be image/+ type of image that needed to be sent </param>
    /// <param name="iFormFileInController"> IFormFile input parameter name that use in controller.</param>
    /// <returns> HttpResponseMessage </returns>
    public Task<HttpResponseMessage> Post(string endpoint, string filePath, string contentType, string iFormFileInController);

    /// <summary>
    /// Sends an asynchronous POST request with file upload and additional form parameters
    /// </summary>
    /// <param name="endpoint"> Endpoint for request </param>
    /// <param name="filePath"> Location of json file for request </param>
    /// <param name="contentType"> Should be image/+ type of image that needed to be sent </param>
    /// <param name="iFormFileName"> IFormFile input parameter name that use in controller </param>
    /// <param name="fromFormParameters"> [FromForm] parameters that should be sent to controller </param>
    /// <returns> HttpResponseMessage </returns>
    public Task<HttpResponseMessage> Post(
        string endpoint,
        string filePath,
        string contentType,
        string iFormFileName,
        Dictionary<string, string?> fromFormParameters);
    
    /// <summary>
    /// Sends an asynchronous PUT request with serialized content body
    /// </summary>
    /// <param name="endpoint"> Endpoint for request </param>
    /// <param name="content"> Сoncrete object for body in request </param>
    /// <param name="headers"> Optional headers for request </param>
    /// <param name="serializer"> Optional serializer for content </param>
    /// <returns> HttpResponseMessage </returns>
    public Task<HttpResponseMessage> Put(string endpoint, object content, Dictionary<string, string> headers = null, ISerializer serializer = null);
    
    /// <summary>
    /// Sends an asynchronous PUT request without content body
    /// </summary>
    /// <param name="endpoint"> Endpoint for request </param>
    /// <param name="headers"> Optional headers for request </param>
    /// <returns> HttpResponseMessage </returns>
    public Task<HttpResponseMessage> Put(string endpoint, Dictionary<string, string> headers = null);

    /// <summary>
    /// Sends an asynchronous PUT request with file upload as multipart form data
    /// </summary>
    /// <param name="endpoint"> Endpoint for request </param>
    /// <param name="filePath"> Location of json file for request </param>
    /// <param name="contentType"> Should be image/+ type of image that needed to be sent </param>
    /// <param name="iFormFileInController"> IFormFile input parameter name that use in controller.</param>
    /// <returns> HttpResponseMessage </returns>
    public Task<HttpResponseMessage> Put(string endpoint, string filePath, string contentType, string iFormFileInController);
    
    /// <summary>
    /// Sends an asynchronous PATCH request with serialized content body
    /// </summary>
    /// <param name="endpoint"> Endpoint for request </param>
    /// <param name="content"> Сoncrete object for body in request </param>
    /// <param name="headers"> Optional headers for request </param>
    /// <param name="serializer"> Optional serializer for content </param>
    /// <returns> HttpResponseMessage </returns>
    public Task<HttpResponseMessage> Patch(string endpoint, object content, Dictionary<string, string> headers = null, ISerializer serializer = null);

    /// <summary>
    /// Sends an asynchronous GET request
    /// </summary>
    /// <param name="endpoint"> Endpoint and parameters for request </param>
    /// <param name="headers"> Optional headers for request </param>
    /// <returns> HttpResponseMessage </returns>
    public Task<HttpResponseMessage> Get(string endpoint, Dictionary<string, string> headers = null);

    /// <summary>
    /// Sends an asynchronous DELETE request
    /// </summary>
    /// <param name="endpoint"> Endpoint for request </param>
    /// <param name="headers"> Optional headers for request </param>
    /// <returns> HttpResponseMessage </returns>
    public Task<HttpResponseMessage> Delete(string endpoint, Dictionary<string, string> headers = null);
    
    /// <summary>
    /// Sends an asynchronous DELETE request with serialized content body
    /// </summary>
    /// <param name="endpoint"> Endpoint for request </param>
    /// <param name="content"> Сoncrete object for body in request </param>
    /// <param name="headers"> Optional headers for request </param>
    /// <param name="serializer"> Optional serializer for content </param>
    /// <returns> HttpResponseMessage </returns>
    public Task<HttpResponseMessage> Delete(string endpoint, object content, Dictionary<string, string> headers = null, ISerializer serializer = null);

    /// <summary>
    /// Sends an asynchronous POST request and polls until the response meets the specified condition
    /// </summary>
    /// <param name="endpoint"> Endpoint for request </param>
    /// <param name="content"> Сoncrete object for body in request </param>
    /// <param name="condition"> Delegate with wait condition </param>
    /// <param name="headers"> Optional headers for request </param>
    /// <param name="retries"> Optional number of retries </param>
    /// <param name="timeBetweenRetriesMs"> Optional time between retries in milliseconds </param>
    /// <param name="serializer"> Optional serializer for content </param>
    /// <param name="deserializer"> Optional deserializer for response </param>
    /// <returns> Object that set as T </returns>
    public Task<T> PostAsyncAndWaitFor<T>(string endpoint, object content, Func<T, bool> condition, Dictionary<string, string> headers = null, int? retries = null, int? timeBetweenRetriesMs = null, ISerializer serializer = null, IDeserializer deserializer = null);
    
    /// <summary>
    /// Sends an asynchronous POST request and polls until the HTTP response meets the specified condition
    /// </summary>
    /// <param name="endpoint"> Endpoint for request </param>
    /// <param name="content"> Сoncrete object for body in request </param>
    /// <param name="condition"> Delegate with wait condition </param>
    /// <param name="headers"> Optional headers for request </param>
    /// <param name="retries"> Optional number of retries </param>
    /// <param name="timeBetweenRetriesMs"> Optional time between retries in milliseconds </param>
    /// <param name="serializer"> Optional serializer for content </param>
    /// <returns> HttpResponseMessage </returns>
    public Task<HttpResponseMessage> PostAsyncAndWaitForHttpResponse(string endpoint, object content, Func<HttpResponseMessage, bool> condition, Dictionary<string, string> headers = null, int? retries = null, int? timeBetweenRetriesMs = null, ISerializer serializer = null);
    
    /// <summary>
    /// Sends an asynchronous GET request and polls until the response meets the specified condition
    /// </summary>
    /// <param name="endpoint"> Endpoint for request </param>
    /// <param name="condition"> Delegate with wait condition </param>
    /// <param name="headers"> Optional headers for request </param>
    /// <param name="retries"> Optional number of retries </param>
    /// <param name="timeBetweenRetriesMs"> Optional time between retries in milliseconds </param>
    /// <param name="deserializer"> Optional deserializer for response </param>
    /// <returns> Object that set as T </returns>
    public Task<T> GetAsyncAndWaitFor<T>(string endpoint, Func<T, bool> condition, Dictionary<string, string> headers = null, int? retries  = null, int? timeBetweenRetriesMs = null, IDeserializer deserializer = null);
    
    /// <summary>
    /// Sends an asynchronous GET request and polls until the HTTP response meets the specified condition
    /// </summary>
    /// <param name="endpoint"> Endpoint for request </param>
    /// <param name="condition"> Delegate with wait condition </param>
    /// <param name="headers"> Optional headers for request </param>
    /// <param name="retries"> Optional number of retries </param>
    /// <param name="timeBetweenRetriesMs"> Optional time between retries in milliseconds </param>
    /// <returns> HttpResponseMessage </returns>
    public Task<HttpResponseMessage> GetAsyncAndWaitForHttpResponse(string endpoint, Func<HttpResponseMessage, bool> condition, Dictionary<string, string> headers = null, int? retries = null , int? timeBetweenRetriesMs = null);
    
    /// <summary>
    /// Sends an asynchronous GET request and polls until the expected status code is returned
    /// </summary>
    /// <param name="endpoint"> Endpoint for request </param>
    /// <param name="expectedCode"> Status code for wait </param>
    /// <param name="headers"> Optional headers for request </param>
    /// <param name="retries"> Optional number of retries </param>
    /// <param name="timeBetweenRetriesMs"> Optional time between retries in milliseconds </param>
    /// <returns> HttpResponseMessage </returns>
    public Task<HttpResponseMessage> GetAsyncAndWaitForStatusCode(string endpoint, HttpStatusCode expectedCode = HttpStatusCode.OK, Dictionary<string, string> headers = null, int? retries = null, int? timeBetweenRetriesMs = null);
}