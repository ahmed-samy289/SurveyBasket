namespace SurveyBasket.Services;

public interface IPollService
{
    IEnumerable<Poll> GetAll();

    Poll? Get(int Id);

    Poll Add(Poll poll);

    bool Put(int Id, Poll poll);
    bool Delete(int Id);

}
