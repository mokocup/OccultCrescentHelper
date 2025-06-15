using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Fates;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.InstanceContent;
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
        if (Common.IsInSouthHorn(territoryId, AgentMap.Instance()->CurrentMapId))
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
            Framework.Update -= WeatherWatcher;
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
            OnForkedTowerCE();
        }
    }

    private void OnForkedTowerCE()
    {
        if (Configuration.PlayFTSfx)
        {
            UIGlobals.PlaySoundEffect(Configuration.FTSfx);
        }

        if (Configuration.PlayFTSfx)
        {
            var mapLink = Common.CreateSouthHornLink(Constants.ForkedTowerBloodCE.Position.X,
                                                     Constants.ForkedTowerBloodCE.Position.Y);
            var payload = new SeStringBuilder()
                          .AddUiForeground(500)
                          .AddText(Constants.ForkedTowerBloodCE.Name)
                          .AddUiForegroundOff()
                          .BuiltString
                          .Append(mapLink);

            Toast.ShowQuest(payload);
            Chat.Print(new XivChatEntry { Message = payload });
        }
    }

    private unsafe void CETableWatcher(IFramework framework)
    {
        var publicContent = PublicContentOccultCrescent.GetInstance();
        if (publicContent == null)
            return;

        var dynamicEvents = publicContent->DynamicEventContainer.Events;

        foreach (var ceEvent in dynamicEvents)
        {
            if (ceEvent.State != DynamicEventState.Register)
                continue;
            // TODO: Detect Forked Tower up instead of relay on weather
            //if (ceEvent.DynamicEventId == Constants.ForkedTowerBloodCE.EventId && ceEvent.StartTimestamp != Constants.ForkedTowerBloodCE.lastStartTime)
            //{

            //    Constants.ForkedTowerBloodCE.lastStartTime = ceEvent.StartTimestamp;
            //    OnForkedTowerCE();
            //    continue;
            //}

            var currentCE =
                Constants.OccultCrescentCEs.FirstOrDefault(cCE => cCE.EventId == ceEvent.DynamicEventId);
            if (currentCE != null && ceEvent.StartTimestamp != currentCE.lastStartTime)
            {
                currentCE.lastStartTime = ceEvent.StartTimestamp;
                OnCEChange(ceEvent);
            }
        }
    }

    private void OnCEChange(DynamicEvent newCE)
    {
        if (Configuration.PlayCESfx)
        {
            UIGlobals.PlaySoundEffect(Configuration.CESfx);
        }

        if (Configuration.ShowCEToast)
        {
            var cCEInformation =
                Constants.OccultCrescentCEs.FirstOrDefault(cCE => cCE.EventId == newCE.DynamicEventId);
            SeString mapLink;
            var payloadBuilder = new SeStringBuilder().AddUiForeground(500);
            if (cCEInformation != null)
            {
                mapLink = cCEInformation.MapLink;
                payloadBuilder = payloadBuilder.AddText(cCEInformation.Name);
            }
            else
            {
                mapLink = Common.CreateSouthHornLink(newCE.MapMarker.Position.X, newCE.MapMarker.Position.Z);
                payloadBuilder = payloadBuilder.AddText(newCE.Name.ToString());
            }

            Log.Verbose(
                $"New CE: {cCEInformation.Name} {cCEInformation.EventId} {cCEInformation.Position.X} {cCEInformation.Position.Y} {newCE.State}");
            var payload = payloadBuilder.AddUiForegroundOff()
                                        .BuiltString
                                        .Append(mapLink);

            Toast.ShowQuest(payload);
            Chat.Print(new XivChatEntry { Message = payload });
        }
    }

    private void FateTableWatcher(IFramework framework)
    {
        if (!FateTable.SequenceEqual<IFate>(LastFateList))
        {
            var newFateList = FateTable.Except<IFate>(LastFateList)
                                       .ToList();
            if (newFateList.Any())
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
                var cFateInformation =
                    Constants.OccultCrescentFates.FirstOrDefault(cFate => cFate.FateId == fate.FateId);
                SeString mapLink;
                var payloadBuilder = new SeStringBuilder().AddUiForeground(500);
                if (cFateInformation != null)
                {
                    mapLink = cFateInformation.MapLink;
                    payloadBuilder = payloadBuilder.AddText(cFateInformation.Name);
                    Log.Verbose(
                        $"New Fate: {cFateInformation.Name} {cFateInformation.FateId} {cFateInformation.Position.X} {cFateInformation.Position.Y}");
                }
                else
                {
                    mapLink = Common.CreateSouthHornLink(fate.Position.X, fate.Position.Z);
                    payloadBuilder = payloadBuilder.AddText(fate.Name.ToString());
                    Log.Debug(
                        $"Fate Not Found In List: {fate.Name} {fate.FateId} {fate.Position.X} {fate.Position.Y} {fate.Position.Z}");
                }

                var payload = payloadBuilder.AddUiForegroundOff()
                                            .BuiltString
                                            .Append(mapLink);

                Toast.ShowQuest(payload);
                Chat.Print(new XivChatEntry { Message = payload });
            }
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
        Framework.Update -= WeatherWatcher;
    }

    private void DrawUI() => WindowSystem.Draw();
    public void ToggleFTWindow() => ForkedTowerWindow.Toggle();
    public void ToggleMainWindow() => MainWindow.Toggle();
}
