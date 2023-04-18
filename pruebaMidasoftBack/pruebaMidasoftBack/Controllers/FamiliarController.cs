using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pruebaMidasoftBack.Api.Filters;
using pruebaMidasoftBack.Core.DTOs;
using pruebaMidasoftBack.Core.Interfaces;
using pruebaMidasoftBack.Core.Validations;
using System.Net;
using System.Security.Claims;

namespace pruebaMidasoftBack.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("Api/[controller]")]
    public class FamiliarController : ControllerBase
    {
        private IFamiliarService FamiliarService { get; }
        public ITokenValidate TokenValidate { get; }

        public FamiliarController(IFamiliarService FamiliarService, ITokenValidate tokenValidate)
        {
            this.FamiliarService = FamiliarService;
            TokenValidate = tokenValidate;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FamiliarDTO>>> Get()
        {
            try
            {
                //Verificar que el usuario del token existe en la base de datos
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var jwtToken = await TokenValidate.ValidarToken(identity);
                if (!jwtToken.Success) return BadRequest("Error en credenciales.");

                return Ok(await FamiliarService.GetFamiliares());
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que pueda ocurrir en el servicio
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet("{familiarId}")]
        public async Task<IActionResult> GetFamiliarById(int familiarId)
        {
            try
            {
                //Verificar que el usuario del token existe en la base de datos
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var jwtToken = await TokenValidate.ValidarToken(identity);
                if (!jwtToken.Success) return BadRequest("Error en credenciales.");

                //Buscar familiar por id
                var familiar = await FamiliarService.GetFamiliarById(familiarId);

                if (familiar != null)
                {
                    return Ok(familiar);
                }
                else
                {
                    // Retornar un código de estado 404 si no se encuentra el familiar
                    return BadRequest("No se pudo obtener el familiar."); 
                }
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que pueda ocurrir en el servicio
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Save([FromBody] CreateFamiliarDTO familiarDTO)
        {
            try
            {
                //Verificar que el usuario del token existe en la base de datos
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var jwtToken = await TokenValidate.ValidarToken(identity);
                if (!jwtToken.Success) return BadRequest("Error en credenciales.");

                // Validar el objeto CreateFamiliarDTO utilizando la clase de validación CreateFamiliarValidator
                var validator = new CreateFamiliarValidator();
                var result = await validator.ValidateAsync(familiarDTO);

                // Si la validación no es exitosa, retornar una respuesta 400 Bad Request con los mensajes de error
                if (!result.IsValid)
                {
                    var errors = result.Errors.Select(error => new { Propiedad = error.PropertyName, Mensaje = error.ErrorMessage });
                    return BadRequest(errors);
                }

                int rowsAffected = await FamiliarService.CreateFamiliar(familiarDTO, jwtToken.UserDTO.Usuario);
                if (rowsAffected > 0)
                {
                    // Si la operación fue exitosa, retornar una respuesta 201 Created con el número de filas afectadas
                    return StatusCode((int)HttpStatusCode.Created, rowsAffected);
                }
                else
                {
                    // Si la operación no tuvo ningún impacto en la base de datos, retornar una respuesta 400 Bad Request
                    return BadRequest("No se pudo crear el familiar.");
                }
            }
            catch (Exception ex)
            {
                // En caso de ocurrir una excepción, retornar una respuesta 500 Internal Server Error con el mensaje de error
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult> Update([FromBody] CreateFamiliarDTO familiarDTO)
        {
            try
            {
                //Verificar que el usuario del token existe en la base de datos
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var jwtToken = await TokenValidate.ValidarToken(identity);
                if (!jwtToken.Success) return BadRequest("Error en credenciales.");

                // Validar el objeto CreateFamiliarDTO utilizando la clase de validación CreateFamiliarValidator
                var validator = new CreateFamiliarValidator();
                var result = await validator.ValidateAsync(familiarDTO);

                // Si la validación no es exitosa, retornar una respuesta 400 Bad Request con los mensajes de error
                if (!result.IsValid)
                {
                    var errors = result.Errors.Select(error => new { Propiedad = error.PropertyName, Mensaje = error.ErrorMessage });
                    return BadRequest(errors);
                }

                int rowsAffected = await FamiliarService.EditFamiliar(familiarDTO, jwtToken.UserDTO.Usuario);
                if (rowsAffected > 0)
                {
                    return Ok("El objeto FamiliarDTO se ha actualizado exitosamente."); // Se devuelve una respuesta HTTP 200 OK con un mensaje de éxito
                }
                else
                {
                    return NotFound("No se encontró el objeto FamiliarDTO especificado."); // Se devuelve una respuesta HTTP 404 Not Found si no se encuentra el objeto a editar
                }
            }
            catch (Exception ex)
            {
                // Manejar errores y devolver una respuesta de error adecuada
                return StatusCode(500, $"Ocurrió un error al actualizar el objeto FamiliarDTO: {ex.Message}");
            }
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                //Verificar que el usuario del token existe en la base de datos
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var jwtToken = await TokenValidate.ValidarToken(identity);
                if (!jwtToken.Success) return BadRequest("Error en credenciales.");

                int rowsAffected = await FamiliarService.DeleteFamiliar(id);
                if (rowsAffected > 0)
                {
                    // Se devuelve una respuesta HTTP 200 OK con un mensaje de éxito
                    return Ok("El objeto FamiliarDTO se ha eliminado exitosamente."); 
                }
                else
                {
                    // Se devuelve una respuesta HTTP 404 Not Found si no se encuentra el objeto a eliminar
                    return NotFound("No se encontró el objeto FamiliarDTO especificado."); 
                }
            }
            catch (Exception ex)
            {
                // Manejar errores y devolver una respuesta de error adecuada
                return StatusCode(500, $"Ocurrió un error al eliminar el objeto FamiliarDTO: {ex.Message}");
            }
        }
    }
}
