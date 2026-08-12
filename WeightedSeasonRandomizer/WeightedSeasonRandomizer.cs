#pragma warning disable CS0618 // Type or member is obsolete

using System.Reflection;
using System.Text.Json;
using JetBrains.Annotations;
using SPTarkov.Common.Models.Logging;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Helpers.Server;
using SPTarkov.Server.Core.Models.Enums;
using SPTarkov.Server.Core.Models.Spt.Config;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;
using WeightedSeasonRandomizer.Patches;

namespace WeightedSeasonRandomizer;

[Injectable(TypePriority = OnLoadOrder.PostLoad + 1), UsedImplicitly]
public class WeightedSeasonRandomizer(ModHelper modHelper, WeatherConfig weatherConfig, ISptLogger<WeightedSeasonRandomizer> logger) : IOnLoad
{
    private static readonly Random Random = new();
    private static readonly JsonSerializerOptions JSONOptions = new() { WriteIndented = true };
    private static ISptLogger<WeightedSeasonRandomizer>? _logger;
    private static WsrConfig? _config;
    private static WeatherConfig? _weatherCfg;
    
    public Task OnLoadAsync(CancellationToken cancellationToken)
    {
        _logger = logger;
        _weatherCfg = weatherConfig;
        
        var pathToMod = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());
        var configPath = Path.Combine(pathToMod, "config.json");
        
        if (!File.Exists(configPath))
        {
            _config = new WsrConfig();
            var json = JsonSerializer.Serialize(_config, JSONOptions);
            File.WriteAllText(configPath, json);
        }
        else
        {
            _config = modHelper.GetJsonDataFromFile<WsrConfig>(pathToMod, configPath);
        }
        new GetLocalWeather().Enable();
        PushNewSeason();
        return Task.CompletedTask;
    }

    public static void PushNewSeason()
    {
        _weatherCfg?.OverrideSeason = SelectNewSeason();
    }
    
    private static Season SelectNewSeason()
    {
        if (_logger == null) throw new NullReferenceException(nameof(_logger));
        if (_config == null) return Season.SUMMER;

        var totalWeight = _config.AsEnumerable().Sum(weight => weight.Value);

        var roll = Random.Next(totalWeight);

        foreach (var kvp in _config.AsEnumerable())
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