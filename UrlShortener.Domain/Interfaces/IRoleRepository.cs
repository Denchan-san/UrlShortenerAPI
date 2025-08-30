using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Domain.Models;

namespace UrlShortener.Domain.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role> GetByNameAsync(string name);
    }
}
