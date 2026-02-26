using Moq;
using UrlShortenerApi.Repositories.Interfaces;
using UrlShortenerApi.Services;
using UrlShortenerApi.Models;
using Microsoft.Extensions.Options;

namespace UrlShortenerApiTest
{
    public class UrlShortenerServiceTests
    {
        private readonly Mock<IUrlRepository> _urlRepositoryMock;
        private readonly UrlShortenerService _urlShortenerService;
        private readonly string _baseUrl = "http://localhost:8080/";

        public UrlShortenerServiceTests()
        {
            _urlRepositoryMock = new Mock<IUrlRepository>();
            var optionsMock = new Mock<IOptions<UrlShortenerServiceOptions>>();
            optionsMock.Setup(o => o.Value).Returns(new UrlShortenerServiceOptions { BaseUrl = _baseUrl });
            _urlShortenerService = new UrlShortenerService(_urlRepositoryMock.Object, optionsMock.Object);
        }

        [Fact]
        public async Task ShortenUrlAsync_ValidUrl_ReturnsShortUrl()
        {
            // Arrange
            string fullUrl = "http://mytestwebsite.com";
            string alias = "test321";
            _urlRepositoryMock.Setup(repo => repo.ExistsAsync(alias)).ReturnsAsync(false);
            _urlRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<ShortUrl>())).Returns(Task.CompletedTask);

            // Act
            var result = await _urlShortenerService.ShortenUrlAsync(fullUrl, alias);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(fullUrl, result.FullUrl);
            Assert.Equal(_baseUrl + alias, result.ShortUrlValue);
        }

        [Fact]
        public async Task ShortenUrlAsync_InvalidUrl_ThrowsArgumentException()
        {
            // Arrange
            string invalidUrl = "invalid-url";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _urlShortenerService.ShortenUrlAsync(invalidUrl));
        }

        [Fact]
        public async Task ShortenUrlAsync_AliasTaken_ThrowsInvalidOperationException()
        {
            // Arrange
            string fullUrl = "http://mytestwebsite.com";
            string alias = "test321";
            _urlRepositoryMock.Setup(repo => repo.ExistsAsync(alias)).ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _urlShortenerService.ShortenUrlAsync(fullUrl, alias));
        }
 
    }
}