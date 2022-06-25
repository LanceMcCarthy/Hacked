using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Hacked.Services.Apis;

public class PwnedPasswordService : IDisposable
{
    private HttpClient client;
    private readonly HttpClientHandler handler;
    private DateTime lastCalled;

    public PwnedPasswordService(HttpClientHandler handler = null)
    {
        if (handler != null)
            this.handler = handler;

        ValidateClient();
    }

    public async Task<string> CheckPasswordAsync(string password)
    {
        // https://haveibeenpwned.com/API/v3
        string shortHash = "";
        string fullHash = "";

        using (SHA1Managed sha1 = new SHA1Managed())
        {
            var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(password));
            var sb = new StringBuilder(hash.Length * 2);

            foreach (byte b in hash)
            {
                // can be "x2" if you want lowercase
                sb.Append(b.ToString("X2"));
            }

            fullHash = sb.ToString();
            shortHash = fullHash.Substring(0, 5);
        }

        ValidateClient();
        await ValidateRequestDelayAsync();

        using (var request = new HttpRequestMessage(HttpMethod.Get, $"range/{shortHash}"))
        using (var response = await client.SendAsync(request))
        {
            lastCalled = DateTime.UtcNow;

            var json = await response.Content.ReadAsStringAsync();
            var allHashes = JsonConvert.DeserializeObject<List<string>>(json);

            foreach (var hash in allHashes)
            {
                // Example of a line is "74E73CDBD285D283E7401A044BF08220C75:257"
                //The first part is the hashed pwd, the second part is how many times it was found in the data set
                var hashValSplit = hash.Split(':');

                if (hashValSplit[0] == fullHash)
                {
                    return $"The entered password has been identified {hashValSplit[1]} times in the database.";
                }
            }

            return $"The entered password was not in the database.";
        }
    }

    private void ValidateClient()
    {
        if (client != null)
            return;

        // If we're passed a handler, use it to instantiate the client. Otherwise, don't use one.
        client = handler != null ? new HttpClient(handler) : new HttpClient();

        client.BaseAddress = new Uri("https://api.pwnedpasswords.com/");
        client?.DefaultRequestHeaders.Add("User-Agent", "Hacked-for-Windows-Universal");
    }

    /// <summary>
    /// This method determines if the next call to HIBP needs to be delayed
    /// Logic - If less than 1500ms has elapsed, delay the call until 1500ms has elapsed
    /// </summary>
    /// <returns></returns>
    private async Task ValidateRequestDelayAsync()
    {
        var timeElapsedSinceLastCall = DateTime.UtcNow - lastCalled;

        if (timeElapsedSinceLastCall < TimeSpan.FromMilliseconds(1500))
        {
            var timeNeededToWait = TimeSpan.FromMilliseconds(1500) - timeElapsedSinceLastCall;

            // Delay the call until 1.5 seconds has elapsed
            await Task.Delay(timeNeededToWait);
        }
    }

    public void Dispose()
    {
        client?.Dispose();
    }
}
