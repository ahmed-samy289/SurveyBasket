

using SurveyBasket.Errors;

namespace SurveyBasket.Services;

public class PollService(ApplicationDbContext context) : IPollService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<Poll>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _context.Polls.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<Result<PollResponse>> GetAsync(int Id, CancellationToken cancellationToken = default)
    {
        var poll = await _context.Polls.FindAsync(Id, cancellationToken);

        return poll is not null
            ? Result.Success(poll.Adapt<PollResponse>()) 
            : Result.Failure<PollResponse>(PollErrors.NotFound);
    }

    public async Task<Result<PollResponse>> AddAsync(PollRequest request, CancellationToken cancellationToken = default)
    {
        var isExisting = await _context.Polls.AnyAsync(p => p.Title == request.Title, cancellationToken);

        if (isExisting)
        {
            return Result.Failure<PollResponse>(PollErrors.DuplicatedTitle);
        }

        var poll = request.Adapt<Poll>();

        await _context.Polls.AddAsync(poll, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(poll.Adapt<PollResponse>());
    }
    
    public async Task<Result> PutAsync(int Id, PollRequest request, CancellationToken cancellationToken = default)
    {
        var isExisting = await _context.Polls.AnyAsync(p => p.Title == request.Title && p.Id != Id, cancellationToken);

        if (isExisting)
        {
            return Result.Failure(PollErrors.DuplicatedTitle);
        }

        var CurrentPoll = await _context.Polls.FindAsync(Id, cancellationToken);

        if (CurrentPoll is null)
        {
            return Result.Failure(PollErrors.NotFound);
        }

        CurrentPoll.Title = request.Title;
        CurrentPoll.Summary = request.Summary;
        CurrentPoll.StartsAt = request.StartsAt;
        CurrentPoll.EndsAt = request.EndsAt;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();

    }

    public async Task<Result> DeleteAsync(int Id, CancellationToken cancellationToken = default)
    {
        var Poll = await _context.Polls.FindAsync(Id, cancellationToken);

        if (Poll is null)
        {
            return Result.Failure(PollErrors.NotFound);
        }

        _context.Remove(Poll);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> TogglePublishAsync(int Id, CancellationToken cancellationToken = default)
    {
        var Poll = await _context.Polls.FindAsync(Id, cancellationToken);

        if (Poll is null)
        {
            return Result.Failure(PollErrors.NotFound);
        }

        Poll.IsPublished = !Poll.IsPublished;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();

    }
}
