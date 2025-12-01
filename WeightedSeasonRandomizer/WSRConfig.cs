using SPTarkov.Server.Core.Models.Enums;

namespace WeightedSeasonRandomizer;

public class WSRConfig
{
    public int Winter { get; set; } = 5;
    public int EarlySpring { get; set; } = 10;
    public int Spring { get; set; } = 15;
    public int Storm { get; set; } = 10;
    public int Summer { get; set; } = 30;
    public int Autumn { get; set; } = 20;
    public int LateAutumn { get; set; } = 10;
    

    public IEnumerable<KeyValuePair<Season, int>> AsEnumerable()
    {
        yield return new KeyValuePair<Season, int>(Season.WINTER, Winter);
        yield return new KeyValuePair<Season, int>(Season.SPRING_EARLY, EarlySpring);
        yield return new KeyValuePair<Season, int>(Season.SPRING, Spring);
        yield return new KeyValuePair<Season, int>(Season.STORM, Storm);
        yield return new KeyValuePair<Season, int>(Season.SUMMER, Summer);
        yield return new KeyValuePair<Season, int>(Season.AUTUMN, Autumn);
        yield return new KeyValuePair<Season, int>(Season.AUTUMN_LATE, LateAutumn);
    }
}