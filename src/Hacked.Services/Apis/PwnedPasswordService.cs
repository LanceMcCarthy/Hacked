using Hacked.Core.Common;
using Hacked.Services.Interfaces;
using System;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Hacked.Services.Apis;

public class PwnedPasswordService : IPwndPasswordService, IDisposable
{
    private readonly HttpClient client;

    public PwnedPasswordService(HttpClientHandler? handler = null)
    {
        if (handler == null)
        {
            handler = new HttpClientHandler();

            if(handler.SupportsAutomaticDecompression)
                handler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
        }

        client = new HttpClient(handler);
        client.BaseAddress = new Uri(HibpConstants.PwnedPasswordsApiBaseAddress);
        client.DefaultRequestHeaders.Add(HibpConstants.HibpUserAgentKey, HibpConstants.HibpUserAgentValue);
    }

    public async Task<string> CheckPasswordAsync(string password)
    {
        // HIBP k-anonymity model: compute hex SHA-1, send only first 5 chars, compare suffixes in response
        using var sha1 = SHA1.Create();
        var hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(password));
        var hashedPassword = BitConverter.ToString(hashBytes).Replace("-", "").ToUpperInvariant();
        var hashPrefix = hashedPassword.Substring(0, 5);
        var hashSuffix = hashedPassword.Substring(5);

        using var request = new HttpRequestMessage(HttpMethod.Get, $"{HibpConstants.ApiRoute_Range}/{hashPrefix}");
        using var response = await client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            return "Unable to check the password at this time. Please try again later.";

        // Response is plain text: HASH_SUFFIX:COUNT per line (NOT JSON)
        var responseText = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrEmpty(responseText))
            return "Good news — this password was not found in any known data breaches!";

        foreach (var line in responseText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
        {
            var colonIndex = line.IndexOf(':');
            if (colonIndex < 0) continue;

            var lineSuffix = line.Substring(0, colonIndex).Trim();
            if (!lineSuffix.Equals(hashSuffix, StringComparison.OrdinalIgnoreCase)) continue;

            if (int.TryParse(line.Substring(colonIndex + 1).Trim(), out var count))
                return $"⚠️ This password has appeared {count:N0} time(s) in known data breaches. Please use a different password.";
        }

        return "✅ Good news — this password was not found in any known data breaches!";
    }
    
    public void Dispose()
    {
        client.Dispose();
    }
}

