using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using SurveyBasket.Abstractions;
using SurveyBasket.Abstractions.Consts;
using SurveyBasket.Authentication;
using SurveyBasket.Errors;
using SurveyBasket.Helpers;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;

namespace SurveyBasket.Services;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ILogger<AuthService> logger,
    IJwtProvider jwtProvider,
    IEmailSender emailSender,
    IHttpContextAccessor httpContextAccessor,
    ApplicationDbContext context
    ) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly ILogger<AuthService> _logger = logger;
    private readonly IJwtProvider _JwtProvider = jwtProvider;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ApplicationDbContext _context = context;
    private readonly int _RefreshTokenExpiryDays = 14;

    public async Task<Result<AuthResponse>> GetTokenAsync(string Email, string Password, CancellationToken cancellationToken = default)
    {
        var User = await _userManager.FindByEmailAsync(Email);

        if (User is null)
        {
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
        }

        if (User.IsDisabled)
        {
            return Result.Failure<AuthResponse>(UserErrors.UserDisabled);
        }

        var result = await _signInManager.PasswordSignInAsync(User, Password, false, false);

        if (result.Succeeded) {

            var (userRoles, userPermissions) = await GetUserRolesAndPermessions(User, cancellationToken);

            var (Token, ExpiresIn) = _JwtProvider.GenerateToken(User, userRoles, userPermissions);

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

        if (User.IsDisabled)
        {
            return Result.Failure<AuthResponse>(UserErrors.UserDisabled);
        }

        var storedRefreshToken = User.RefreshTokens.FirstOrDefault(rt => rt.Token == RefreshToken && rt.IsActive);

        if (storedRefreshToken is null)
        {
            return Result.Failure<AuthResponse>(UserErrors.InvalidRefreshToken);
        }

        storedRefreshToken.RevokedOn = DateTime.UtcNow;

        var (userRoles, userPermissions) = await GetUserRolesAndPermessions(User, cancellationToken);

        var (NewToken, ExpiresIn) = _JwtProvider.GenerateToken(User, userRoles, userPermissions);

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

            await SendConfirmationEmailAsync(User, code);  

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
            await _userManager.AddToRoleAsync(user, DefaultRoles.Member);
            return Result.Success();
        }

        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));

    }

    public async Task<Result> ResendConfirmationEmailAsync(ResendComfirmationEmailRequest request) {

        if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
        {
            return Result.Success(); // for security reasons we return success even if the email is not found
        }

        if (user.EmailConfirmed)
        {
            return Result.Failure(UserErrors.DuplictedConfirmation);
        }

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        _logger.LogInformation("Confirmation code: {Code}", code);

        await SendConfirmationEmailAsync(user, code);

        return Result.Success();
    }

    public async Task<Result> SendResetPasswordAsync(string Email)
    {
        if (await _userManager.FindByEmailAsync(Email) is not { } user)
        {
            return Result.Success(); // for security reasons we return success even if the email is not found
        }

        if (!user.EmailConfirmed)
        {
            return Result.Failure(UserErrors.EmailNotConfirmed);
        }

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        _logger.LogInformation("Reset code: {Code}", code);

        await SendResetPasswordEmailAsync(user, code);

        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null || !user.EmailConfirmed)
        {
            return Result.Failure(UserErrors.InvalidCode);
        }

        IdentityResult result;

        try
        {
            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
            result = await _userManager.ResetPasswordAsync(user, code, request.NewPassword);
        }
        catch (FormatException)
        {
            result = IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken());
        }


        if (result.Succeeded)
        {
            return Result.Success();
        }

        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status401Unauthorized));
    }   

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private async Task SendConfirmationEmailAsync(ApplicationUser user, string code)
    {
        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

        var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation",
            new Dictionary<string, string>
            {
                    { "{{name}}", user.FirstName },
                    { "{{action_url}}", $"{origin}/auth/email-confirmation?userId={user.Id}&code={code}" }
            });

        BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "Confirm your email", emailBody));

        await Task.CompletedTask;
    }

    private async Task SendResetPasswordEmailAsync(ApplicationUser user, string code)
    {
        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

        var emailBody = EmailBodyBuilder.GenerateEmailBody("ForgetPassword",
            new Dictionary<string, string>
            {
                    { "{{name}}", user.FirstName },
                    { "{{action_url}}", $"{origin}/auth/reset-password?userId={user.Id}&code={code}" }
            });

        BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "Reset your password", emailBody));

        await Task.CompletedTask;
    }

    private async Task<(IEnumerable<string> roles , IEnumerable<string> permissions)> GetUserRolesAndPermessions(ApplicationUser user,CancellationToken cancellationToken)
    {
        var userRoles = await _userManager.GetRolesAsync(user);

        //var userPermissions = await _context.Roles
        //    .Join(_context.RoleClaims,
        //          Role => Role.Id,
        //          claim => claim.RoleId,
        //          (Role, claim) => new { Role, claim })
        //    .Where(x => userRoles.Contains(x.Role.Name!))
        //    .Select(x => x.claim.ClaimValue!)
        //    .Distinct()
        //    .ToListAsync(cancellationToken);

        var userPermissions = await (from r in _context.Roles
                                     join p in _context.RoleClaims
                                     on r.Id equals p.RoleId
                                     where userRoles.Contains(r.Name!)
                                     select p.ClaimValue!)
                                     .Distinct()
                                     .ToListAsync(cancellationToken);

        return (userRoles, userPermissions );
    }

}
