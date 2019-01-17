using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Hacked.Core.Common;
using Hacked.Core.Models;
using Newtonsoft.Json;

namespace Hacked.Services.Apis
{
    public class BeenPwnedService : IDisposable
    {
        private HttpClient client;
        private readonly HttpClientHandler handler;
        private DateTime lastCalled;

        public BeenPwnedService(HttpClientHandler handler = null)
        {
            if (handler != null)
                this.handler = handler;

            ValidateClient();
        }

        /// <summary>
        /// The API takes a single parameter which is the account to be searched for. 
        /// The account is not case sensitive and will be trimmed of leading or trailing white spaces. The account should always be URL encoded
        /// </summary>
        /// <param name="account">Email address, should always be URL encoded</param>
        /// <returns></returns>
        public async Task<ObservableCollection<Breach>> CheckForBreachesAsync(MonitoredAccount account)
        {
            ValidateClient();

            await ValidateRequestDelayAsync();

            using (var request = new HttpRequestMessage(HttpMethod.Get, $"https://haveibeenpwned.com/api/v2/breachedaccount/{account.Address}"))
            {
                using (var response = await client.SendAsync(request))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        Debug.WriteLine("Check for breaches - request made");

                        lastCalled = DateTime.UtcNow;

                        var json = await response.Content.ReadAsStringAsync();

                        Debug.WriteLine($"Check for breaches - json:\r\n\n{json}\r\n\n");

                        var result = JsonConvert.DeserializeObject<ObservableCollection<Breach>>(json);

                        account.LastUpdated = DateTime.Now;

                        Debug.WriteLine($"Check for breaches - JSON.NET result:\r\n\n{result}\r\n\n");

                        return result;
                    }
                    else
                    {
                        throw new PwnedApiException("HttpException Calling API Service") { StatusCode = response.StatusCode };
                    }
                }
            }
        }

        /// <summary>
        ///  Gets all breaches in the system.
        /// </summary>
        /// <returns></returns>
        public async Task<ObservableCollection<Breach>> GetAllKnownBreachesAsync()
        {
            ValidateClient();
            await ValidateRequestDelayAsync();

            using (var request = new HttpRequestMessage(HttpMethod.Get, "https://haveibeenpwned.com/api/v2/breaches"))
            using (var response = await client.SendAsync(request))
            {
                lastCalled = DateTime.UtcNow;

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ObservableCollection<Breach>>(json);
                
                return result;
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
            ValidateClient();
            await ValidateRequestDelayAsync();

            using (var request = new HttpRequestMessage(HttpMethod.Get, "https://haveibeenpwned.com/api/v2/dataclasses"))
            using (var response = await client.SendAsync(request))
            {
                lastCalled = DateTime.UtcNow;

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<string>>(json);

                //Needed to align with HIBP new policy of 1.5 seconds between calls
                await Task.Delay(TimeSpan.FromMilliseconds(1500));

                return result;
            }
        }

        /// <summary>
        /// The API takes a single parameter which is the email address to be searched for. 
        /// Unlike searching for breaches, usernames that are not email addresses cannot be searched for. 
        /// The email is not case sensitive and will be trimmed of leading or trailing white spaces. The email should always be URL encoded.
        /// </summary>
        /// <param name="emailaddress">Email address, should always be URL encoded</param>
        /// <returns></returns>
        public async Task<ObservableCollection<Breach>> GetPastesAsync(string emailAddress)
        {
            ValidateClient();
            await ValidateRequestDelayAsync();

            using (var request = new HttpRequestMessage(HttpMethod.Get, $"https://haveibeenpwned.com/api/v2/breachedaccount/{emailAddress}"))
            using (var response = await client.SendAsync(request))
            {
                lastCalled = DateTime.UtcNow;

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ObservableCollection<Breach>>(json);
                
                return result;
            }
        }

        private void ValidateClient()
        {
            if (client == null && handler == null) //default usage
            {
                client = new HttpClient();
            }
            else if (client == null && handler != null) //usage when an HttpClientHandler is passed
            {
                client = new HttpClient(handler);
            }

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
            Debug.WriteLine($"BeenPwndService - timeElapsedSinceLastCall: {timeElapsedSinceLastCall}");

            if (timeElapsedSinceLastCall < TimeSpan.FromMilliseconds(1500))
            {
                var timeNeededToWait = TimeSpan.FromMilliseconds(1500) - timeElapsedSinceLastCall;
                Debug.WriteLine($"BeenPwndService - timeNeededToWait: {timeNeededToWait}");

                // Delay the call until 1.5 seconds has elapsed
                await Task.Delay(timeNeededToWait);
            }
        }

        public void Dispose()
        {
            client?.Dispose();
        }
    }
}
