using Microsoft.AspNetCore.Authorization;
using MinimalAPICatalogo.Models;
using MinimalAPICatalogo.Services;

namespace MinimalAPICatalogo.ApiEndpoints
{
    public static class AutenticacaoEndpoints
    {
        public static void MapAutenticacaoEndpoints(this WebApplication app)
        {
            // endpoint para login
            app.MapPost("/login", [AllowAnonymous] (UserModel userModel, ITokenService tokenService) =>
            {
                if (userModel == null)
                {
                    return Results.BadRequest("Credencial INVÁLIDO!");
                }
                if (userModel.UserName == "macoratti" && userModel.Password == "numsey#123")
                {
                    var tokenString = tokenService.GerarToken(app.Configuration["Jwt:Key"],
                       app.Configuration["Jwt:Issuer"],
                       app.Configuration["Jwt:Audience"],
                       userModel);
                    return Results.Ok(new { token = tokenString });
                }
                else
                {
                    return Results.BadRequest("Credencial INVÁLIDO!");
                }
            }).Produces(StatusCodes.Status400BadRequest)
              .Produces(StatusCodes.Status200OK)
              .WithName("Login")
              .WithTags("Autenticacao");
        }
    }
}

