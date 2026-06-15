
using SurveyBasket.Abstractions.Consts;

namespace SurveyBasket.Persistence.EntitiesConfigrations;

public class UserRoleConfigrations : IEntityTypeConfiguration<IdentityUserRole<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
    {
        builder.HasData(
            new IdentityUserRole<string>
            {
                UserId = DefaultUsers.AdminId,
                RoleId = DefaultRoles.AdminRoleId
            }
        );
    }
}
