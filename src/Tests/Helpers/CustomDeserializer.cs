using GR8Tech.Sport.TestUtils.HttpClientFactory.Abstractions;
using Newtonsoft.Json;

namespace Tests.Helpers;

public class CustomDeserializer : IDeserializer
{
    public T Deserialize<T>(string obj)
    {
        return JsonConvert.DeserializeObject<T>(obj);
    }
}