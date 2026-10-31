using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SurveyBasket.Abstractions.Consts;
using SurveyBasket.Contracts.Roles;
using SurveyBasket.Contracts.Users;
using SurveyBasket.Errors;
using System.Reflection.Metadata.Ecma335;

namespace SurveyBasket.Services;

public class UserService(UserManager<ApplicationUser> userManager ,IRoleService roleService, ApplicationDbContext context) : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IRoleService _roleService = roleService;
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

    public async Task<Result<UserResponse>> GetUserByIdAsync(string Id)
    {
        if (await _userManager.FindByIdAsync(Id) is not { } user)
        {
            return Result.Failure<UserResponse>(UserErrors.UserNotFound);
        }

        var roles = await _userManager.GetRolesAsync(user);

        var response = (user,roles).Adapt<UserResponse>();

        return Result.Success(response);
    }

    public async Task<Result<UserResponse>> AddAsync(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var emailExists = await _userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken);
        if (emailExists)
        {
            return Result.Failure<UserResponse>(UserErrors.DuplicatedEmail);
        }

        var allowedRoles = await _roleService.GetAllAsync(cancellationToken: cancellationToken);

        if (request.Roles.Except(allowedRoles.Select(x => x.Name)).Any())
        {
            return Result.Failure<UserResponse>(UserErrors.InvalidRoles);
        }

        var user = request.Adapt<ApplicationUser>();

        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRolesAsync(user, request.Roles);

            var response = (user, request.Roles).Adapt<UserResponse>();

            return Result.Success(response);
        }

        var error = result.Errors.First();
        return Result.Failure<UserResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }


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
