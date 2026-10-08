using SurveyBasket.Contracts.Roles;
using Microsoft.AspNetCore.Identity;
using SurveyBasket.Errors;

namespace SurveyBasket.Services;

public class RoleService(RoleManager<ApplicationRole> roleManager):IRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;

    public async Task<IEnumerable<RoleResponse>> GetAllAsync(bool? includeDisabled = false, CancellationToken cancellationToken = default) =>
        await _roleManager.Roles
            .Where(x => !x.IsDefault && (!x.IsDeleted || (includeDisabled.HasValue && includeDisabled.Value)))
            .ProjectToType<RoleResponse>()
            .ToListAsync(cancellationToken);

    public async Task<Result<RoleDetailResponse>> GetAsync(string id)
    {
        if (await _roleManager.FindByIdAsync(id) is not { } role)
        {
            return Result.Failure<RoleDetailResponse>(RoleErrors.RoleNotFound);
        }

        var permissions = await _roleManager.GetClaimsAsync(role);

        var response = new RoleDetailResponse(
            role.Id,
            role.Name!,
            role.IsDeleted,
            permissions.Select(x => x.Value)
        );

        return Result.Success(response);
    }
}
