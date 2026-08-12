using SPTarkov.Server.Core.Models.Enums;

namespace WeightedSeasonRandomizer;

public class WsrConfig
{
    private int Winter { get; set; } = 5;
    private int EarlySpring { get; set; } = 10;
    private int Spring { get; set; } = 15;
    private int Storm { get; set; } = 10;
    private int Summer { get; set; } = 30;
    private int Autumn { get; set; } = 20;
    private int LateAutumn { get; set; } = 10;
    

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