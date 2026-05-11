using SurveyBasket.Contracts.Users;

namespace SurveyBasket.Services;

public class UserService(UserManager<ApplicationUser> userManager) : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<Result<GetUserProfileResponse>> GetUserProfileAsync(string UserId)
    {
        var user = await _userManager
            .Users.Where(x=>x.Id == UserId)
            .ProjectToType<GetUserProfileResponse>()
            .SingleAsync();

        return Result.Success(user);
    }

    public async Task<Result> UpdateUserProfileAsync(string Id , UpdateUserProfileRequest request)
    {
        var user = await _userManager.FindByIdAsync(Id);

        user = request.Adapt(user);

        await _userManager.UpdateAsync(user!);

        return Result.Success();
    }

    public async Task<Result> ChangePasswordAsync(string UserId, ChangePasswordRequest request)
    {
        var user = await _userManager.FindByIdAsync(UserId);

        var result = await _userManager.ChangePasswordAsync(user!, request.CurrentPassword, request.NewPassword);

        if (result.Succeeded)
        {
            return Result.Success();        
        }

        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));

    }


}
