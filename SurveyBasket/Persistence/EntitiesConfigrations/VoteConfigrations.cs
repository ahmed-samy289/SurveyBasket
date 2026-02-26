
namespace SurveyBasket.Persistence.EntitiesConfigrations;

public class VoteConfigrations : IEntityTypeConfiguration<Vote>
{
    public void Configure(EntityTypeBuilder<Vote> builder)
    {
        builder.HasIndex(x => new { x.UserId,x.PollId }).IsUnique();
    }
}
