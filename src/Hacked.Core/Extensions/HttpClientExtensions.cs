using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Hacked.Core.Extensions;

public static class HttpClientExtensions
{
    /// <summary>
    /// Automatically reads the retry-after header and applies a Task.Delay
    /// </summary>
    /// <param name="retryAfter">The Retry Condition Header from an HTTP response with a 429 error</param>
    /// <returns></returns>
    public static async Task ApplyRetryDelayAsync(this RetryConditionHeaderValue retryAfter)
    {
        var retryDelay = TimeSpan.FromSeconds(2).Milliseconds;

        // Use the value from the header
        if (retryAfter.Delta.HasValue)
        {
            retryDelay = retryAfter.Delta.Value.Milliseconds;
        }
        else if (retryAfter.Date.HasValue)
        {
            // backup option, try to calculate our own delta
            var delta = retryAfter.Date.Value.ToUniversalTime() - DateTimeOffset.UtcNow;
            retryDelay = delta.Milliseconds;
        }

        // Wait the recommended delay
        await Task.Delay(retryDelay);
    }
}