
namespace SurveyBasket.Persistence.EntitiesConfigrations;

public class QuestionConfigrations : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.HasIndex(x => new { x.PollId, x.Content }).IsUnique();

        builder.Property(x => x.Content)
            .HasMaxLength(1000);
    }
}
