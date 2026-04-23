
namespace SurveyBasket.Controllers;

[Route("/[controller]")]
[ApiController]
public class AuthController(IAuthService authService,ILogger<AuthController> logger) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly ILogger<AuthController> _logger = logger;

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation( "Logging with email: {Email} and password: {Password}",request.Email,request.Password);

        var authResult = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);

        return authResult.IsSuccess
            ? Ok(authResult.Value)
            : authResult.ToProblem();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var authResult = await _authService.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        return authResult.IsSuccess 
            ? Ok(authResult.Value) 
            : authResult.ToProblem();
    }
    
    
    [HttpPost("revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshToken(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var isRevoked = await _authService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        return isRevoked.IsSuccess 
            ? Ok() 
            : isRevoked.ToProblem();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var authResult = await _authService.RegisterAsync(request, cancellationToken);

        return authResult.IsSuccess
            ? Ok()
            : authResult.ToProblem();
    }
   
    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(ConfirmEmailRequest request)
    {
        var result = await _authService.ConfirmEmailAsync(request);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }
}
