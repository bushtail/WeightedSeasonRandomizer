#pragma warning disable CS0618 // Type or member is obsolete

using System.Reflection;
using System.Text.Json;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;

namespace WeightedSeasonRandomizer;

[Injectable(TypePriority = OnLoadOrder.PostSptModLoader)]
public class WeightedSeasonRandomizer(ModHelper modHelper, ConfigServer configServer, ISptLogger<WeightedSeasonRandomizer> logger) : IOnLoad
{
    private static readonly Random _random = new();
    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };
    private static ISptLogger<WeightedSeasonRandomizer>? _logger;
    private static WSRConfig? Config;
    private static WeatherConfig? WeatherCfg;
    
    public Task OnLoad()
    {
        _logger = logger;
        WeatherCfg = configServer.GetConfig<WeatherConfig>();
        if (WeatherCfg == null) throw new NullReferenceException(nameof(WeatherConfig));
        var pathToMod = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());
        var configPath = Path.Combine(pathToMod, "config.json");
        
        if (!File.Exists(configPath))
        {
            Config = new WSRConfig();
            var json = JsonSerializer.Serialize(Config, _jsonOptions);
            File.WriteAllText(configPath, json);
        }
        else
        {
            Config = modHelper.GetJsonDataFromFile<WSRConfig>(pathToMod, configPath);
        }
        new GetLocalWeather().Enable();
        PushNewSeason();
        return Task.CompletedTask;
    }

    public static void PushNewSeason()
    {
        if (WeatherCfg != null) WeatherCfg.OverrideSeason = SelectNewSeason();
    }
    
    private static Season SelectNewSeason()
    {
        if (_logger == null) throw new NullReferenceException(nameof(_logger));
        if (Config == null) return Season.SUMMER;

        var totalWeight = Config.AsEnumerable().Sum(weight => weight.Value);

        var roll = _random.Next(totalWeight);

        foreach (var kvp in Config.AsEnumerable())
        {
            if (roll < kvp.Value)
            {
                _logger.Info($"[WSR] Randomly selected {kvp.Key.ToString()} as the next season.");
                return kvp.Key;
            }
            roll -= kvp.Value;
        }
        return Season.SUMMER; // Fall back to summer.
    }
}