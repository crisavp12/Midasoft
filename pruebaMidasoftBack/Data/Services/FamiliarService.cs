using System;
using pruebaMidasoftBack.Core.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using pruebaMidasoftBack.Core.Interfaces;
using pruebaMidasoftBack.Core.DTOs;
using AutoMapper;
using System.Data;

namespace pruebaMidasoftBack.Data.Services
{
    public class FamiliarService : IFamiliarService
    {
        public IConfiguration Configuration { get; }
        public IMapper Mapper { get; }

        public FamiliarService(IConfiguration configuration, IMapper mapper)
        {
            Configuration = configuration;
            Mapper = mapper;
        }
        public async Task<FamiliarDTO> GetFamiliarById(int familiarId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Configuration.GetConnectionString("dbconnection")))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("SPGetFamiliarById", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@FamiliarId", familiarId);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                FamiliarDTO familiarDTO = new FamiliarDTO();
                                familiarDTO.FamiliarId = reader.GetInt32(reader.GetOrdinal("FamiliarId"));
                                familiarDTO.Usuario = reader.GetString(reader.GetOrdinal("Usuario"));
                                familiarDTO.Cedula = reader.GetString(reader.GetOrdinal("Cedula"));
                                familiarDTO.Nombres = reader.GetString(reader.GetOrdinal("Nombres"));
                                familiarDTO.Apellidos = reader.GetString(reader.GetOrdinal("Apellidos"));
                                familiarDTO.Genero = reader.GetString(reader.GetOrdinal("Genero"));
                                familiarDTO.Parentesco = reader.GetString(reader.GetOrdinal("Parentesco"));
                                familiarDTO.Edad = reader.GetInt32(reader.GetOrdinal("Edad"));
                                familiarDTO.MenorEdad = reader.GetBoolean(reader.GetOrdinal("MenorEdad"));
                                familiarDTO.FechaNacimiento = reader.IsDBNull(reader.GetOrdinal("FechaNacimiento")) ? null : (DateTime?)reader.GetDateTime(reader.GetOrdinal("FechaNacimiento"));

                                // Retorna un objeto FamiliarDTO con los datos obtenidos de la base de datos
                                return familiarDTO;
                            }
                            else
                            {
                                return null; // Si no se encuentra el familiar se envia un null
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

        private async Task<bool> ValidarFamiliarExistente(string cedula)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Configuration.GetConnectionString("dbconnection")))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("SPGetFamiliarByDocument", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@cedula", cedula);

                        // Ejecutar el comando y obtener el resultado
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                // Si el valor es 1, significa que existe un familiar asociado al usuario
                                return true;
                            }
                            else
                            {
                                // Si no se encuentra ningún resultado, devolver false
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de exceciones
                throw new Exception("Error al verificar la existencia del familiar: " + ex.Message);
            }
        }
        

        private async Task<bool> ValidarFamiliarAsociadoAUsuario(string usuario)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Configuration.GetConnectionString("dbconnection")))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("SPGetFamiliarByUser", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@usuario", usuario);

                        // Ejecutar el comando y obtener el resultado
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                // Leer el valor devuelto por el stored procedure
                                int existe = Convert.ToInt32(reader["Existe"]);

                                // Si el valor es 1, significa que existe un familiar asociado al usuario
                                return existe == 1;
                            }
                            else
                            {
                                // Si no se encuentra ningún resultado, devolver false
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de exceciones
                throw new Exception("Error al verificar la existencia del familiar por usuario: " + ex.Message);
            }
        }

        public async Task<int> CreateFamiliar(CreateFamiliarDTO familiarDTO, string usuario)
        {

            // creación de un nuevo registro (CREATE)
            try
            {
                //Validar si ya existe un familiar para este usuario
                bool familiar = await ValidarFamiliarAsociadoAUsuario(usuario);
                if (familiar)
                {
                    throw new Exception("Ya existe un familiar asociado a este usuario");
                }

                //Validar si ya existe un familiar con este numero de documento
                familiar = await ValidarFamiliarExistente(familiarDTO.Cedula);
                if (familiar)
                {
                    throw new Exception("Ya existe un familiar con este documento");
                }

                using (SqlConnection connection = new SqlConnection(Configuration.GetConnectionString("dbconnection")))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("SPCreateFamiliar", connection))
                    {
                        // Establecer el tipo de comando como Stored Procedure
                        command.CommandType = CommandType.StoredProcedure;

                        // Asignar parámetros a los valores reales
                        command.Parameters.AddWithValue("@Usuario", usuario);
                        command.Parameters.AddWithValue("@Cedula", familiarDTO.Cedula);
                        command.Parameters.AddWithValue("@Nombres", familiarDTO.Nombres);
                        command.Parameters.AddWithValue("@Apellidos", familiarDTO.Apellidos);
                        command.Parameters.AddWithValue("@Genero", familiarDTO.Genero);
                        command.Parameters.AddWithValue("@Parentesco", familiarDTO.Parentesco);
                        command.Parameters.AddWithValue("@Edad", familiarDTO.Edad);
                        command.Parameters.AddWithValue("@MenorEdad", familiarDTO.Edad < 18);
                        command.Parameters.AddWithValue("@FechaNacimiento", familiarDTO.FechaNacimiento ?? (object)DBNull.Value);

                        // Ejecutar el comando
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        // rowsAffected contiene el número de filas afectadas por la operación

                        return rowsAffected;
                    }
                }
            }
            catch (Exception ex)
            {
                // En caso de ocurrir una excepción, puedes retornar un valor predeterminado o lanzar una excepción
                throw new Exception("Error al crear el familiar: " + ex.Message);
            }
        }


        public async Task<int> EditFamiliar(CreateFamiliarDTO familiarDTO, string usuario)
        {
            try
            {

                using (SqlConnection connection = new SqlConnection(Configuration.GetConnectionString("dbconnection")))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("SPEditFamiliar", connection))
                    {
                        // Establecer el tipo de comando como Stored Procedure
                        command.CommandType = CommandType.StoredProcedure;

                        // Asignar parámetros a los valores reales
                        command.Parameters.AddWithValue("@FamiliarId", familiarDTO.FamiliarId);
                        command.Parameters.AddWithValue("@Usuario", usuario);
                        command.Parameters.AddWithValue("@Cedula", familiarDTO.Cedula);
                        command.Parameters.AddWithValue("@Nombres", familiarDTO.Nombres);
                        command.Parameters.AddWithValue("@Apellidos", familiarDTO.Apellidos);
                        command.Parameters.AddWithValue("@Genero", familiarDTO.Genero);
                        command.Parameters.AddWithValue("@Parentesco", familiarDTO.Parentesco);
                        command.Parameters.AddWithValue("@Edad", familiarDTO.Edad);
                        command.Parameters.AddWithValue("@MenorEdad", familiarDTO.Edad < 18);
                        command.Parameters.AddWithValue("@FechaNacimiento", familiarDTO.FechaNacimiento ?? (object)DBNull.Value);

                        // Ejecutar el comando
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        // rowsAffected contiene el número de filas afectadas por la operación

                        return rowsAffected;
                    }
                }
            
            }
            catch (Exception ex)
            {
                // En caso de ocurrir una excepción, puedes retornar un valor predeterminado o lanzar una excepción
                throw new Exception("Error al crear el familiar: " + ex.Message);
            }
        }

        public async Task<IEnumerable<FamiliarDTO>> GetFamiliares()
        {

            using (SqlConnection connection = new SqlConnection(Configuration.GetConnectionString("dbconnection"))) 
            {
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand("SPGetFamiliares", connection))
                {
                    // Establecer el tipo de comando como Stored Procedure
                    command.CommandType = CommandType.StoredProcedure; 
                    List<Familiar> familiares = new List<Familiar>();

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        
                        while (reader.Read())
                        {
                            Familiar familiar = new Familiar();
                            familiar.FamiliarId = Convert.ToInt32(reader["FamiliarId"]);
                            familiar.Usuario = Convert.ToString(reader["Usuario"]);
                            familiar.Cedula = Convert.ToString(reader["Cedula"]);
                            familiar.Nombres = Convert.ToString(reader["Nombres"]);
                            familiar.Apellidos = Convert.ToString(reader["Apellidos"]);
                            familiar.Genero = Convert.ToString(reader["Genero"]);
                            familiar.Parentesco = Convert.ToString(reader["Parentesco"]);
                            familiar.Edad = Convert.ToInt32(reader["Edad"]);
                            familiar.MenorEdad = reader.GetBoolean(reader.GetOrdinal("MenorEdad"));
                            familiar.FechaNacimiento = reader["FechaNacimiento"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["FechaNacimiento"]);

                            familiares.Add(familiar);
                        }
                    }
                    List<FamiliarDTO> familiaresDTO = Mapper.Map<List<FamiliarDTO>>(familiares);
                    return familiaresDTO;
                }
            }
        }
        public async Task<int> DeleteFamiliar(int familiarId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Configuration.GetConnectionString("dbconnection")))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("SPDeleteFamiliar", connection))
                    {
                        // Establecer el tipo de comando como Stored Procedure
                        command.CommandType = CommandType.StoredProcedure; 

                        // Asignar parámetros a los valores reales
                        command.Parameters.AddWithValue("@FamiliarId", familiarId);

                        // Ejecutar el comando
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        // rowsAffected contiene el número de filas afectadas por la operación

                        return rowsAffected;
                    }
                }
            }
            catch (Exception ex)
            {
                // En caso de ocurrir una excepción, puedes retornar un valor predeterminado o lanzar una excepción
                throw new Exception("Error al crear el familiar: " + ex.Message);
            }
        }
    }
}
