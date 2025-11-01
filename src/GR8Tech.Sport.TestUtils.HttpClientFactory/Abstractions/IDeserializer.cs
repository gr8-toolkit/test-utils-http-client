namespace GR8Tech.Sport.TestUtils.HttpClientFactory.Abstractions;

public interface IDeserializer
{
    public T Deserialize<T>(string obj);
}