using Hacked.Services.Apis;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Hacked.Tests;

/// <summary>
/// Tests for PwnedPasswordService using a fake HttpClientHandler.
///
/// The service computes a standard hex SHA-1 hash.
/// For "password": hex SHA-1 = "5BAA61E4C9B93F3F0682250B6CF8331B7EE68FD8"
///   prefix = "5BAA6", suffix = "1E4C9B93F3F0682250B6CF8331B7EE68FD8"
/// </summary>
public class PwnedPasswordServiceTests
{
    private const string KnownPassword = "password";
    // Hex SHA-1 suffix of "password" (characters 5–39 of the uppercase hex digest)
    private const string KnownSuffix = "1E4C9B93F3F0682250B6CF8331B7EE68FD8";

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
