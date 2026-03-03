using Microsoft.AspNetCore.Mvc;
using UrlShortenerApi.Services.interfaces;
using UrlShortenerApi.Models;

namespace UrlShortenerApi.Controllers
{
    [ApiController]
    [Route("/")]
    public class UrlShortenerController : ControllerBase
    {
        private readonly IUrlShortenerService _service;
        private readonly ILogger<UrlShortenerController> _logger;

        public UrlShortenerController(IUrlShortenerService service, ILogger<UrlShortenerController> logger)
        { 
            _service = service;
            _logger = logger;
        } 

        /// <summary>
        /// Accepts a full url and saves it with an alias. 
        /// An alias is auto generated if none supplied.
        /// </summary>
        /// <param name="fullurl">fullurl</param>
        /// <param name="CustomAlias">CustomAlias</param>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/shorten
        ///       {
        ///          "fullUrl": "https://test1/example.com",
        ///          "customAlias": "test1"
        ///       }
        ///     
        /// </remarks>
        /// <response code="201">URL successfully shortened</response>
        /// <response code="400">Invalid input or alias already taken</response>
        #region Annotation
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [Produces("application/json")]
        #endregion
        [HttpPost("shorten")]
        public async Task<IActionResult> Shorten([FromBody] ShortenUrlRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.FullUrl))
                return BadRequest("fullUrl is required");

            if (!Uri.IsWellFormedUriString(req.FullUrl, UriKind.Absolute))
                return BadRequest("Invalid URL format");

            try
            {
                var result = await _service.ShortenUrlAsync(req.FullUrl, req.CustomAlias);
                return Created("", new { shortUrl = result.ShortUrlValue });
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid URL: {FullUrl}", req.FullUrl);
                return BadRequest("Invalid URL");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Alias already taken: {CustomAlias}", req.CustomAlias);
                return BadRequest("Alias already taken");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while shortening the URL.");
                return StatusCode(500, "An unexpected error occurred");
            }
        }

        /// <summary>
        /// Accepts an alias and redirect to its full URL 
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/{alias}
        ///     
        /// </remarks>
        /// <returns>Redirect to full URL</returns>
        /// <response code="302">Redirect to the original URL</response>
        /// <response code="404">Alias not found</response>
        #region Annotation
        [ProducesResponseType(302)]
        [ProducesResponseType(404)]
        [Produces("application/json")]
        #endregion
        [HttpGet("{alias}")]
        public async Task<IActionResult> RedirectToFullUrl(string alias)
        {
            try
            {
                var url = await _service.GetByAliasAsync(alias);
                if (url == null) return NotFound("Alias not found");
                return Redirect(url.FullUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while redirecting to the full URL.");
                return StatusCode(500, "An error occurred while redirecting to the full URL");
            }
        }

        /// <summary>
        /// Deletes a shortened URL 
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/{alias}
        ///     
        /// </remarks>
        /// <returns>Redirect to full URL</returns>
        /// <response code="204">Successfully deleted</response>
        /// <response code="404">Alias not found</response>
        #region Annotation
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [Produces("application/json")]
        #endregion
        [HttpDelete("{alias}")]
        public async Task<IActionResult> Delete(string alias)
        {
            try
            {
                var url = await _service.GetByAliasAsync(alias);
                if (url == null) return NotFound();
                await _service.DeleteAsync(alias);
                return NoContent(); // No content to return, indicating successful deletion
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the URL.");
                return StatusCode(500, "An error occurred while deleting the URL");
            }
        }

        /// <summary>
        /// Gets a list of all shorten urls 
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /api/urls
        ///     
        /// </remarks>
        /// <returns>Room all saves shorten url data</returns>
        /// <response code="200">Returns url data</response>
        #region Annotation
        [ProducesResponseType(typeof(string), 200)]
        [Produces("application/json")]
        #endregion
        [HttpGet("urls")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var urls = await _service.GetAllAsync();
                return Ok(urls.Select(u => new { u.Alias, u.FullUrl, shortUrl = u.ShortUrlValue }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the URL.");
                return StatusCode(500, "An error occurred while retrieving the URLs");
            }
        }
    }
}
