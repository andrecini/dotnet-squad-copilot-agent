using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Copilot.SquadAgent.StickerManager.Domain.Interfaces.Security;
using Copilot.SquadAgent.StickerManager.Domain.Models.User;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Copilot.SquadAgent.StickerManager.Infrastructure.Security;

public class JwtTokenGenerator(IConfiguration configuration) : IJwtTokenGenerator
{
    private const int ExpiresInSeconds = 3600;

    public TokenModel Generate(UserModel user)
    {
        var jwtSettings = configuration.GetSection("Jwt");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"]!,
            audience: jwtSettings["Audience"]!,
            claims: claims,
            expires: DateTime.UtcNow.AddSeconds(ExpiresInSeconds),
            signingCredentials: credentials);

        return new TokenModel
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            TokenType = "Bearer",
            ExpiresIn = ExpiresInSeconds
        };
    }
}
