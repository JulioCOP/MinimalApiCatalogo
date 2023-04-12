using MinimalAPICatalogo.Models;

namespace MinimalAPICatalogo.Services;

public interface ITokenService
{
    string GerarToken(string key, string issuer, string audience, UserModel user);
}
