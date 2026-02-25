namespace UrlShortenerApi.Models
{
    public class ShortUrl
    {
        public string Alias { get; set; } = default!;
        public string FullUrl { get; set; } = default!;
        public string ShortUrlValue { get; set; } = default!;
    }
}
