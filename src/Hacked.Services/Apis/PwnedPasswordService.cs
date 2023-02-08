using Hacked.Core.Common;
using Hacked.Core.Extensions;
using Hacked.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hacked.Services.Apis;

public class PwnedPasswordService : IPwndPasswordService, IDisposable
{
    private readonly HttpClient client;

    public PwnedPasswordService(HttpClientHandler handler = null)
    {
        if (handler == null)
        {
            handler = new HttpClientHandler();

            if(handler.SupportsAutomaticDecompression)
                handler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
        }

        client = new HttpClient(handler);
        client.BaseAddress = new Uri(HibpConstants.HibpApiBaseAddress);
        client?.DefaultRequestHeaders.Add(HibpConstants.HibpUserAgentKey, HibpConstants.HibpUserAgentValue);
    }

    public async Task<string> CheckPasswordAsync(string password)
    {
        var hashedPassword = password.Hash(); // custom extension method

        var shortHash = hashedPassword.Substring(0, 5);
        
        using var request = new HttpRequestMessage(HttpMethod.Get, $"{HibpConstants.ApiRoute_Range}/{shortHash}");

        using var response = await client.SendAsync(request);
        
        var json = await response.Content.ReadAsStringAsync();

        if (!string.IsNullOrEmpty(json))
        {
            var allHashes = JsonConvert.DeserializeObject<List<string>>(json);

            if (allHashes != null)
            {
                foreach (var hash in allHashes)
                {
                    // Example of a line is "74E73CDBD285D283E7401A044BF08220C75:257"
                    //The first part is the hashed pwd, the second part is how many times it was found in the data set
                    var hashValSplit = hash.Split(':');

                    if (hashValSplit[0] == hashedPassword)
                    {
                        return $"The entered password has been identified {hashValSplit[1]} times in the database.";
                    }
                }
            }
        }
        
        return "That password was not located in the database.";
    }
    
    public void Dispose()
    {
        client?.Dispose();
    }
}
