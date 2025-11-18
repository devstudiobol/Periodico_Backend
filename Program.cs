// --- AÑADIDOS PARA CLOUDINARY ---
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PeriodicoUpdate.Data;
using PeriodicoUpdate.Models;
using PeriodicoUpdate.Services;
using System.Text;
// ---------------------------------

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<DBconexion>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("Connection")));

// --- CONFIGURACIÓN DE CLOUDINARY ---

// 1. Mapea la sección "CloudinarySettings" de appsettings.json a una clase
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

// 2. Registra el cliente oficial de Cloudinary (del paquete NuGet) como Singleton
builder.Services.AddSingleton(provider =>
{
    // Obtiene las opciones de configuración que acabamos de registrar
    var settings = provider.GetRequiredService<IOptions<CloudinarySettings>>().Value;

    // Valida que las credenciales no estén vacías
    if (string.IsNullOrEmpty(settings.CloudName) ||
        string.IsNullOrEmpty(settings.ApiKey) ||
        string.IsNullOrEmpty(settings.ApiSecret))
    {
        throw new InvalidOperationException("No se encontraron las credenciales de Cloudinary en la configuración.");
    }

    // Crea la cuenta con las credenciales
    Account account = new Account(
        settings.CloudName,
        settings.ApiKey,
        settings.ApiSecret
    );

    // Devuelve una instancia del cliente de Cloudinary
    return new Cloudinary(account);
});

// 3. Registra TU PROPIO servicio de fotos (IPhotoService)
builder.Services.AddScoped<IPhotoService, PhotoService>();

// --- FIN DE SECCIÓN DE CLOUDINARY ---


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Habilitar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("Cors", policy =>
    {
        policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
    });
});

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// --- ATENCIÓN AQUÍ ---
// Asegúrate de que UseAuthentication() esté ANTES de UseAuthorization()
app.UseAuthentication();
app.UseAuthorization();
// --------------------

app.UseCors("Cors");

app.MapControllers();

app.Run();