using GR8Tech.Sport.TestUtils.HttpClientFactory.Configurations;
using GR8Tech.Sport.TestUtils.HttpClientFactory.Configurations.Options;
using Microsoft.Extensions.Configuration;
using Polly;
using static Polly.Policy;

namespace GR8Tech.Sport.TestUtils.HttpClientFactory.Helpers;

internal static class WaitHelper
{
    private static PollingSettings? _pollingSettings { get; }

    static WaitHelper()
    {
        _pollingSettings = SettingsProvider.Configuration.Get<Configuration>()?.HttpFactorySettings?.PollingSettings 
                           ?? new PollingSettings { Retries = 60, TimeBetweenRetriesMs = 1000 };
    }

    internal static Task<T> WaitFor<T>(this Func<Task<T>> func, Func<T, bool> repeatCondition, int? retries, int? timeBetweenRetriesMs)
        =>  Handle<Exception>()
            .OrResult(repeatCondition.Invert())
            .WaitAndRetryAsync(retries ?? _pollingSettings.Retries, i => TimeSpan.FromMilliseconds(timeBetweenRetriesMs ?? _pollingSettings.TimeBetweenRetriesMs))
            .ExecuteAsync(func.Invoke);

    private static Func<T, bool> Invert<T>(this Func<T, bool> predicate)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        return x => !predicate(x);
    }
}
