
using Microsoft.AspNetCore.Identity;
using SurveyBasket.Authentication;
using System.Security.Cryptography;

namespace SurveyBasket.Services;

public class AuthService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _JwtProvider = jwtProvider;

    private readonly int _RefreshTokenExpiryDays = 14;

    public async Task<AuthResponse?> GetTokenAsync(string Email, string Password, CancellationToken cancellationToken = default)
    {
        var User = await _userManager.FindByEmailAsync(Email);

        if (User is null)
        {
            return null;
        }

        var IsValidPassword = await _userManager.CheckPasswordAsync(User, Password);

        if (!IsValidPassword)
        {
            return null;
        }

        var (Token, ExpiresIn) = _JwtProvider.GenerateJwtToken(User);

        var RefreshToken = GenerateRefreshToken();

        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_RefreshTokenExpiryDays);

        User.RefreshTokens.Add(new RefreshToken
        {
            Token = RefreshToken,
            ExpiresOn = refreshTokenExpiration,
        });

        await _userManager.UpdateAsync(User);

        return new AuthResponse(User.Id, User.Email, User.FirstName, User.LastName, Token, ExpiresIn, RefreshToken, refreshTokenExpiration);
    }

    public async Task<AuthResponse?> GetRefreshTokenAsync(string Token, string RefreshToken, CancellationToken cancellationToken = default)
    {
        var UserId = _JwtProvider.ValidateToken(Token);

        if (UserId is null)
        {
            return null;
        }

        var User  = await _userManager.FindByIdAsync(UserId);

        if (User is null)
        {
            return null;
        }

        var storedRefreshToken = User.RefreshTokens.FirstOrDefault(rt => rt.Token == RefreshToken && rt.IsActive);

        if (storedRefreshToken is null)
        {
            return null;
        }

        storedRefreshToken.RevokedOn = DateTime.UtcNow;

        var (NewToken, ExpiresIn) = _JwtProvider.GenerateJwtToken(User);

        var NewRefreshToken = GenerateRefreshToken();

        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_RefreshTokenExpiryDays);

        User.RefreshTokens.Add(new RefreshToken
        {
            Token = NewRefreshToken,
            ExpiresOn = refreshTokenExpiration,
        });

        await _userManager.UpdateAsync(User);

        return new AuthResponse(User.Id, User.Email, User.FirstName, User.LastName, NewToken, ExpiresIn, NewRefreshToken, refreshTokenExpiration);


    }

    public async Task<bool> RevokeRefreshTokenAsync(string Token, string RefreshToken, CancellationToken cancellationToken = default)
    {
        var UserId = _JwtProvider.ValidateToken(Token);

        if (UserId is null)
        {
            return false;
        }

        var User = await _userManager.FindByIdAsync(UserId);

        if (User is null)
        {
            return false;
        }

        var storedRefreshToken = User.RefreshTokens.FirstOrDefault(rt => rt.Token == RefreshToken && rt.IsActive);

        if (storedRefreshToken is null)
        {
            return false;
        }

        storedRefreshToken.RevokedOn = DateTime.UtcNow;

        await _userManager.UpdateAsync(User);

        return true;
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    
}
