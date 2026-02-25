namespace UrlShortenerApi.Models
{
    public class ShortenUrlRequest
    {
        public string FullUrl { get; set; } = default!;
        public string? CustomAlias { get; set; }
    }

}
