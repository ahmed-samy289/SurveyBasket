using SurveyBasket.Contracts.Polls;

namespace SurveyBasket.Services;

public interface IPollService
{
    Task<IEnumerable<Poll>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<PollResponse>> GetAsync(int Id, CancellationToken cancellationToken = default);
    Task<Result<PollResponse>> AddAsync(PollRequest request , CancellationToken cancellationToken = default);
    Task<Result> PutAsync(int Id, PollRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(int Id,CancellationToken cancellationToken = default);
    Task<Result> TogglePublishAsync(int Id,CancellationToken cancellationToken = default);

}
