
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
    }
}
