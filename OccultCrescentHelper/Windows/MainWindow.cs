using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Sound;
using FFXIVClientStructs.FFXIV.Client.UI;
using ImGuiNET;
using System;
using System.Numerics;
using System.Threading.Channels;
using static Lumina.Data.Files.ScdFile;

namespace OccultCrescentHelper.Windows;

public class MainWindow : Window, IDisposable
{
    private Configuration Configuration;

    private GameFunction GameFunction;

    // We give this window a constant ID using ###
    // This allows for labels being dynamic, like "{FPS Counter}fps###XYZ counter window",
    // and the window ID will always be "###XYZ counter window" for ImGui
    public MainWindow(OccultCrescentHelper plugin) : base("Occult Crescent Helper##OccultCrescentHelper")
    {
        Flags = ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse;

        SizeConstraints = new WindowSizeConstraints()
        {
            MinimumSize = new Vector2(135, 70),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        Configuration = plugin.Configuration;
        GameFunction = new GameFunction();
    }

    public void Dispose() { }

    public override void PreDraw() { }

    public override void Draw()
    {
        if (ImGui.BeginTabBar("##SettingTabs"))
        {
            var hasValueChange = false;

            hasValueChange |= TabFate();
            //hasValueChange |= TabCE();
            hasValueChange |= TabForkTower();
#if DEBUG
            TabDebug();
#endif

            if (hasValueChange) Configuration.Save();

            ImGui.EndTabBar();
        }
    }

    private bool TabFate()
    {
        var hasValueChange = false;

        if (ImGui.BeginTabItem("Fate##fate"))
        {
            hasValueChange |= ImGui.Checkbox("Show Toast", ref Configuration.ShowFateToast);

            hasValueChange |= ImGui.Checkbox("Play Sound", ref Configuration.PlayFateSfx);

            ImGui.SameLine();

            if (Configuration.PlayFateSfx)
            {
                ImGui.SetNextItemWidth(100.0f * ImGuiHelpers.GlobalScale);
                if (ImGui.BeginCombo("##fate-sfx",
                                     (Constants.PlayableSoundEffect.Find(cSound => cSound.EffectId ==
                                                                             Configuration.FateSfx) ??
                                      Constants.PlayableSoundEffect[0]).Name))
                {
                    foreach (var sound in Constants.PlayableSoundEffect)
                    {
                        var isSelected = sound.EffectId == Configuration.FateSfx;

                        if (ImGui.Selectable(sound.Name.ToString(), isSelected))
                        {
                            hasValueChange = true;
                            Configuration.FateSfx = sound.EffectId;
                        }
                    }

                    ImGui.EndCombo();
                }

                ImGui.SameLine();

                if (ImGuiComponents.IconButton(FontAwesomeIcon.Play))
                {
                    if (!Constants.PlayableSoundEffect.Exists(cSound => cSound.EffectId == Configuration.FateSfx))
                    {
                        OccultCrescentHelper.Log.Debug($"FateSFX: {Configuration.FateSfx} are not in Constants List");
                    }
                    else
                    {
                        UIGlobals.PlaySoundEffect(Configuration.FateSfx);
                    }
                }
            }

            ImGui.EndTabItem();
        }

        return hasValueChange;
    }

    private bool TabCE()
    {
        var hasValueChange = false;

        if (ImGui.BeginTabItem("Critical Encounter##ce"))
        {
            ImGui.EndTabItem();
        }

        return hasValueChange;
    }

    private bool TabForkTower()
    {
        var hasValueChange = false;

        if (ImGui.BeginTabItem("Forked Tower##ft"))
        {
            hasValueChange |= ImGui.Checkbox("Show Toast", ref Configuration.ShowFTToast);

            hasValueChange |= ImGui.Checkbox("Play Sound", ref Configuration.PlayFTSfx);

            ImGui.SameLine();

            if (Configuration.PlayFTSfx)
            {
                ImGui.SetNextItemWidth(100.0f * ImGuiHelpers.GlobalScale);
                if (ImGui.BeginCombo(
                        "##ft-sfx",
                        (Constants.PlayableSoundEffect.Find(cSound => cSound.EffectId == Configuration.FTSfx) ??
                         Constants.PlayableSoundEffect[0]).Name))
                {
                    foreach (var sound in Constants.PlayableSoundEffect)
                    {
                        var isSelected = sound.EffectId == Configuration.FTSfx;

                        if (ImGui.Selectable(sound.Name.ToString(), isSelected))
                        {
                            hasValueChange = true;
                            Configuration.FTSfx = sound.EffectId;
                        }

                        ;
                    }

                    ImGui.EndCombo();
                }

                ImGui.SameLine();

                if (ImGuiComponents.IconButton(FontAwesomeIcon.Play))
                {
                    if (!Constants.PlayableSoundEffect.Exists(cSound => cSound.EffectId == Configuration.FTSfx))
                    {
                        OccultCrescentHelper.Log.Debug($"FTSFX: {Configuration.FateSfx} are not in Constants List");
                    }
                    else
                    {
                        UIGlobals.PlaySoundEffect(Configuration.FTSfx);
                    }
                }
            }

            ImGui.EndTabItem();
        }

        return hasValueChange;
    }

    private unsafe void TabDebug()
    {
        if (ImGui.BeginTabItem("Debug##debug"))
        {
            if (ImGui.Button("Get CE Pointer##ce-button"))
            {
                try
                {
                    var activeCE =
                        *(ushort*)((nint)GameFunction.GetPublicContentOccultCrescentInstance() + 0x1400 + 0x1D7C);
                    OccultCrescentHelper.Log.Verbose(
                        $"InstancesAddr : {((nint)GameFunction.GetPublicContentOccultCrescentInstance()).ToString("X")}");
                    OccultCrescentHelper.Log.Verbose($"ActiveCE : {activeCE}");
                }
                catch (InvalidOperationException ex)
                {
                    OccultCrescentHelper.Log.Debug(ex.Message);
                }
            }
        }

        ImGui.EndTabItem();
    }
}
