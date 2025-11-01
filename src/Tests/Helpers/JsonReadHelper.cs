using Newtonsoft.Json;

namespace Tests.Helpers;

public static class JsonReadHelper
{
    public static T GetJsonDataFromFile<T>(string filePath)
        => (T)new JsonSerializer().Deserialize(File.OpenText(@$"{filePath}"), typeof(T));
}