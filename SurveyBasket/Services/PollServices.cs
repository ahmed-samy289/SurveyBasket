

namespace SurveyBasket.Services;

public class PollService : IPollService
{
    private static readonly List<Poll> _Polls = [
        new Poll{
            Id = 1,
            Title = "Poll 1",
            Description = "First Poll"
        }
    ];

    public IEnumerable<Poll> GetAll()
    {
        return _Polls;
    }

    public Poll? Get(int Id)
    {
        return _Polls.SingleOrDefault(X => X.Id == Id);
    }

    public Poll Add(Poll poll)
    {
        poll.Id = _Polls.Count + 1;
        _Polls.Add(poll);
        return poll;
    }

    public bool Put(int Id, Poll poll)
    {
        var CurrentPoll = Get(Id);

        if (CurrentPoll is null)
        {
            return false;
        }

        CurrentPoll.Title = poll.Title;
        CurrentPoll.Description = poll.Description;

        return true;

    }

    public bool Delete(int Id)
    {
        var Poll = Get(Id);

        if (Poll is null)
        {
            return false;
        }

        _Polls.Remove(Poll);

        return true;
    }
}
