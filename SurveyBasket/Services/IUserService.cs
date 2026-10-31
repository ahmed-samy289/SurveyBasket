using SurveyBasket.Contracts.Users;

namespace SurveyBasket.Services;

public interface IUserService
{
    Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<UserResponse>> GetUserByIdAsync(string Id);
    Task<Result<UserResponse>> AddAsync(CreateUserRequest request, CancellationToken cancellationToken);

    Task<Result<GetUserProfileResponse>> GetUserProfileAsync(string UserId);
    Task<Result> UpdateUserProfileAsync(string Id, UpdateUserProfileRequest request);
    Task<Result> ChangePasswordAsync(string UserId, ChangePasswordRequest request);

}
