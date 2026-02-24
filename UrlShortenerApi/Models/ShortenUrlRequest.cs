namespace UrlShortenerApi.ModelDTOs
{
    public class ShortenUrlRequest
    {
        public string FullUrl { get; set; } = default!;
        public string? CustomAlias { get; set; }
    }

}
