namespace Tests;

[SetUpFixture]
public class AssemblySetUp
{

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {

    }

    [OneTimeTearDown]
    public void BaseTearDown()
    {
        TestServices.HttpClientFactory?.Dispose();
    }
}