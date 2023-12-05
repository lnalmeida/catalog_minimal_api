using CatalogA_PI_Minimal.Domain.Models;

namespace CatalogA_PI_Minimal.Services;

public interface ITokenService
{
    string GenerateToken(string key, string issuer, string audience, UserModel user);
}
