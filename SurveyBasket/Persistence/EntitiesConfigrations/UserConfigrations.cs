
using SurveyBasket.Abstractions.Consts;

namespace SurveyBasket.Persistence.EntitiesConfigrations;

public class UserConfigrations : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.OwnsMany(x => x.RefreshTokens)
            .ToTable("Refreshtokens")
            .WithOwner()
            .HasForeignKey("UserId");


        builder.Property(x => x.FirstName)
            .HasMaxLength(100);

        builder.Property(x => x.LastName)
            .HasMaxLength(100);

        var passwordHasher = new PasswordHasher<ApplicationUser>();

        builder.HasData(new ApplicationUser
        {
            Id = DefaultUsers.AdminId,
            FirstName = "Admin",
            LastName = "User",
            UserName = DefaultUsers.AdminEmail,
            NormalizedUserName = DefaultUsers.AdminEmail.ToUpper(),
            Email = DefaultUsers.AdminEmail,
            NormalizedEmail = DefaultUsers.AdminEmail.ToUpper(),
            EmailConfirmed = true,
            SecurityStamp = DefaultUsers.AdminSecurityStamp,
            ConcurrencyStamp = DefaultUsers.AdminConcurrencyStamp,
            PasswordHash = "AQAAAAIAAYagAAAAEAq0Q0hY08fU2ESndaVUqNTEEcnGUmGDL/Ra2VAVuDFBNmXbcpDt6iLw9ttMjxzs0A=="
        });
    }
}
