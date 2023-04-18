using pruebaMidasoftBack.Core.DTOs;
using pruebaMidasoftBack.Core.Models;

namespace pruebaMidasoftBack.Core.Interfaces
{
    public interface IFamiliarService
    {
        Task<int> CreateFamiliar(CreateFamiliarDTO familiarDTO, string usuario);
        Task<IEnumerable<FamiliarDTO>> GetFamiliares();
        Task<int> EditFamiliar(CreateFamiliarDTO familiarDTO, string usuario);
        Task<int> DeleteFamiliar(int familiarId);
        Task<FamiliarDTO> GetFamiliarById(int familiarId);
    }
}
