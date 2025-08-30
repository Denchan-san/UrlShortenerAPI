using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Domain.Interfaces;
using UrlShortener.Domain.Models;
using UrlShortener.Infrastructure.Data;

namespace UrlShortener.Infrastructure.Repositories
{
    public class UrlRepository : IUrlRepository
    {
        private readonly AppDbContext _context;

        public UrlRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ShortUrl url)
        {
            _context.ShortUrls.Add(url);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.ShortUrls.FindAsync(id);
            if(entity != null)
            {
                _context.ShortUrls.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ShortUrl>> GetAllAsync()
        {
            return await _context.ShortUrls.ToListAsync();
        }

        public async Task<ShortUrl> GetByCodeAsync(string code)
        {
            return await _context.ShortUrls.FirstOrDefaultAsync(x => x.ShortenUrl == code);
        }
    }
}
