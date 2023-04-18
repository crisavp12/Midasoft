using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using pruebaMidasoftBack.Api.Filters;
using pruebaMidasoftBack.Api.Middlewares;
using pruebaMidasoftBack.Core.Interfaces;
using pruebaMidasoftBack.Data.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Autenticacion
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// Agregar el servicio de logging

builder.Services.AddLogging(); 

//Mapeo de objetos
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//Servicios
builder.Services.AddScoped<LogMiddleware>();
builder.Services.AddScoped<IFamiliarService, FamiliarService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ITokenValidate, TokenValidate>();
builder.Services.AddScoped<IMiddlewareService, MiddlewareService>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<LogMiddleware>(); // Registrar el middleware de log

app.MapControllers();

app.Run();
