using UrlShortenerApi.Models;

namespace UrlShortenerApi.Repositories.Interfaces
{
    public interface IUrlRepository
    {
        Task<ShortUrl?> GetByAliasAsync(string alias);
        Task<IEnumerable<ShortUrl>> GetAllAsync();
        Task AddAsync(ShortUrl shortUrl);
        Task DeleteAsync(string alias);
        Task<bool> ExistsAsync(string alias);
    }

}
