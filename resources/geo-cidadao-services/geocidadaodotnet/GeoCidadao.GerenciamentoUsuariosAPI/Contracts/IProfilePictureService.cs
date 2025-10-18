using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeoCidadao.GerenciamentoUsuariosAPI.Contracts
{
    public interface IProfilePictureService
    {
        Task UpdateUserPhotoAsync(Guid userId, string photoBase64);
    }
}