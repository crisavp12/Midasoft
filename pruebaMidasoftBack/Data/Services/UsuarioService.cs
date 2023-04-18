using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using pruebaMidasoftBack.Core.DTOs;
using pruebaMidasoftBack.Core.Interfaces;
using pruebaMidasoftBack.Core.Models;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace pruebaMidasoftBack.Data.Services
{
    public class UsuarioService: IUsuarioService
    {
        public IConfiguration Configuration { get; }
        public IMapper Mapper { get; }
        public UsuarioService(IMapper mapper, IConfiguration configuration)
        {
            Mapper = mapper;
            Configuration = configuration;
        }

        public async Task<IEnumerable<UsernameDTO>> GetUsuarios()
        {
            using (SqlConnection connection = new SqlConnection(Configuration.GetConnectionString("dbconnection")))
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("SPGetUsuarios", connection))
                {
                    // Establecer el tipo de comando como Stored Procedure
                    command.CommandType = CommandType.StoredProcedure; 
                    List<UsernameDTO> usuarios = new List<UsernameDTO>();

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            // Asignar parámetros a los valores reales
                            UsernameDTO usuario = new UsernameDTO();
                            usuario.Usuario = Convert.ToString(reader["Usuario"]);

                            usuarios.Add(usuario);
                        }
                    }

                    List<UsernameDTO> usuariosDTO = Mapper.Map<List<UsernameDTO>>(usuarios);
                    return usuariosDTO;
                }
            }
        }

        public async Task<UserDTO> GetUserById(string username)
        {
            // Traer usuario por el username
            try
            {
                using (SqlConnection connection = new SqlConnection(Configuration.GetConnectionString("dbconnection")))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("SPGetUsuarioByLogin", connection))
                    {
                        // Establecer el tipo de comando como Stored Procedure
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@usuario", username);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                // Asignar parámetros a los valores reales
                                User usuario = new User();
                                usuario.Usuario = reader.GetString(reader.GetOrdinal("Usuario"));
                                usuario.Contrasena = reader.GetString(reader.GetOrdinal("Contrasena"));

                                UserDTO userDTO = Mapper.Map<UserDTO>(usuario);

                                return userDTO;
                            }
                            else
                            {
                                // Si no se encuentra el usuario retornar null 
                                return null; 
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de exceciones
                throw new Exception("Error al obtener el familiar por FamiliarId: " + ex.Message);
            }
        }

        public async Task<string> GetUsuarioByLogin(UserDTO UserDTO)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Configuration.GetConnectionString("dbconnection")))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("SPGetUsuarioByLogin", connection))
                    {
                        // Establecer el tipo de comando como Stored Procedure
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Usuario", UserDTO.Usuario);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                User usuario = new User();
                                usuario.Usuario = reader.GetString(reader.GetOrdinal("Usuario"));
                                usuario.Contrasena = reader.GetString(reader.GetOrdinal("Contrasena"));

                                // Verificar si la contraseña es correcta
                                if (UserDTO.Contrasena != usuario.Contrasena)
                                {
                                    throw new Exception("Contraseña incorrecta");
                                }

                                // Obtener la configuración del Jwt desde la sección correspondiente del archivo de configuración
                                var jwtSection = Configuration.GetSection("Jwt");

                                // Crear un objeto Jwt para almacenar la configuración
                                var JsonWT = new Jwt();
                                JsonWT.Key = jwtSection["Key"];
                                JsonWT.Issuer = jwtSection["Issuer"];
                                JsonWT.Audience = jwtSection["Audience"];
                                JsonWT.Subject = jwtSection["Subject"];


                                // Declarar claims para JWT
                                var claims = new List<Claim>
                                {
                                    new Claim(JwtRegisteredClaimNames.Sub, JsonWT.Subject),
                                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                                    new Claim("id", usuario.Usuario)
                                };

                                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JsonWT.Key));
                                var singin = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                                // Crear la descripción del token
                                var tokenDescriptor = new SecurityTokenDescriptor
                                {
                                    Expires = DateTime.Now.AddHours(8),
                                    Issuer = JsonWT.Issuer,
                                    Audience = JsonWT.Audience,
                                    SigningCredentials = singin,
                                    Subject = new ClaimsIdentity(claims)
                                };

                                // Crear el token JWT
                                var tokenHandler = new JwtSecurityTokenHandler();
                                var token = tokenHandler.CreateToken(tokenDescriptor);

                                // Generar el token JWT como string
                                var tokenString = tokenHandler.WriteToken(token);

                                return tokenString;
                            }
                            else
                            {
                                // Si no se encuentra el usuario retorna null
                                return null; 
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de exceciones
                throw new Exception("Error al obtener el usuario por Usuario: " + ex.Message);
            }
        }

        public async Task<int> CreateUser(UserDTO userDTO)
        {
            // Creación de un nuevo registro 
            try
            {
                using (SqlConnection connection = new SqlConnection(Configuration.GetConnectionString("dbconnection")))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("SPCreateUser", connection))
                    {
                        // Establecer el tipo de comando como Stored Procedure
                        command.CommandType = CommandType.StoredProcedure;

                        // Asignar parámetros a los valores reales
                        command.Parameters.AddWithValue("@Usuario", userDTO.Usuario);
                        command.Parameters.AddWithValue("@Contrasena", userDTO.Contrasena);

                        // Ejecutar el comando
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        // rowsAffected contiene el número de filas afectadas por la operación

                        return rowsAffected;
                    }
                }
            }
            catch (Exception ex)
            {
                // En caso de ocurrir una excepción
                throw new Exception("Error al crear el familiar: " + ex.Message);
            }
        }

        public async Task<int> EditUser(UserDTO userDTO)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Configuration.GetConnectionString("dbconnection")))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("SPEditUserPassword", connection))
                    {
                        // Establecer el tipo de comando como Stored Procedure
                        command.CommandType = CommandType.StoredProcedure;

                        // Asignar parámetros a los valores reales
                        command.Parameters.AddWithValue("@Usuario", userDTO.Usuario);
                        command.Parameters.AddWithValue("@Contrasena", userDTO.Contrasena);

                        // Ejecutar el comando
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        // rowsAffected contiene el número de filas afectadas por la operación

                        return rowsAffected;
                    }
                }
            }
            catch (Exception ex)
            {
                // En caso de ocurrir una excepción
                throw new Exception("Error al editar el usuario: " + ex.Message);
            }
        }
        public async Task<int> DeleteUser(UsernameDTO usernameDTO)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Configuration.GetConnectionString("dbconnection")))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("SPDeleteUser", connection))
                    {
                        // Establecer el tipo de comando como Stored Procedure
                        command.CommandType = CommandType.StoredProcedure;

                        // Asignar parámetros a los valores reales
                        command.Parameters.AddWithValue("@Usuario", usernameDTO.Usuario);

                        // Ejecutar el comando
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        // rowsAffected contiene el número de filas afectadas por la operación

                        return rowsAffected;
                    }
                }
            }
            catch (Exception ex)
            {
                // En caso de ocurrir una excepción
                throw new Exception("Error al eliminar el usuario: " + ex.Message);
            }
        }
    }
}
