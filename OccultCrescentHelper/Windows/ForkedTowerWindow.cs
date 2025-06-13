using System;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using ImGuiNET;

namespace OccultCrescentHelper.Windows
{
    internal class ForkedTowerWindow : Window, IDisposable
    {
        public ForkedTowerWindow(OccultCrescentHelper plugin) : base("Forked Tower Entry Count##ft-entry-count")
        {
            Flags = ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoResize;

            SizeConstraints = new WindowSizeConstraints()
            {
                MinimumSize = new Vector2(300, 70),
                MaximumSize = new Vector2(300, 70)
            };
        }

        public void Dispose() { }

        public unsafe override void Draw()
        {
            if(Common.IsPlayerInSouthHorn(OccultCrescentHelper.ClientState.TerritoryType, AgentMap.Instance()->CurrentMapId))
            {
                var playerInsideCircle = OccultCrescentHelper.ObjectTable.OfType<IPlayerCharacter>().Where<IPlayerCharacter>(cPlayer => Vector2.Distance(Constants.OccultCrescentSouthHornForkedTowerEntryPosition, new Vector2(cPlayer.Position.X, cPlayer.Position.Z)) < 20);

                ImGui.Text($"Player Inside Entry:");
                ImGui.SameLine();
                ImGui.Text($"{playerInsideCircle.Count()}");
                var localPlayer = OccultCrescentHelper.ClientState.LocalPlayer;
                if (localPlayer != null)
                {
                    var distantBetweenEntryAndplayer = Vector2.Distance(Constants.OccultCrescentSouthHornForkedTowerEntryPosition, new Vector2(localPlayer.Position.X, localPlayer.Position.Z));
                    // Max range for Objectable is 100, 20 is distant from inside entry circle
                    if(distantBetweenEntryAndplayer > 120)
                    {
                        ImGui.TextColored(ImGuiColors.DPSRed, "You are too far, counting might not correct");
                    }
                }
            }
        }

        public override void PreDraw() { }
    }
}
