

namespace SurveyBasket.Services;

public class PollService(ApplicationDbContext context) : IPollService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<Poll>> GetAllAsync(CancellationToken cancellationToken = default)=>
        await _context.Polls.AsNoTracking().ToListAsync(cancellationToken);


    public async Task<Poll?> GetAsync(int Id, CancellationToken cancellationToken = default)
    {
        return await _context.Polls.FindAsync(Id,cancellationToken);
    }

    public async Task<Poll> AddAsync(Poll poll, CancellationToken cancellationToken = default)
    {
        await _context.Polls.AddAsync(poll,cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return poll;
    }

    public async Task<bool> PutAsync(int Id, Poll poll, CancellationToken cancellationToken = default)
    {
        var CurrentPoll = await GetAsync(Id,cancellationToken);

        if (CurrentPoll is null)
        {
            return false;
        }

        CurrentPoll.Title = poll.Title;
        CurrentPoll.Summary = poll.Summary;
        CurrentPoll.StartsAt = poll.StartsAt;
        CurrentPoll.EndsAt  = poll.EndsAt;

        await _context.SaveChangesAsync(cancellationToken);

        return true;

    }

    public async Task<bool> DeleteAsync(int Id,CancellationToken cancellationToken = default)
    {
        var Poll = await GetAsync(Id,cancellationToken);

        if (Poll is null)
        {
            return false;
        }

        _context.Remove(Poll);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> TogglePublishAsync(int Id, CancellationToken cancellationToken = default)
    {
        var Poll = await GetAsync(Id, cancellationToken);

        if (Poll is null)
        {
            return false;
        }

        Poll.IsPublished = !Poll.IsPublished;

        await _context.SaveChangesAsync(cancellationToken);

        return true;

    }
}
