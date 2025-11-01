using System.Text.Json;
using GR8Tech.Sport.TestUtils.HttpClientFactory.Abstractions;
using GR8Tech.Sport.TestUtils.HttpClientFactory.Configurations;
using GR8Tech.Sport.TestUtils.HttpClientFactory.Configurations.Options;
using Microsoft.Extensions.Configuration;

namespace GR8Tech.Sport.TestUtils.HttpClientFactory.Implementation;

internal class Serializer : ISerializer, IDeserializer
{
    private readonly SerializerOptions? _serializerOptions;
    
    private readonly JsonSerializerOptions _serializerOptionsOptions;

    internal Serializer()
    {
        _serializerOptions = SettingsProvider.Configuration.Get<Configuration>()?.HttpFactorySettings?.SerializerOptions;
        
        _serializerOptionsOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = _serializerOptions != null ? _serializerOptions.PropertyNameCaseInsensitive : true
        };
    }

    public T Deserialize<T>(string jsonString)
    {
        try
        {
            var result = JsonSerializer.Deserialize<T>(jsonString, _serializerOptionsOptions);
            if (result == null) throw new ApplicationException("Unable to deserialize Json string to:" + typeof(T));

            return result;
        }
        catch (Exception e)
        {
            throw new ApplicationException($"Unable to deserialize JSON string {typeof(T)}. Exception {e}");
        }
    }

    public string Serialize(object input)
    {
        try
        {
            var result = JsonSerializer.Serialize(input, _serializerOptionsOptions);

            return result;
        }
        catch (Exception e)
        {
            throw new ApplicationException($"Unable to serialize {input}. Exception {e}");
        }
    }
    
}