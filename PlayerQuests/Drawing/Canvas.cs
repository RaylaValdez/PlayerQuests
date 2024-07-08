using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface.GameFonts;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using PlayerQuests.Drawing;

namespace PlayerQuests.Drawing
{
    internal unsafe class Canvas : Window
    {
        public Canvas() : base("PlayerQuestGroundTargettingOverlay",
            ImGuiWindowFlags.NoInputs
            | ImGuiWindowFlags.NoTitleBar
            | ImGuiWindowFlags.NoScrollbar
            | ImGuiWindowFlags.NoBackground
            | ImGuiWindowFlags.AlwaysUseWindowPadding
            | ImGuiWindowFlags.NoSavedSettings
            | ImGuiWindowFlags.NoFocusOnAppearing
        , true)
        {
            this.IsOpen = true;
            this.RespectCloseHotkey = false;
        }

        public override void PreDraw()
        {
            base.PreDraw();
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));
            ImGuiHelpers.SetNextWindowPosRelativeMainViewport(Vector2.Zero);
            ImGui.SetNextWindowSize(ImGuiHelpers.MainViewport.Size);
        }

        public override void Draw()
        {
            MouseButtonState.UpdateState();
            var cursorPos = ImGui.GetMousePos();
            Services.GameGui.ScreenToWorld(cursorPos, out var worldPos);
            if (!PluginHelpers.questType.IsNullOrEmpty())
            {
                PluginHelpers.DrawDummy(PluginHelpers.questName, Plugin.Configuration!.lastWorldPos);
            }
            

            if (Plugin.Configuration!.showPosPicker)
            {
                DrawFunctions.CircleXZ(worldPos, 0.5f, new Brush { Color = new Vector4(81, 54, 148, 180) / 255f, Fill = new Vector4(184, 83, 159, 123) / 255, Thickness = 3f });
                DrawFunctions.CircleXZ(worldPos, 0.5f, new Brush { Color = new Vector4(81, 54, 148, 180) / 255f, Fill = new Vector4(184, 83, 159, 0) / 255, Thickness = 3f });
                DrawFunctions.CircleXZ(worldPos, 0.3f, new Brush { Color = new Vector4(81, 54, 148, 180) / 255f, Fill = new Vector4(184, 83, 159, 0) / 255, Thickness = 3f });
                DrawFunctions.CircleXZ(worldPos, 0.01f, new Brush { Color = new Vector4(81, 54, 148, 180) / 255f, Fill = new Vector4(184, 83, 159, 0) / 255, Thickness = 2.5f });

                if (MouseButtonState.LeftPressed)
                {
                    Plugin.Configuration.showPosPicker = false;
                    Plugin.Configuration.lastWorldPos = new Vector3(MathF.Round(worldPos.X,2), MathF.Round(worldPos.Y, 2), MathF.Round(worldPos.Z, 2));
                }

            }
        }

        public override void PostDraw()
        {
            base.PostDraw();
            ImGui.PopStyleVar();
        }

    }
}
