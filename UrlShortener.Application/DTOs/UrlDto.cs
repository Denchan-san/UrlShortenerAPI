using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.Application.DTOs
{
    internal class UrlDto
    {
        public string OriginalUrl { get; set; }
        public string ShortenUrl { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedAt { get; set; }
    }
}
