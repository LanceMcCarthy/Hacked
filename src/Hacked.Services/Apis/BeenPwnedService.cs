using Hacked.Core.Common;
using Hacked.Core.Extensions;
using Hacked.Core.Models;
using Hacked.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hacked.Services.Apis;

public class BeenPwnedService : IPwndBreachService, IDisposable
{
    private readonly HttpClient client;

    public BeenPwnedService(HttpClientHandler handler = null)
    {
        if (handler == null)
        {
            handler = new HttpClientHandler();

            if (handler.SupportsAutomaticDecompression)
                handler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
        }

        client = new HttpClient(handler);
        client.BaseAddress = new Uri(HibpConstants.HibpApiBaseAddress);
        client?.DefaultRequestHeaders.Add(HibpConstants.HibpUserAgentKey, HibpConstants.HibpUserAgentValue);
        client?.DefaultRequestHeaders.Add(HibpConstants.HibpApiHeaderKey, Secrets.HibpApiKey);
    }

    /// <summary>
    /// The API takes a single parameter which is the account to be searched for. 
    /// The account is not case sensitive and will be trimmed of leading or trailing white spaces. The account should always be URL encoded
    /// </summary>
    /// <param name="account">Email address, should always be URL encoded</param>
    /// <param name="truncateResponse">Determine whether only the name of the breach is returned rather than the complete breach data</param>
    /// <returns>A collection of breaches</returns>
    public async Task<ObservableCollection<Breach>> CheckForBreachesAsync(MonitoredAccount account, bool truncateResponse = false)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"{HibpConstants.ApiRoute_BreachedAccount}/{account.Address}?truncateResponse={truncateResponse}");

        var retryCount = 0;

        while (true)
        {
            if (retryCount > 50)
            {
                throw new Exception("[CheckForBreachesAsync] Too many retry attempts, please wait a few minutes and try again.");
            }

            using var response = await client.SendAsync(request);

            switch (response.StatusCode)
            {
                //200 = (GOOD response type, list of breaches available in body)
                case HttpStatusCode.OK:
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ObservableCollection<Breach>>(json);
                //400
                case HttpStatusCode.BadRequest:
                    throw new PwnedApiException("Bad request — the account does not comply with an acceptable format (i.e. it's an empty string)") { StatusCode = response.StatusCode };
                //401
                case HttpStatusCode.Unauthorized:
                    throw new PwnedApiException("Unauthorized — either no API key was provided or it wasn't valid") { StatusCode = response.StatusCode };
                //403
                case HttpStatusCode.Forbidden:
                    throw new PwnedApiException("Forbidden — no user agent has been specified in the request") { StatusCode = response.StatusCode };
                //404 = (GOOD response type, no breaches for the submitted account)
                case HttpStatusCode.NotFound:
                    throw new PwnedApiException("No Breaches") { StatusCode = response.StatusCode };
                //503 = returned by Cloudflare if the API service is broken, or if Cloudflare is forcing an access confirmation for the client's IP
                case HttpStatusCode.ServiceUnavailable:
                    throw new PwnedApiException("Cloudflare unavailable, API service unreachable.") { StatusCode = response.StatusCode };
                case HttpStatusCode.InternalServerError:
                    throw new PwnedApiException("Unknown server error, not on the HIBP expected responses list.") { StatusCode = response.StatusCode };
                default:
                    {
                        if (response.StatusCode.ToString() == "429")
                        {
                            // Elegant back-off using the response's recommended retry delay
                            await response.Headers.RetryAfter.ApplyRetryDelayAsync();
                            retryCount++;
                        }

                        break;
                    }
            }
        }
    }

    /// <summary>
    ///  Gets all breaches in the system.
    /// </summary>
    /// <param name="truncateResponse">Determine whether only the name of the breach is returned rather than the complete breach data</param>
    /// <returns>A collection of breaches</returns>
    public async Task<ObservableCollection<Breach>> GetAllKnownBreachesAsync(bool truncateResponse = false)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"{HibpConstants.ApiRoute_Breaches}?truncateResponse={truncateResponse}");

        var retryCount = 0;

        while (true)
        {
            if (retryCount > 50)
            {
                throw new Exception("[GetAllKnownBreachesAsync] Too many retry attempts, please wait a few minutes and try again.");
            }

            using var response = await client.SendAsync(request);

            switch (response.StatusCode)
            {
                //200 = (GOOD response type, list of breaches available in body)
                case HttpStatusCode.OK:
                    var json = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<ObservableCollection<Breach>>(json);
                //400
                case HttpStatusCode.BadRequest:
                    throw new PwnedApiException("Bad request — the account does not comply with an acceptable format (i.e. it's an empty string)") { StatusCode = response.StatusCode };
                //401
                case HttpStatusCode.Unauthorized:
                    throw new PwnedApiException("Unauthorized — either no API key was provided or it wasn't valid") { StatusCode = response.StatusCode };
                //403
                case HttpStatusCode.Forbidden:
                    throw new PwnedApiException("Forbidden — no user agent has been specified in the request") { StatusCode = response.StatusCode };
                //404 = (GOOD response type, no breaches for the submitted account)
                case HttpStatusCode.NotFound:
                    throw new PwnedApiException("No Breaches") { StatusCode = response.StatusCode };
                //503 = returned by Cloudflare if the API service is broken, or if Cloudflare is forcing an access confirmation for the client's IP
                case HttpStatusCode.ServiceUnavailable:
                    throw new PwnedApiException("Cloudflare unavailable, API service unreachable.") { StatusCode = response.StatusCode };
                case HttpStatusCode.InternalServerError:
                    throw new PwnedApiException("Unknown server error, not on the HIBP expected responses list.") { StatusCode = response.StatusCode };
                default:
                    {
                        if (response.StatusCode.ToString() == "429")
                        {
                            // Elegant back-off using the response's recommended retry delay
                            await response.Headers.RetryAfter.ApplyRetryDelayAsync();
                            retryCount++;
                        }

                        break;
                    }
            }
        }
    }

    /// <summary>
    /// A "data class" is an attribute of a record compromised in a breach. 
    /// For example, many breaches expose data classes such as "Email addresses" and "Passwords". 
    /// The values returned by this service are ordered alphabetically in a string array and will expand over time as new breaches expose previously unseen classes of data.
    /// </summary>
    /// <returns>an array of classes</returns>
    public async Task<List<string>> GetAllKnownDataClassesAsync()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, HibpConstants.ApiRoute_DataClasses);

        var retryCount = 0;

        while (true)
        {
            if (retryCount > 50)
            {
                throw new Exception("[GetAllKnownDataClasses] Too many retry attempts, please wait a few minutes and try again.");
            }

            using var response = await client.SendAsync(request);

            switch (response.StatusCode)
            {
                //200 = (GOOD response type, list of breaches available in body)
                case HttpStatusCode.OK:
                    // Getting DataClasses is a fast, but frequent operation. Make sure we're not intentionally overpowering the API (10 RPS)
                    await Task.Delay(TimeSpan.FromMilliseconds(500));
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<string>>(json);
                //400
                case HttpStatusCode.BadRequest:
                    throw new PwnedApiException("Bad request — the account does not comply with an acceptable format (i.e. it's an empty string)") { StatusCode = response.StatusCode };
                //401
                case HttpStatusCode.Unauthorized:
                    throw new PwnedApiException("Unauthorized — either no API key was provided or it wasn't valid") { StatusCode = response.StatusCode };
                //403
                case HttpStatusCode.Forbidden:
                    throw new PwnedApiException("Forbidden — no user agent has been specified in the request") { StatusCode = response.StatusCode };
                //404 = (GOOD response type, no breaches for the submitted account)
                case HttpStatusCode.NotFound:
                    throw new PwnedApiException("No DataClasses Found") { StatusCode = response.StatusCode };
                //503 = returned by Cloudflare if the API service is broken, or if Cloudflare is forcing an access confirmation for the client's IP
                case HttpStatusCode.ServiceUnavailable:
                    throw new PwnedApiException("Cloudflare unavailable, API service unreachable.") { StatusCode = response.StatusCode };
                case HttpStatusCode.InternalServerError:
                    throw new PwnedApiException("Unknown server error, not on the HIBP expected responses list.") { StatusCode = response.StatusCode };
                default:
                    {
                        if (response.StatusCode.ToString() == "429")
                        {
                            // Elegant back-off using the response's recommended retry delay
                            await response.Headers.RetryAfter.ApplyRetryDelayAsync();
                            retryCount++;
                        }

                        break;
                    }
            }
        }
    }

    /// <summary>
    /// The API takes a single parameter which is the email address to be searched for. 
    /// Unlike searching for breaches, usernames that are not email addresses cannot be searched for. 
    /// The email is not case sensitive and will be trimmed of leading or trailing white spaces. The email should always be URL encoded.
    /// </summary>
    /// <param name="emailAddress">Email address, should always be URL encoded</param>
    /// <param name="truncateResponse">Determine whether only the name of the breach is returned rather than the complete breach data</param>
    /// <returns>A collection of breaches</returns>
    public async Task<ObservableCollection<Breach>> GetPastesAsync(string emailAddress, bool truncateResponse = false)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"{HibpConstants.ApiRoute_BreachedAccount}/{emailAddress}?truncateResponse={truncateResponse}");

        var retryCount = 0;

        while (true)
        {
            if (retryCount > 50)
            {
                throw new Exception("[GetPastesAsync] Too many retry attempts, please wait a few minutes and try again.");
            }

            using var response = await client.SendAsync(request);

            switch (response.StatusCode)
            {
                //200 = (GOOD response type, list of breaches available in body)
                case HttpStatusCode.OK:
                    var json = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<ObservableCollection<Breach>>(json);
                //400
                case HttpStatusCode.BadRequest:
                    throw new PwnedApiException("Bad request — the account does not comply with an acceptable format (i.e. it's an empty string)") { StatusCode = response.StatusCode };
                //401
                case HttpStatusCode.Unauthorized:
                    throw new PwnedApiException("Unauthorized — either no API key was provided or it wasn't valid") { StatusCode = response.StatusCode };
                //403
                case HttpStatusCode.Forbidden:
                    throw new PwnedApiException("Forbidden — no user agent has been specified in the request") { StatusCode = response.StatusCode };
                //404 = (GOOD response type, no breaches for the submitted account)
                case HttpStatusCode.NotFound:
                    throw new PwnedApiException("No Pastes") { StatusCode = response.StatusCode };
                //503 = returned by Cloudflare if the API service is broken, or if Cloudflare is forcing an access confirmation for the client's IP
                case HttpStatusCode.ServiceUnavailable:
                    throw new PwnedApiException("Cloudflare unavailable, API service unreachable.") { StatusCode = response.StatusCode };
                case HttpStatusCode.InternalServerError:
                    throw new PwnedApiException("Unknown server error, not on the HIBP expected responses list.") { StatusCode = response.StatusCode };
                default:
                    {
                        if (response.StatusCode.ToString() == "429")
                        {
                            // Elegant back-off using the response's recommended retry delay
                            await response.Headers.RetryAfter.ApplyRetryDelayAsync();
                            retryCount++;
                        }

                        break;
                    }
            }
        }
    }

    public void Dispose()
    {
        client?.Dispose();
    }
}
