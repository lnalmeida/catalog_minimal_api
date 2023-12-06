using CatalogA_PI_Minimal.Context;
using CatalogA_PI_Minimal.Domain.Models;
using CatalogA_PI_Minimal.Endpoints;
using CatalogA_PI_Minimal.Services;
using CatalogA_PI_Minimal.ServicesExtensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

#region Configure Services
builder.AddApiSwagger();
builder.AddPersistence();
builder.Services.AddCors();
builder.AddAuthenticationJWT();
#endregion

var app = builder.Build();

#region Endpoints
app.MapGet("/", () => "Catalog Minimal API Online.").WithTags("Test");

app.MapLoginEndpoint();

app.MapCategoriesEndpoints();

app.MapProductsEndpoints();

#endregion

#region Exception Handling
var env = app.Environment;

app.UseExceptionHandling(env)
    .UseSwaggerMiddleware()
    .UseAppCors();
#endregion

app.UseAuthentication();
app.UseAuthorization();

app.Run();
