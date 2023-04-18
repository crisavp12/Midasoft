using Newtonsoft.Json;
using pruebaMidasoftBack.Core.DTOs;
using pruebaMidasoftBack.Core.Interfaces;
using System.Text;

namespace pruebaMidasoftBack.Api.Middlewares
{
    public class LogMiddleware : IMiddleware 
    {
    
        private readonly ILogger<LogMiddleware> _logger;
        private readonly IMiddlewareService MiddlewareService;

        public LogMiddleware(ILogger<LogMiddleware> logger, IMiddlewareService middlewareService)
        {
            _logger = logger;
            MiddlewareService = middlewareService;
        }

        //Datos de entrada y salida para insertarlos en la tabla log
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            using (var ms = new MemoryStream())
            {
                LogDTO logDTO = new LogDTO();

                // Guardar el cuerpo original de la respuesta
                var cuerpoOriginalRespuesta = context.Response.Body;
                context.Response.Body = ms;
                context.Request.EnableBuffering();
                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    // Leer el cuerpo de la solicitud
                    var requestBody = await reader.ReadToEndAsync();
                    context.Request.Body.Seek(0, SeekOrigin.Begin);

                    // Obtener los encabezados de la solicitud como una cadena de texto
                    var headers = context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
                    var headersString = JsonConvert.SerializeObject(headers);

                    context.Request.Body.Seek(0, SeekOrigin.Begin);

                    // Asignar la información de la solicitud al objeto LogDTO
                    logDTO.Peticion = ($"Path: {context.Request.Path}, Headers: {headersString}, Body: {requestBody}");
                }
                context.Request.Body.Position = 0;

                // Ejecutar el siguiente middleware
                await next(context);

                ms.Seek(0, SeekOrigin.Begin);
                string respuesta = new StreamReader(ms).ReadToEnd();
                ms.Seek(0, SeekOrigin.Begin);

                await ms.CopyToAsync(cuerpoOriginalRespuesta);
                context.Response.Body = cuerpoOriginalRespuesta;

                // Obtener el código de estado de la respuesta y asignarlo al objeto LogDTO junto con la respuesta
                string statusCode = context.Response.StatusCode.ToString();
                logDTO.Respuesta = ($"{statusCode}: {respuesta}");

                // Crear un registro de log utilizando el objeto LogDTO
                MiddlewareService.CreateLog(logDTO);
            }
        }
    }
}
