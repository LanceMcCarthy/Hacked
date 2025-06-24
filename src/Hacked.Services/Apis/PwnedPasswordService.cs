using CommonHelpers.Extensions;
using Hacked.Core.Common;
using Hacked.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
        // extension method from CommonHelpers.Extensions, uses SHA1Managed as required by HIBP API
        var hashedPassword = password.Hash(); 

        var shortHash = hashedPassword.Substring(0, 5);
        
        using var request = new HttpRequestMessage(HttpMethod.Get, $"{HibpConstants.ApiRoute_Range}/{shortHash}");

        using var response = await client.SendAsync(request);
        
        var json = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrEmpty(json)) 
            return "That password was not located in the database.";

        var allHashes = JsonConvert.DeserializeObject<List<string>>(json);

        if (allHashes == null) 
            return "There was a problem deserializing the hashes.";

        // A dictionary to hold all data set matches (instead of only returning the first match)
        var matchingHashes = new Dictionary<string, int>();

        // Example response =>
        // "74E73CDBD285D283E7401A044BF08220C75:257"
        // The first part is the hashed pwd, ex: 74E73CDBD285D283E7401A044BF08220C75
        // The second part is how many times it was found in the data set, ex: 257

        // We split the string by ':' and check if the first part matches the hashed password, if it matches, add to the dictionary
        foreach (var hashValSplit in allHashes
                     .Select(hash => hash.Split(':'))
                     .Where(hashValSplit => hashValSplit[0] == hashedPassword))
        {
            if (int.TryParse(hashValSplit[1], out var count))
            {
                matchingHashes.Add(hashValSplit[0], count);
            }
            else
            {
                return "There was a problem parsing the count of occurrences.";
            }
        }

        if (matchingHashes.Count == 0)
            return "That password was not located in the database.";

        var result = "The entered password has been identified in the database:\n";
        foreach (var hash in matchingHashes) 
            result += $"{hash.Key} - {hash.Value} times\n";
        result += "Please consider using a different password.";
        return result;
    }
    
    public void Dispose()
    {
        client?.Dispose();
    }
}
