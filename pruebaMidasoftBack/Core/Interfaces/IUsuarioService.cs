using pruebaMidasoftBack.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pruebaMidasoftBack.Core.Interfaces
{
    public interface IUsuarioService
    {
        Task<string> GetUsuarioByLogin(UserDTO UserDTO);
        Task<UserDTO> GetUserById(string username);
        Task<IEnumerable<UsernameDTO>> GetUsuarios();
        Task<int> CreateUser(UserDTO userDTO);
        Task<int> EditUser(UserDTO userDTO);
        Task<int> DeleteUser(UsernameDTO usernameDTO);
    }
}
