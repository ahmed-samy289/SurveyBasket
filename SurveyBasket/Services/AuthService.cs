
using Microsoft.AspNetCore.Identity;
using SurveyBasket.Authentication;

namespace SurveyBasket.Services;

public class AuthService(UserManager<ApplicationUser> userManager , IJwtProvider jwtProvider) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _JwtProvider  = jwtProvider;


    public async Task<AuthResponse?> GetTokenAsync(string Email, string Password, CancellationToken cancellationToken = default)
    {
        var User = await _userManager.FindByEmailAsync(Email);

        if (User is null) { 
            return null;
        }

        var IsValidPassword = await _userManager.CheckPasswordAsync(User, Password);

        if (!IsValidPassword)
        {
            return null;
        }

        var (Token, ExpiresIn) = _JwtProvider.GenerateJwtToken(User);

        return new AuthResponse(User.Id, User.Email, User.FirstName, User.LastName, Token, ExpiresIn);
    }
}
