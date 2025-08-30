using UrlShortener.Application.DTOs;
using UrlShortener.Application.Exceptions;
using UrlShortener.Domain.Interfaces;
using UrlShortener.Domain.Models;

namespace UrlShortener.Application.Services
{
    public class UrlService
    {
        private readonly IUrlRepository _urlRepository;

        public UrlService(IUrlRepository urlRepository)
        {
            _urlRepository = urlRepository;
        }

        public async Task<ShortUrl> CreateAsync(CreateUrlDto dto, string userName)
        {
            var existingUrl = (await _urlRepository.GetAllAsync())
                                .FirstOrDefault(u => u.OriginalUrl == dto.OriginalUrl);

            if (existingUrl != null)
                throw new UrlAlreadyExistsException("Такий URL вже існує.");

            var shortenUrl = Guid.NewGuid().ToString("N")[..6];

            var shortUrl = new ShortUrl
            {
                OriginalUrl = dto.OriginalUrl,
                ShortenUrl = shortenUrl,
                CreatedBy = userName,
                CreatedAt = DateTime.UtcNow
            };

            await _urlRepository.AddAsync(shortUrl);
            return shortUrl;
        }


        public async Task DeleteAtIdAsync(int id)
        {
            await _urlRepository.DeleteAsync(id);
        }

        public async Task DeleteByCodeAsync(string code)
        {
            var url = await _urlRepository.GetByCodeAsync(code);
            if (url == null) throw new KeyNotFoundException("URL не знайдено");

            await _urlRepository.DeleteAsync(url.Id);
        }


        public async Task<IEnumerable<ShortUrl>> GetAllAsync()
        {
            return await _urlRepository.GetAllAsync();
        }

        public async Task<ShortUrl> GetByCodeAsync(string code)
        {
            return await _urlRepository.GetByCodeAsync(code);
        }
    }
}
