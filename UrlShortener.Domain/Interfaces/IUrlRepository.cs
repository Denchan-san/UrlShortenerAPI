using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Domain.Models;

namespace UrlShortener.Domain.Interfaces
{
    public interface IUrlRepository
    {
        Task<IEnumerable<ShortUrl>> GetAllAsync();
        Task<ShortUrl> GetByCodeAsync(string code);
        Task AddAsync (ShortUrl url);
        Task DeleteAsync(int id);
    }
}
