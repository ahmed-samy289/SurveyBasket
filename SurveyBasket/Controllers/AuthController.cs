
namespace SurveyBasket.Controllers;

[Route("/[controller]")]
[ApiController]
public class AuthController(IAuthService authService,ILogger<AuthController> logger) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly ILogger<AuthController> _logger = logger;

    [HttpPost]
    public async Task<IActionResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation( "Logging with email: {Email} and password: {Password}",request.Email,request.Password);

        var authResult = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);

        return authResult.IsSuccess
            ? Ok(authResult.Value)
            : authResult.ToProblem();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var authResult = await _authService.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        return authResult.IsSuccess 
            ? Ok(authResult.Value) 
            : authResult.ToProblem();
    }
    
    
    [HttpPost("revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var isRevoked = await _authService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        return isRevoked.IsSuccess 
            ? Ok() 
            : isRevoked.ToProblem();
    }
}
