using Hacked.Services.Apis;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Hacked.Tests;

/// <summary>
/// Tests for PwnedPasswordService using a fake HttpClientHandler.
/// 
/// The service uses CommonHelpers Hash() which computes a decimal SHA1.
/// For "password": decimal SHA1 = "91170972282011856363613037111082485127126230143216"
///   prefix = "91170", suffix = "972282011856363613037111082485127126230143216"
/// </summary>
public class PwnedPasswordServiceTests
{
    private const string KnownPassword = "password";
    // Decimal SHA1 suffix of "password" (computed via CommonHelpers.StringExtensions.Hash())
    private const string KnownSuffix = "972282011856363613037111082485127126230143216";

    private class FakeHttpClientHandler : HttpClientHandler
    {
        private readonly string _responseBody;
        private readonly HttpStatusCode _statusCode;

        public FakeHttpClientHandler(string responseBody, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            _responseBody = responseBody;
            _statusCode = statusCode;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
            => Task.FromResult(new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(_responseBody)
            });
    }

    [Test]
    public async Task CheckPasswordAsync_WhenSuffixMatchWithCount_ReturnsBreachWarning()
    {
        // Arrange: response contains matching suffix with count > 0
        var handler = new FakeHttpClientHandler($"{KnownSuffix}:5\r\n");
        var service = new PwnedPasswordService(handler);

        // Act
        var result = await service.CheckPasswordAsync(KnownPassword);

        // Assert: should indicate the password was found in breaches
        result.Should().Contain("breaches");
    }

    [Test]
    public async Task CheckPasswordAsync_WhenNoSuffixMatch_ReturnsGoodNews()
    {
        // Arrange: response does not contain the matching suffix
        var handler = new FakeHttpClientHandler("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA:3\r\nBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB:1\r\n");
        var service = new PwnedPasswordService(handler);

        // Act
        var result = await service.CheckPasswordAsync(KnownPassword);

        // Assert: should indicate the password was NOT found
        result.Should().Contain("Good news");
    }

    [Test]
    public async Task CheckPasswordAsync_WhenApiUnavailable_ReturnsErrorMessage()
    {
        // Arrange: server returns 503
        var handler = new FakeHttpClientHandler(string.Empty, HttpStatusCode.ServiceUnavailable);
        var service = new PwnedPasswordService(handler);

        // Act
        var result = await service.CheckPasswordAsync(KnownPassword);

        // Assert: graceful error message
        result.Should().Contain("Unable to check");
    }

    [Test]
    public async Task CheckPasswordAsync_WhenEmptyResponse_ReturnsGoodNews()
    {
        // Arrange: empty response body
        var handler = new FakeHttpClientHandler(string.Empty);
        var service = new PwnedPasswordService(handler);

        // Act
        var result = await service.CheckPasswordAsync(KnownPassword);

        // Assert: empty body treated as not found
        result.Should().Contain("Good news");
    }
}
