using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using SurveyBasket.Abstractions;
using SurveyBasket.Authentication;
using SurveyBasket.Errors;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;

namespace SurveyBasket.Services;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ILogger<AuthService> logger,
    IJwtProvider jwtProvider) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly ILogger<AuthService> _logger = logger;
    private readonly IJwtProvider _JwtProvider = jwtProvider;

    private readonly int _RefreshTokenExpiryDays = 14;

    public async Task<Result<AuthResponse>> GetTokenAsync(string Email, string Password, CancellationToken cancellationToken = default)
    {
        var User = await _userManager.FindByEmailAsync(Email);

        if (User is null)
        {
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
        }

        var result = await _signInManager.PasswordSignInAsync(User, Password, false, false);

        if (result.Succeeded) {
            var (Token, ExpiresIn) = _JwtProvider.GenerateJwtToken(User);

            var RefreshToken = GenerateRefreshToken();

            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_RefreshTokenExpiryDays);

            User.RefreshTokens.Add(new RefreshToken
            {
                Token = RefreshToken,
                ExpiresOn = refreshTokenExpiration,
            });

            await _userManager.UpdateAsync(User);

            var Response = new AuthResponse(User.Id, User.Email, User.FirstName, User.LastName, Token, ExpiresIn, RefreshToken, refreshTokenExpiration);

            return Result.Success(Response);
        }

        return Result.Failure<AuthResponse>(result.IsNotAllowed ? UserErrors.EmailNotConfirmed : UserErrors.InvalidCredentials);
    }

    public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string Token, string RefreshToken, CancellationToken cancellationToken = default)
    {
        var UserId = _JwtProvider.ValidateToken(Token);

        if (UserId is null)
        {
            return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);
        }

        var User  = await _userManager.FindByIdAsync(UserId);

        if (User is null)
        {
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
        }

        var storedRefreshToken = User.RefreshTokens.FirstOrDefault(rt => rt.Token == RefreshToken && rt.IsActive);

        if (storedRefreshToken is null)
        {
            return Result.Failure<AuthResponse>(UserErrors.InvalidRefreshToken);
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

        var response = new AuthResponse(User.Id, User.Email, User.FirstName, User.LastName, NewToken, ExpiresIn, NewRefreshToken, refreshTokenExpiration);

        return Result.Success(response);
    }

    public async Task<Result> RevokeRefreshTokenAsync(string Token, string RefreshToken, CancellationToken cancellationToken = default)
    {
        var UserId = _JwtProvider.ValidateToken(Token);

        if (UserId is null)
        {
            return Result.Failure(UserErrors.InvalidJwtToken);
        }

        var User = await _userManager.FindByIdAsync(UserId);

        if (User is null)
        {
            return Result.Failure(UserErrors.InvalidCredentials);
        }

        var storedRefreshToken = User.RefreshTokens.FirstOrDefault(rt => rt.Token == RefreshToken && rt.IsActive);

        if (storedRefreshToken is null)
        {
            return Result.Failure(UserErrors.InvalidRefreshToken);
        }

        storedRefreshToken.RevokedOn = DateTime.UtcNow;

        await _userManager.UpdateAsync(User);

        return Result.Success();
    }

    public async Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var EmailIsExists = await _userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken);

        if (EmailIsExists) {
            return Result.Failure(UserErrors.DuplicatedEmail);
        }

        var User = request.Adapt<ApplicationUser>();

        var result = await _userManager.CreateAsync(User, request.Password);

        if (result.Succeeded)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(User);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            _logger.LogInformation("Confirmation code: {Code}", code);

            // TODO: Send confirmation email with the code

            return Result.Success();
        }

        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));

    }


    public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
    {
        if (await _userManager.FindByIdAsync(request.UserId) is not { } user)
        {
            return Result .Failure(UserErrors.InvalidCode);
        }

        if (user.EmailConfirmed)
        {
            return Result.Failure(UserErrors.DuplictedConfirmation);
        }

        var code = request.Code;

        try 
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        }
        catch (FormatException)
        {
            return Result.Failure(UserErrors.InvalidCode);
        }

        var result = await _userManager.ConfirmEmailAsync(user, code);

        if (result.Succeeded)
        {
            return Result.Success();
        }

        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));

    }


    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    
}
