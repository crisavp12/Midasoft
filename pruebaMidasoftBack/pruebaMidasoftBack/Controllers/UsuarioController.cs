using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pruebaMidasoftBack.Core.DTOs;
using pruebaMidasoftBack.Core.Interfaces;
using pruebaMidasoftBack.Core.Models;
using pruebaMidasoftBack.Data.Services;
using System.Net;
using System.Security.Claims;

namespace pruebaMidasoftBack.Controllers
{
    [ApiController]
    [Authorize]
    [Route("Api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private IUsuarioService UsuarioService { get; }
        public ITokenValidate TokenValidate { get; }

        public UsuarioController(IUsuarioService usuarioService, ITokenValidate TokenValidate)
        {
            UsuarioService = usuarioService;
            this.TokenValidate = TokenValidate;
        }

        //Iniciar sesion
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserDTO UserDTO)
        {
            try
            {
                //Generar token
                var token = await UsuarioService.GetUsuarioByLogin(UserDTO);

                if (token != null)
                {
                    return Ok(token);
                }
                else
                {
                    // Retornar un código de estado 404 si no se encuentra el usuario
                    return NotFound(); 
                }
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que pueda ocurrir en el servicio
                return StatusCode(500, $"Error al ingresar: {ex.Message}");
            }
        }

        //Lista de usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> Get()
        {
            try
            {
                //Verificar que el usuario del token existe en la base de datos
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var jwtToken = await TokenValidate.ValidarToken(identity);
                if (!jwtToken.Success) return BadRequest("Error en credenciales.");

                return Ok(await UsuarioService.GetUsuarios());
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que pueda ocurrir en el servicio
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        //Crear usuario
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Save([FromBody] UserDTO userDTO)
        {
            try
            {

                int rowsAffected = await UsuarioService.CreateUser(userDTO);
                if (rowsAffected > 0)
                {
                    // Si la operación fue exitosa, retornar una respuesta 201 Created con el número de filas afectadas
                    return StatusCode((int)HttpStatusCode.Created, rowsAffected);
                }
                else
                {
                    // Si la operación no tuvo ningún impacto en la base de datos, retornar una respuesta 400 Bad Request
                    return BadRequest("No se pudo crear el usuario.");
                }
            }
            catch (Exception ex)
            {
                // En caso de ocurrir una excepción, retornar una respuesta 500 Internal Server Error con el mensaje de error
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        //Actualizar usuario
        [HttpPut]
        public async Task<ActionResult> Update([FromBody] UserDTO userDTO)
        {
            try
            { 
                //Verificar que el usuario del token existe en la base de datos
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var jwtToken = await TokenValidate.ValidarToken(identity);
                if (!jwtToken.Success) return BadRequest("Error en credenciales.");

                int rowsAffected = await UsuarioService.EditUser(userDTO);
                if (rowsAffected > 0)
                {
                    return Ok("La contraseña se ha actualizado exitosamente."); // Se devuelve una respuesta HTTP 200 OK con un mensaje de éxito
                }
                else
                {
                    return NotFound("No se encontró el usuario especificado."); // Se devuelve una respuesta HTTP 404 Not Found si no se encuentra el objeto a editar
                }
            }
            catch (Exception ex)
            {
                // Manejar errores y devolver una respuesta de error adecuada
                return StatusCode(500, $"Ocurrió un error al actualizar el usuario: {ex.Message}");
            }
        }

        [HttpDelete]
        public async Task<ActionResult> Delete([FromBody] UsernameDTO usernameDTO)
        {
            try
            {
                //Verificar que el usuario del token existe en la base de datos
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var jwtToken = await TokenValidate.ValidarToken(identity);
                if (!jwtToken.Success) return BadRequest("Error en credenciales.");

                int rowsAffected = await UsuarioService.DeleteUser(usernameDTO);
                if (rowsAffected > 0)
                {
                    return Ok("El usuario se ha eliminado exitosamente."); // Se devuelve una respuesta HTTP 200 OK con un mensaje de éxito
                }
                else
                {
                    return NotFound("No se encontró el usuario especificado."); // Se devuelve una respuesta HTTP 404 Not Found si no se encuentra el objeto a editar
                }
            }
            catch (Exception ex)
            {
                // Manejar errores y devolver una respuesta de error adecuada
                return StatusCode(500, $"Ocurrió un error al eliminar el usuario: {ex.Message}");
            }
        }
    }
}