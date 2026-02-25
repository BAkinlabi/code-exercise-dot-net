using UrlShortenerApi.Models;

namespace UrlShortenerApi.Services.interfaces
{
    public interface IUrlShortenerService
    {
        Task<ShortUrl> ShortenUrlAsync(string fullUrl, string customAlias = null);
        Task<ShortUrl> GetByAliasAsync(string alias);
        Task<IEnumerable<ShortUrl>> GetAllAsync();
        Task DeleteAsync(string alias);
    }
}
