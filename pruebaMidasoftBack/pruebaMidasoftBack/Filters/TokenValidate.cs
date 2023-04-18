using pruebaMidasoftBack.Core.DTOs;
using pruebaMidasoftBack.Core.Interfaces;
using System.Security.Claims;

namespace pruebaMidasoftBack.Api.Filters
{
    public class TokenValidate: ITokenValidate
    {
        private IUsuarioService UsuarioService { get; }
        public TokenValidate(IUsuarioService usuarioService)
        {
            UsuarioService = usuarioService;
        }

        //Validar que es un token valido y que existe el usuario del token
        public async Task<TokenValidationResult> ValidarToken(ClaimsIdentity identity)
        {
            try
            {
                //Contar los claims del identity
                if (identity.Claims.Count() == 0)
                {
                    throw new Exception("Verifica que estás enviando un token valido");
                }

                //Encontrar identificador en los claims y verificar si existe el usuario
                var id = identity.Claims.FirstOrDefault(x => x.Type == "id").Value;
                UserDTO userDTO = await UsuarioService.GetUserById(id);
                if (userDTO == null)
                {
                    throw new Exception("Registrarse nuevamente");
                }

                return new TokenValidationResult
                {
                    Success = true,
                    UserDTO = userDTO
                };

            }
            catch (Exception ex)
            {
                // Manejo de exceciones
                throw new Exception("Error al obtener el Usuario: " + ex.Message);
            }
        }
    }
}
