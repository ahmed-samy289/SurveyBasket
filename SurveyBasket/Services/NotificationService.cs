using Microsoft.AspNetCore.Identity.UI.Services;
using SurveyBasket.Helpers;

namespace SurveyBasket.Services;

public class NotificationService(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    HttpContextAccessor httpContextAccessor,
    IEmailSender emailSender
    ) : INotificationService
{
    private readonly ApplicationDbContext _context = context;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly HttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IEmailSender _emailSender = emailSender;

    public async Task SendNewPollsNotification(int? pollId = null)
    {
        IEnumerable<Poll> polls = [];

        if (pollId.HasValue)
        {
            var poll  = await _context.Polls.SingleOrDefaultAsync(p => p.Id == pollId && p.IsPublished);

            polls = [poll!];
        }
        else
        {
            polls  = await _context.Polls
                .Where(p => p.IsPublished && p.StartsAt == DateOnly.FromDateTime(DateTime.UtcNow))
                .AsNoTracking()
                .ToListAsync();
        }

        //TODO : select members only

        var users = await _userManager.Users.ToListAsync();

        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

        foreach (var poll in polls)
        {
            foreach (var user in users)
            {
                var placeholder = new Dictionary<string, string>
                {
                    { "{{name}}", user.FirstName },
                    { "{{pollTitle}}", poll.Title },
                    { "{{endDate}}", poll.EndsAt.ToString() },
                    { "{{url}}", $"{origin}/polls/start/{poll.Id}" }
                };

                var body = EmailBodyBuilder.GenerateEmailBody("PollNotification", placeholder);

                await _emailSender.SendEmailAsync(user.Email!, "New Poll Available", body);

            }
        }

    }
}
