using System.Collections.Generic;
using System.Linq;
using Dalamud.Game;
using Dalamud.Game.ClientState.Fates;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using OccultCrescentHelper.Attributes;
using OccultCrescentHelper.Windows;

namespace OccultCrescentHelper;

public sealed class OccultCrescentHelper : IDalamudPlugin
{
    [PluginService]
    internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;

    [PluginService]
    internal static ICommandManager CommandManager { get; private set; } = null!;

    [PluginService]
    internal static IToastGui Toast { get; private set; } = null!;

    [PluginService]
    internal static IClientState ClientState { get; private set; } = null!;

    [PluginService]
    internal static IDataManager DataManager { get; private set; } = null!;

    [PluginService]
    public static IFramework Framework { get; private set; } = null!;

    [PluginService]
    internal static IChatGui Chat { get; private set; } = null!;

    [PluginService]
    internal static IPluginLog Log { get; private set; } = null!;

    [PluginService]
    internal static IGameInteropProvider GameInteropProvider { get; private set; } = null!;

    [PluginService]
    internal static ISigScanner SigScanner { get; private set; } = null!;

    [PluginService]
    public static IObjectTable ObjectTable { get; private set; } = null!;
    [PluginService]
    internal static IFateTable FateTable { get; private set; } = null!;

    private int LastSeenWeatherId = -1;
    private List<IFate> LastFateList = [];

    public Configuration Configuration { get; init; }
    private readonly PluginCommandManager<OccultCrescentHelper> commandManager;
    public readonly WindowSystem WindowSystem = new("OccultCrescentHelper");
    private MainWindow MainWindow { get; init; }
    private ForkedTowerWindow ForkedTowerWindow { get; init; }

    public OccultCrescentHelper()
    {
        commandManager = new PluginCommandManager<OccultCrescentHelper>(this, CommandManager);
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        MainWindow = new MainWindow(this);
        ForkedTowerWindow = new ForkedTowerWindow(this);
        WindowSystem.AddWindow(MainWindow);
        WindowSystem.AddWindow(ForkedTowerWindow);


        PluginInterface.UiBuilder.Draw += DrawUI;

        PluginInterface.UiBuilder.OpenMainUi += ToggleMainWindow;
        PluginInterface.UiBuilder.OpenConfigUi += ToggleMainWindow;

        ClientState.TerritoryChanged += OnTerritoryChange;

        OnTerritoryChange(ClientState.TerritoryType);
    }

    private unsafe void OnTerritoryChange(ushort territoryId)
    {
        if (Common.IsPlayerInSouthHorn(territoryId, AgentMap.Instance()->CurrentMapId))
        {
            Log.Verbose($"Current Weather: {WeatherManager.Instance()->GetCurrentWeather()}");
            LastSeenWeatherId = WeatherManager.Instance()->GetCurrentWeather();
            OnWeatherChange(LastSeenWeatherId);
            Framework.Update += FateTableWatcher;
            Framework.Update += CETableWatcher;
            Framework.Update += WeatherWatcher;
        }
        else
        {
            Framework.Update -= FateTableWatcher;
            Framework.Update -= CETableWatcher;
            Framework.Update += WeatherWatcher;
        }
    }

    private unsafe void WeatherWatcher(IFramework framework)
    {
        if (LastSeenWeatherId != WeatherManager.Instance()->GetCurrentWeather())
        {
            Log.Verbose($"Weather Change: {LastSeenWeatherId} -> {WeatherManager.Instance()->GetCurrentWeather()}");
            LastSeenWeatherId = WeatherManager.Instance()->GetCurrentWeather();
            OnWeatherChange(LastSeenWeatherId);
        }
    }

    private void OnWeatherChange(int newWeatherId)
    {
        if (newWeatherId == Constants.OccultCrescentAuroralMirageWeatherId)
        {
            if (Configuration.PlayFTSfx)
            {
                UIGlobals.PlaySoundEffect(Configuration.FTSfx);
            }

            if (Configuration.PlayFTSfx)
            {
                var mapLink = SeString.CreateMapLink(ClientState.TerritoryType, ClientState.MapId, 63 * 1000, 4 * 1000);
                var payload = new SeStringBuilder()
                              .AddUiForeground(500)
                              .AddText("Auroral Mirages")
                              .AddUiGlowOff()
                              .AddUiForegroundOff()
                              .BuiltString
                              .Append(mapLink);

                Toast.ShowQuest(payload);
                Chat.Print(new XivChatEntry { Message = payload });
            }
        }
    }

    private unsafe void CETableWatcher(IFramework framework) { }

    private void FateTableWatcher(IFramework framework)
    {
        if (!FateTable.SequenceEqual<IFate>(LastFateList))
        {
            var newFateList = FateTable.Except<IFate>(LastFateList)
                                       .Where<IFate>(nFate => nFate.FateId != Constants.OccultCrescentBunnyFateId)
                                       .ToList();
            if (newFateList.Count() > 0)
            {
                OnFateChange(newFateList);
            }
        }

        LastFateList = [.. FateTable];
    }

    private void OnFateChange(IEnumerable<IFate> newFateList)
    {
        if (Configuration.PlayFateSfx)
        {
            UIGlobals.PlaySoundEffect(Configuration.FateSfx);
        }

        if (Configuration.ShowFateToast)
        {
            foreach (var fate in newFateList)
            {
                var mapLink = SeString.CreateMapLink(ClientState.TerritoryType, ClientState.MapId,
                                                     (int)fate.Position.X * 1000, (int)fate.Position.Z * 1000);
                var payload = new SeStringBuilder()
                              .AddUiForeground(500)
                              .AddText(fate.Name.ToString())
                              .AddUiGlowOff()
                              .AddUiForegroundOff()
                              .BuiltString
                              .Append(mapLink);
                Log.Verbose(
                    $"New Fate: {fate.Name} {fate.FateId} {fate.Position.X} {fate.Position.Y} {fate.Position.Z}");

                Toast.ShowQuest(payload);
                Chat.Print(new XivChatEntry { Message = payload });
            }
        }
    }

    private void FTAreaWatcher(IFramework framework)
    {
        if (ForkedTowerWindow.IsOpen)
        {

        }
    }

    
    [Command("/och")]
    [HelpMessage("Open Config window.")]
    private void OnMainCommand(string command, string arguments)
    {
        ToggleMainWindow();
    }

    
    [Command("/ochft")]
    [HelpMessage("Open Forked Tower Entry window to check number of player inside entry area.")]
    private void OnFTCommand(string command, string arguments)
    {
        ToggleFTWindow();
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();
        MainWindow.Dispose();
        ForkedTowerWindow.Dispose();
        commandManager.Dispose();
        Framework.Update -= FateTableWatcher;
        Framework.Update -= CETableWatcher;
        Framework.Update += WeatherWatcher;
    }

    private void DrawUI() => WindowSystem.Draw();
    public void ToggleFTWindow() => ForkedTowerWindow.Toggle();
    public void ToggleMainWindow() => MainWindow.Toggle();
}
