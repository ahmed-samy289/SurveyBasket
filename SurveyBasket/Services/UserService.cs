using Microsoft.EntityFrameworkCore;
using SurveyBasket.Abstractions.Consts;
using SurveyBasket.Contracts.Users;

namespace SurveyBasket.Services;

public class UserService(UserManager<ApplicationUser> userManager , ApplicationDbContext context) : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default) =>
            await (
                from u in _context.Users
                join ur in _context.UserRoles
                    on u.Id equals ur.UserId
                join r in _context.Roles
                    on ur.RoleId equals r.Id into roles
                where !roles.Any(x => x.Name == DefaultRoles.Member)
                select new {
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    u.IsDisabled,
                    Roles = roles.Select(x => x.Name!).ToList()
                }
                ).GroupBy(x => new { x.Id, x.FirstName, x.LastName, x.Email, x.IsDisabled })
                 .Select(x=> new UserResponse (
                        x.Key.Id,
                        x.Key.FirstName,
                        x.Key.LastName,
                        x.Key.Email,
                        x.Key.IsDisabled,
                        x.SelectMany(y => y.Roles)
                 ))
                 .ToListAsync(cancellationToken);

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
        //var user = await _userManager.FindByIdAsync(Id);

        //user = request.Adapt(user);

        //await _userManager.UpdateAsync(user!);

        await _userManager.Users
            .Where(x => x.Id == Id)
            .ExecuteUpdateAsync(setters =>
                setters
                 .SetProperty(x => x.FirstName, request.FirstName)
                 .SetProperty(x => x.LastName, request.LastName)
            );

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
