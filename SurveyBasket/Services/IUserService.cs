using SurveyBasket.Contracts.Users;

namespace SurveyBasket.Services;

public interface IUserService
{
    Task<Result<GetUserProfileResponse>> GetUserProfileAsync(string UserId);
    Task<Result> UpdateUserProfileAsync(string Id, UpdateUserProfileRequest request);
    Task<Result> ChangePasswordAsync(string UserId, ChangePasswordRequest request);

}
