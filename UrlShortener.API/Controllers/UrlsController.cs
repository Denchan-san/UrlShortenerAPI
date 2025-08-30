using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.DTOs;
using UrlShortener.Application.Exceptions;
using UrlShortener.Application.Services;

namespace UrlShortener.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UrlsController : ControllerBase
    {
        private readonly UrlService _urlService;

        public UrlsController(UrlService urlService)
        {
            _urlService = urlService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var urls = await _urlService.GetAllAsync();
            return Ok(urls);
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var url = await _urlService.GetByCodeAsync(code);
            if (url == null)
                return NotFound(new { message = $"URL with code '{code}' not found." });

            return Ok(url);
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Create([FromBody] CreateUrlDto dto)
        {
            var userName = User.Identity?.Name ?? "anonymous";

            try
            {
                var url = await _urlService.CreateAsync(dto, userName);
                return CreatedAtAction(nameof(GetByCode), new { code = url.ShortenUrl }, url);
            }
            catch (UrlAlreadyExistsException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpDelete("byCode/{code}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> DeleteByCode(string code)
        {
            await _urlService.DeleteByCodeAsync(code);
            return NoContent();
        }

        [HttpDelete("byId/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteById(int id)
        {
            await _urlService.DeleteAtIdAsync(id);
            return NoContent();
        }
    }
}
