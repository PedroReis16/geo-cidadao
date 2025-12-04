using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeoCidadao.FeedMapAPI.Models.DTOs
{
    public class AuthorDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
    }
}