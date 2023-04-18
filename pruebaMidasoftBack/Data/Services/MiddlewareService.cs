using AutoMapper;
using Microsoft.Extensions.Configuration;
using pruebaMidasoftBack.Core.DTOs;
using pruebaMidasoftBack.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pruebaMidasoftBack.Data.Services
{
    public class MiddlewareService: IMiddlewareService
    {
        public IMapper Mapper { get; }
        public IConfiguration Configuration { get; }

        public MiddlewareService(IMapper mapper, IConfiguration configuration)
        {
            Mapper = mapper;
            Configuration = configuration;
        }

        public async Task CreateLog(LogDTO logDTO)
        {
            // Creación de un nuevo log
            try
            {
                using (SqlConnection connection = new SqlConnection(Configuration.GetConnectionString("dbconnection")))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("SPSaveLog", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Asignar parámetros a los valores reales
                        command.Parameters.AddWithValue("@Peticion", logDTO.Peticion);
                        command.Parameters.AddWithValue("@Respuesta", logDTO.Respuesta);

                        // Ejecutar el comando
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        // rowsAffected contiene el número de filas afectadas por la operación

                    }
                }
            }
            catch (Exception ex)
            {
                // En caso de ocurrir una excepción
                throw new Exception("Error al crear el log: " + ex.Message);
            }
        }
    }
}
