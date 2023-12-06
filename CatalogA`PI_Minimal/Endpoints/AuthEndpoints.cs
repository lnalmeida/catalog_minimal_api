using CatalogA_PI_Minimal.Domain.Models;
using CatalogA_PI_Minimal.Services;
using Microsoft.AspNetCore.Authorization;

namespace CatalogA_PI_Minimal.Endpoints
{
    public static class AuthEndpoints
    {
        public static void MapLoginEndpoint(this WebApplication app)
        {
            app.MapPost("/auth/login", [AllowAnonymous] (UserModel user, ITokenService tokenService) =>
            {
                if (user == null) return Results.BadRequest("Invalid Login!");
                if (user.Username == "nunes" && user.Password == "senha123")
                {
                    var tokenString = tokenService.GenerateToken(app.Configuration["JWTSecret:Key"],
                                                                 app.Configuration["JWTSecret:Issuer"],
                                                                 app.Configuration["JWTSecret:Audience"],
                                                                 user);

                    return Results.Ok(new { token = tokenString });
                }
                else
                {
                    return Results.BadRequest("Invalid Login!!");
                };
            }).WithTags("Auth");
        }
    }
}
