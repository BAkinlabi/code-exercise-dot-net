using Microsoft.Extensions.Options;
using UrlShortenerApi.Models;
using UrlShortenerApi.Repositories.Interfaces;
using UrlShortenerApi.Services.interfaces;

namespace UrlShortenerApi.Services
{
    public class UrlShortenerService : IUrlShortenerService
    {
        private readonly IUrlRepository _urlRepository;
        private readonly string _baseUrl;

        public UrlShortenerService(IUrlRepository urlRepository, IOptions<UrlShortenerServiceOptions> options)
        { 
           _urlRepository = urlRepository;
            _baseUrl = options.Value.BaseUrl ?? "http://localhost:7102/";
        }
        public async Task<ShortUrl> ShortenUrlAsync(string fullUrl, string customAlias = null)
        {
            if (!Uri.TryCreate(fullUrl, UriKind.Absolute, out _))
                throw new ArgumentException("Invalid URL");

            string alias = customAlias ?? GenerateRandomAlias();
            if (await _urlRepository.ExistsAsync(alias))
                throw new InvalidOperationException("Alias already taken");

            var shortUrl = new ShortUrl
            {
                Alias = alias,
                FullUrl = fullUrl,
                ShortUrlValue = _baseUrl + alias
            };
            await _urlRepository.AddAsync(shortUrl);
            return shortUrl;
        }

        public Task<ShortUrl> GetByAliasAsync(string alias) => _urlRepository.GetByAliasAsync(alias);
        public Task<IEnumerable<ShortUrl>> GetAllAsync() => _urlRepository.GetAllAsync();
        public Task DeleteAsync(string alias) => _urlRepository.DeleteAsync(alias);

        private string GenerateRandomAlias()
        {
            return Guid.NewGuid().ToString("N")[..6];
        }
    }
}
