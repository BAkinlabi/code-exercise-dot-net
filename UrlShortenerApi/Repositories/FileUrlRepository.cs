using Microsoft.Extensions.Options;
using System.Text.Json;
using UrlShortenerApi.Models;
using UrlShortenerApi.Repositories.Interfaces;

namespace UrlShortenerApi.Repositories
{
    public class FileUrlRepository : IUrlRepository
    {
        private readonly string _filePath;
        private readonly SemaphoreSlim _lock = new(1, 1);

        public FileUrlRepository(IOptions<UrlShortenerServiceOptions> options)
        {
            _filePath = options.Value.FilePath ?? "json-url-datastore.json";
        }

        private async Task<List<ShortUrl>> LoadAsync()
        {
            if (!File.Exists(_filePath))
                return new List<ShortUrl>();

            var json = await File.ReadAllTextAsync(_filePath);
            return string.IsNullOrWhiteSpace(json)
                ? new List<ShortUrl>()
                : JsonSerializer.Deserialize<List<ShortUrl>>(json) ?? new List<ShortUrl>();
        }

        private async Task SaveAsync(List<ShortUrl> urls)
        {
            var json = JsonSerializer.Serialize(urls, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_filePath, json);
        }

        public async Task<ShortUrl?> GetByAliasAsync(string alias)
        {
            await _lock.WaitAsync();
            try
            {
                var urls = await LoadAsync();
                return urls.FirstOrDefault(u => u.Alias == alias);
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task<IEnumerable<ShortUrl>> GetAllAsync()
        {
            await _lock.WaitAsync();
            try
            {
                return await LoadAsync();
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task AddAsync(ShortUrl shortUrl)
        {
            await _lock.WaitAsync();
            try
            {
                var urls = await LoadAsync();
                urls.Add(shortUrl);
                await SaveAsync(urls);
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task DeleteAsync(string alias)
        {
            await _lock.WaitAsync();
            try
            {
                var urls = await LoadAsync();
                var existing = urls.FirstOrDefault(u => u.Alias == alias);
                if (existing != null)
                {
                    urls.Remove(existing);
                    await SaveAsync(urls);
                }
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task<bool> ExistsAsync(string alias)
        {
            await _lock.WaitAsync();
            try
            {
                var urls = await LoadAsync();
                return urls.Any(u => u.Alias == alias);
            }
            finally
            {
                _lock.Release();
            }
        }
    }

}
