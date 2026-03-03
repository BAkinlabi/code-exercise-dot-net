using Microsoft.Extensions.Options;
using System.Text.Json;
using UrlShortenerApi.Controllers;
using UrlShortenerApi.Models;
using UrlShortenerApi.Repositories.Interfaces;

namespace UrlShortenerApi.Repositories
{
    public class FileUrlRepository : IUrlRepository
    {
        private readonly string _filePath;
        private readonly SemaphoreSlim _lock = new(1, 1);
        private readonly ILogger<FileUrlRepository> _logger;

        public FileUrlRepository(IOptions<UrlShortenerServiceOptions> options, ILogger<FileUrlRepository> logger)
        {
            _filePath = options.Value.FilePath ?? "data/json-url-datastore.json";
            _logger = logger;
        }

        private async Task<List<ShortUrl>> LoadAsync()
        {

            try
            {
                if (!File.Exists(_filePath))
                    return new List<ShortUrl>();

                var json = await File.ReadAllTextAsync(_filePath);
                return string.IsNullOrWhiteSpace(json)
                    ? new List<ShortUrl>()
                    : JsonSerializer.Deserialize<List<ShortUrl>>(json) ?? new List<ShortUrl>();
            }
            catch (DirectoryNotFoundException ex)
            {
                _logger.LogError($"Directory not found: {ex.Message}");
                _logger.LogError($"Current directory: {Directory.GetCurrentDirectory()}");
                return new List<ShortUrl>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured while loading the saved shortened urls: {ex.Message}");
                return new List<ShortUrl>();
            }

        }

        private async Task SaveAsync(List<ShortUrl> urls)
        {
            try
            {
                var json = JsonSerializer.Serialize(urls, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_filePath, json);
            }
            catch (IOException ex)
            {
                _logger.LogError($"An I/O error occurred while saving the URLs: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An unexpected error occurred while saving the URLs: {ex.Message}");
            }
        }

        public async Task<ShortUrl?> GetByAliasAsync(string alias)
        {
            await _lock.WaitAsync();
            try
            {
                var urls = await LoadAsync();
                return urls.FirstOrDefault(u => u.Alias == alias);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving the URL by alias: '{alias}': {ex.Message}");
                return null;
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
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred saving the URL '{shortUrl.FullUrl}': {ex.Message}");
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
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred deleting the URL alias: '{alias}': {ex.Message}");
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
