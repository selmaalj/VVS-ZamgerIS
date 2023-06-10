namespace ooadproject.Models
{
    public interface IActivity
    {
        double GetPointsScored();
        double GetTotalPoints();

        DateTime GetActivityDate();
        string GetActivityName();
        string GetActivityType();
    }
}
