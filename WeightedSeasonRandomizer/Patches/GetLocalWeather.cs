#pragma warning disable CS0618 // Type or member is obsolete

using System.Reflection;
using JetBrains.Annotations;
using SPTarkov.Reflection.Patching;
using SPTarkov.Server.Core.Callbacks;

namespace WeightedSeasonRandomizer;

public class GetLocalWeather : AbstractPatch
{
    protected override MethodBase? GetTargetMethod()
    {
        return typeof(WeatherCallbacks).GetMethod(nameof(WeatherCallbacks.GetLocalWeather));
    }

    [PatchPrefix, UsedImplicitly]
    public static bool Prefix()
    {
        Console.WriteLine($"{nameof(GetLocalWeather)} Prefix");
        WeightedSeasonRandomizer.PushNewSeason();
        return true;
    }
}