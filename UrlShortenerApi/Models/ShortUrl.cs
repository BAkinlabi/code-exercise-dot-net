namespace UrlShortenerApi.Models
{
    public class ShortUrl
    {
        public string Alias { get; set; } = default!;
        public string FullUrl { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
