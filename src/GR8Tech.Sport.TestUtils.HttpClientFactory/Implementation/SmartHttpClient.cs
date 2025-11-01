using System.Net;
using System.Net.Http.Headers;
using System.Text;
using GR8Tech.Sport.TestUtils.HttpClientFactory.Abstractions;
using GR8Tech.Sport.TestUtils.HttpClientFactory.Configurations;
using GR8Tech.Sport.TestUtils.HttpClientFactory.Helpers;
using Serilog;

namespace GR8Tech.Sport.TestUtils.HttpClientFactory.Implementation;

internal sealed class SmartHttpClient : ISmartHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;
    private ISerializer _defaultSerializer { get; }

    public SmartHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        
        _logger = SettingsProvider.Logger.ForContext<SmartHttpClient>();
        _defaultSerializer = new Serializer();
    }

    #region General methods
    
    public Task<HttpResponseMessage> Post(string endpoint, object content, Dictionary<string, string> headers = null, ISerializer serializer = null, string mediaType = "application/json")
    {
        _logger.Debug("Post request to endpoint {endpoint}. Payload {@payload}", endpoint, content);
    
        try
        {
            var serializedObject = serializer == null ? _defaultSerializer.Serialize(content) : serializer.Serialize(content);
        
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(serializedObject, Encoding.UTF8, mediaType)
            };
        
            headers?.ToList().ForEach(h => request.Headers.Add(h.Key, h.Value));
        
            return _httpClient.SendAsync(request);
        }
        catch (Exception e)
        {
            _logger.Error("Post request to endpoint {endpoint} failed with error: {error}. Body: {@payload}", 
                endpoint, e, content);
            throw;
        }
    }

    public Task<HttpResponseMessage> Post(string endpoint, Dictionary<string, string> headers = null)
    {
        _logger.Debug("Post request to endpoint {endpoint}.", endpoint);
        
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            
            headers?.ToList().ForEach(h => request.Headers.Add(h.Key, h.Value));
            
            return _httpClient.SendAsync(request);
        }
        catch (Exception e)
        {
            _logger.Error("Post request to endpoint {endpoint} failed with error: {error}. Body: null", 
                endpoint, e);
            throw;
        }
    }

    public async Task<HttpResponseMessage> Post(
        string endpoint, 
        string filePath, 
        string contentType, 
        string iFormFileInController)
    {
        _logger.Debug("Post request to endpoint {endpoint} with file {filePath}", endpoint, filePath);
        
        string fullFilePath = Directory.GetCurrentDirectory() + filePath ?? string.Empty;

        try
        {
            using var formData = new MultipartFormDataContent();

            if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(fullFilePath))
            {
                var fileStream = new FileStream(fullFilePath, FileMode.Open, FileAccess.Read);
                var imageContent = new StreamContent(fileStream);
                imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
                formData.Add(imageContent, iFormFileInController, Path.GetFileName(fullFilePath));
            }

            // Post with or without file
            return await _httpClient.PostAsync(endpoint, formData);
        }
        catch (Exception e)
        {
            _logger.Error(
                "Post request to endpoint {endpoint} failed. Error: {error}. File path: {filePath}",
                endpoint, e, filePath);
            throw;
        }
    }
    
    public async Task<HttpResponseMessage> Post(
        string endpoint,
        string? filePath,
        string contentType,
        string iFormFileName,
        Dictionary<string, string?> fromFormParameters)
    {
        _logger.Debug("Post request to endpoint {endpoint} with file {filePath}", endpoint, filePath);

        try
        {
            string fullFilePath = Directory.GetCurrentDirectory() + filePath ?? string.Empty;
            
            using var formData = new MultipartFormDataContent();

            // Add file if provided
            if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(fullFilePath))
            {
                if (!File.Exists(fullFilePath))
                {
                    throw new FileNotFoundException($"File not found at {fullFilePath}");
                }

                var fileStream = new FileStream(fullFilePath, FileMode.Open, FileAccess.Read);
                var imageContent = new StreamContent(fileStream);
                imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
                formData.Add(imageContent, iFormFileName, Path.GetFileName(fullFilePath));
            }

            // Add extra form fields
            foreach (var parameter in fromFormParameters)
            {
                formData.Add(new StringContent(parameter.Value ?? string.Empty), parameter.Key);
            }

            return await _httpClient.PostAsync(endpoint, formData);
        }
        catch (Exception e)
        {
            _logger.Error(
                e,
                "Post request to endpoint {endpoint} failed. File: {filePath}", 
                endpoint, filePath);
            throw;
        }
    }

    public Task<HttpResponseMessage> Put(string endpoint, object content, Dictionary<string, string> headers = null, ISerializer serializer = null)
    {
        _logger.Debug("Put request to endpoint {endpoint}. Payload {@payload}", endpoint, content);
    
        try
        {
            var serializedObject = (serializer ?? _defaultSerializer).Serialize(content);
    
            var request = new HttpRequestMessage(HttpMethod.Put, endpoint)
            {
                Content = new StringContent(serializedObject, Encoding.UTF8, "application/json")
            };
        
            headers?.ToList().ForEach(h => request.Headers.Add(h.Key, h.Value));
        
            return _httpClient.SendAsync(request);
        }
        catch (Exception e)
        {
            _logger.Error("Put request to endpoint {endpoint} failed with error: {error}. Body: {@payload}", 
                endpoint, e, content);
            throw;
        }
    }
    
    public Task<HttpResponseMessage> Put(string endpoint, Dictionary<string, string> headers = null)
    {
        _logger.Debug("Put request to endpoint {endpoint}.", endpoint);
    
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Put, endpoint);
            headers?.ToList().ForEach(h => request.Headers.Add(h.Key, h.Value));
            return _httpClient.SendAsync(request);
        }
        catch (Exception e)
        {
            _logger.Error("Put request to endpoint {endpoint} failed with error: {error}. Body: null", 
                endpoint, e);
            throw;
        }
    }

    
    public async Task<HttpResponseMessage> Put(string endpoint, string filePath, string contentType, string iFormFileInController)
    {
        _logger.Debug("Put request to endpoint {endpoint}. With FilePath {filePath}", endpoint, filePath);

        string fullFilePath = Directory.GetCurrentDirectory() + filePath;
        
        try
        {
            var formData = new MultipartFormDataContent();
            var fileStream = new FileStream(fullFilePath, FileMode.Open, FileAccess.Read);
            var imageContent = new StreamContent(fileStream);
            imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
            formData.Add(imageContent, iFormFileInController, Path.GetFileName(fullFilePath));

            var response = await _httpClient.PutAsync(endpoint, formData);
            formData.Dispose();
            await fileStream.DisposeAsync();
        
            return response;
        }
        catch (Exception e)
        {
            _logger.Error("Put request to endpoint {endpoint} failed. Error: {error}. File for request location: {filePath}", 
                endpoint, e, filePath);
            throw;
        }
    }

    public Task<HttpResponseMessage> Patch(string endpoint, object content, Dictionary<string, string> headers = null, ISerializer serializer = null)
    {
        _logger.Debug("Patch request to endpoint {endpoint}. Payload {@payload}", endpoint, content);
    
        try
        {
            var serializedObject = (serializer ?? _defaultSerializer).Serialize(content);
    
            var request = new HttpRequestMessage(HttpMethod.Patch, endpoint)
            {
                Content = new StringContent(serializedObject, Encoding.UTF8, "application/json")
            };
        
            headers?.ToList().ForEach(h => request.Headers.Add(h.Key, h.Value));
        
            return _httpClient.SendAsync(request);
        }
        catch (Exception e)
        {
            _logger.Error("Patch request to endpoint {endpoint} failed with error: {error}. Body: {@payload}", 
                endpoint, e, content);
            throw;
        }
    }

    public Task<HttpResponseMessage> Get(string endpoint, Dictionary<string, string> headers = null)
    {
        _logger.Debug("Get request to endpoint {endpoint}.", endpoint);
    
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            headers?.ToList().ForEach(h => request.Headers.Add(h.Key, h.Value));
            return _httpClient.SendAsync(request);
        }
        catch (Exception e)
        {
            _logger.Error("Get request to endpoint {endpoint} failed with error: {error}.", endpoint, e);
            throw;
        }
    }

    public Task<HttpResponseMessage> Delete(string endpoint, Dictionary<string, string> headers = null)
    {
        _logger.Debug("Delete request to endpoint {endpoint}.", endpoint);
    
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, endpoint);
            headers?.ToList().ForEach(h => request.Headers.Add(h.Key, h.Value));
            return _httpClient.SendAsync(request);
        }
        catch (Exception e)
        {
            _logger.Error("Delete request to endpoint {endpoint} failed with error: {error}.", endpoint, e);
            throw;
        }
    }

    
    public Task<HttpResponseMessage> Delete(string endpoint, object content, Dictionary<string, string> headers = null, ISerializer serializer = null)
    {
        _logger.Debug("Delete request to endpoint {endpoint}. With payload: {payload}", endpoint, content);
    
        try
        {
            var serializedObject = (serializer ?? _defaultSerializer).Serialize(content);
    
            var request = new HttpRequestMessage(HttpMethod.Delete, endpoint)
            {
                Content = new StringContent(serializedObject, Encoding.UTF8, "application/json")
            };
        
            headers?.ToList().ForEach(h => request.Headers.Add(h.Key, h.Value));
        
            return _httpClient.SendAsync(request);
        }
        catch (Exception e)
        {
            _logger.Error("Delete request to endpoint {endpoint} failed with error: {error}. Body: {@payload}", 
                endpoint, e, content);
            throw;
        }
    }


    #endregion
    
    #region Methods with smart waiters

    public async Task<T> PostAsyncAndWaitFor<T>(
        string endpoint, 
        object payload, Func<T, bool> condition,
        Dictionary<string, string> headers = null,
        int? retries = null, 
        int? timeBetweenRetriesMs = null,
        ISerializer serializer = null,
        IDeserializer deserializer = null)
    {
        if (condition == null) throw new ArgumentNullException(nameof(condition));

        var result = await WaitHelper.WaitFor(() => Post(endpoint, payload, headers, serializer).Deserialize<T>(deserializer),
            condition, retries, timeBetweenRetriesMs);

        if (result is null)
            throw new TimeoutException("Returned data doesn't meet expected condition");
        
        return result;
    }
    
    public Task<HttpResponseMessage> PostAsyncAndWaitForHttpResponse(
        string endpoint, 
        object payload, 
        Func<HttpResponseMessage, bool> condition,
        Dictionary<string, string> headers = null,
        int? retries = null, 
        int? timeBetweenRetriesMs = null,
        ISerializer serializer = null)
    {
        if (condition == null) throw new ArgumentNullException(nameof(condition));
        
        return WaitHelper.WaitFor(() => Post(endpoint, payload, headers, serializer), condition, retries, timeBetweenRetriesMs);
    }

    public async Task<T> GetAsyncAndWaitFor<T>(
        string endpoint, 
        Func<T, bool> condition, 
        Dictionary<string, string> headers = null,
        int? retries= null, 
        int? timeBetweenRetriesMs= null,
        IDeserializer deserializer = null)
    {
        if (condition == null) throw new ArgumentNullException(nameof(condition));
        var result = await WaitHelper.WaitFor(() => Get(endpoint, headers).Deserialize<T>(deserializer),
            condition, retries, timeBetweenRetriesMs);

        if (result is null)
            throw new TimeoutException("Returned data doesn't meet expected condition");
        
        return result;
    }
    
    public Task<HttpResponseMessage> GetAsyncAndWaitForHttpResponse(
        string endpoint, 
        Func<HttpResponseMessage, bool> condition,
        Dictionary<string, string> headers = null,
        int? retries = null, 
        int? timeBetweenRetriesMs = null)
    {
        if (condition == null) throw new ArgumentNullException(nameof(condition));

        return WaitHelper.WaitFor(() => Get(endpoint, headers), condition, retries, timeBetweenRetriesMs);
    }


    public Task<HttpResponseMessage> GetAsyncAndWaitForStatusCode(
        string endpoint, 
        HttpStatusCode expectedCode = HttpStatusCode.OK,
        Dictionary<string, string> headers = null,
        int? retries = null, 
        int? timeBetweenRetriesMs = null) 
        => WaitHelper.WaitFor(() => Get(endpoint, headers), 
            x => x.StatusCode == expectedCode, 
            retries, 
            timeBetweenRetriesMs);


    #endregion

    #region SetUp methods

    public void AddHeader(string key, string value)
    {
        if (!_httpClient.DefaultRequestHeaders.Contains(key))
        {
            _httpClient.DefaultRequestHeaders.Add(key, value);
        }
    }
    
    public void RemoveHeader(string key)
    {
        if (_httpClient.DefaultRequestHeaders.Contains(key))
        {
            _httpClient.DefaultRequestHeaders.Remove(key);
        }
    }
    
    public void RemoveAllHeaders()
    {
        _httpClient.DefaultRequestHeaders.Clear();
    }

    public HttpRequestHeaders ReturnActualHeaders()
    {
        return _httpClient.DefaultRequestHeaders;
    }

    #endregion

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
