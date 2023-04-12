using Microsoft.IdentityModel.Tokens;
using MinimalAPICatalogo.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MinimalAPICatalogo.Services;

public class TokenService : ITokenService
{

    //classe que implementa a geração do token
    public string GerarToken(string key, string issuer, string audience, UserModel user)
    {
        var claims = new[] //declarações que compões o playload do usuário
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier,Guid.NewGuid().ToString())
        };         // gerar uma chave - gerando uma chave secreta


        //codificação da chave secreta
        var securityKey =  new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

        //aplicar um algoritmo para ter uma chave secreta
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        
        //gerando o token
        var token = new JwtSecurityToken(issuer: issuer, audience: audience,
                                        claims: claims,
                                        expires: DateTime.Now.AddMinutes(120),
                                        signingCredentials: credentials);

        //desserializar o token
        var tokenHandler =  new JwtSecurityTokenHandler();
        var stringToken = tokenHandler.WriteToken(token);

        return stringToken;
    }

}
