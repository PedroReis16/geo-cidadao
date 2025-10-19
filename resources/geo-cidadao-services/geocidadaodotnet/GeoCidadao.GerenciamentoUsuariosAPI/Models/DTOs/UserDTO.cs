using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Models.DTOs
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}