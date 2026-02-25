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

        [Fact]
        public async Task GetByAliasAsync_ValidAlias_ReturnsShortUrl()
        {
            // Arrange
            string alias = "test321";
            var expectedShortUrl = new ShortUrl { Alias = alias, FullUrl = "http://mytestwebsite.com", ShortUrlValue = _baseUrl + alias };
            _urlRepositoryMock.Setup(repo => repo.GetByAliasAsync(alias)).ReturnsAsync(expectedShortUrl);

            // Act
            var result = await _urlShortenerService.GetByAliasAsync(alias);

            // Assert
            Assert.Equal(expectedShortUrl, result);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllShortUrls()
        {
            // Arrange
            var shortUrls = new List<ShortUrl>
                {
                new ShortUrl { Alias = "test555", FullUrl = "http://mytest1website.com", ShortUrlValue = _baseUrl + "test555" },
                new ShortUrl { Alias = "test321", FullUrl = "http://mytest2website.com", ShortUrlValue = _baseUrl + "test321" },
                new ShortUrl { Alias = "test456", FullUrl = "http://mytestwebsite.co.uk", ShortUrlValue = _baseUrl + "test456" }
                };
            _urlRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(shortUrls);

            // Act
            var result = await _urlShortenerService.GetAllAsync();

            // Assert
            Assert.Equal(shortUrls.Count, result.Count());
        }

        [Fact]
        public async Task DeleteAsync_ValidAlias_CallsRepositoryDelete()
        {
            // Arrange
            string alias = "test321";

            // Act
            await _urlShortenerService.DeleteAsync(alias);

            // Assert
            _urlRepositoryMock.Verify(repo => repo.DeleteAsync(alias), Times.Once);
        }
    }
}