
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SurveyBasket.Authentication;

public class JwtProvider : IJwtProvider
{
    public (string Token, int ExpiresIn) GenerateJwtToken(ApplicationUser user)
    {
        Claim[] claims = [
            
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email,user.Email!),
            new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
            new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];


        var symmetricSecuritKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ThisIsASecretKeyForJwtTokenGeneration1234"));

        var signingCredentials = new SigningCredentials(symmetricSecuritKey, SecurityAlgorithms.HmacSha256);

        //
        var ExpiresIn = 60; // minutes
        var expirationDate = DateTime.UtcNow.AddMinutes(ExpiresIn);

        var token = new JwtSecurityToken(
            issuer: "SurveyBasketAPI",
            audience: "SurveyBasketClient",
            claims: claims,
            expires: expirationDate,
            signingCredentials: signingCredentials
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), ExpiresIn);
    }
}
