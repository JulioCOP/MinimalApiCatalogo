using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MinimalAPICatalogo.ApiEndpoints;
using MinimalAPICatalogo.AppServicesExtensions;
using MinimalAPICatalogo.Context;
using MinimalAPICatalogo.Models;
using MinimalAPICatalogo.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container //ConfigureServices (classe startup) 



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// obter a string de conexão

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options => 
                                            options.UseMySql(connectionString, 
                                            ServerVersion.AutoDetect(connectionString)));

// registro do serviço de geração do token
builder.Services.AddSingleton<ITokenService>(new TokenService());

//validação do token
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

builder.Services.AddAuthorization();


var app = builder.Build(); // Em caso de inclusão no container dever ser antes do comando build

app.MapAutenticacaoEndpoints();
app.MapCategoriasEndpoints();
app.MapProdutosEndpoints();

var environment = app.Environment;
app.UseExceptionHandling(environment).UseSwaggerMiddleware().UseAppCors();


//ativar os serviços de autorização e autenticação
app.UseAuthentication();
app.UseAuthorization();

app.Run();

