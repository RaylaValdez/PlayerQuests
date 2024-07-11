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
using Dalamud.Interface.ManagedFontAtlas;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using PlayerQuests.Drawing;
using PlayerQuests.Helpers;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using Dalamud.Game.Addon.Events;


namespace PlayerQuests.Drawing
{
    public unsafe class Canvas : Window
    {
        public static IFontHandle? FontHandle = null;
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

            FontHandle = GameFontBuilder.GetFont(GameFontFamilyAndSize.Axis18);
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
            IDisposable? fontDisposer = null;
            if (FontHandle?.Available ?? false)
            {
                fontDisposer = FontHandle.Push();
            }
            var previousHovering = PluginHelpers.hovering;
            PluginHelpers.hovering = false;
            var cursorPos = ImGui.GetMousePos();
            Services.GameGui.ScreenToWorld(cursorPos, out var worldPos);

            foreach (var quest in PluginHelpers.Quests)
            {
                PluginHelpers.DrawDummy(quest);
            }
            
            if (!PluginHelpers.TempQuest.QuestType.IsNullOrEmpty())
            {
                PluginHelpers.DrawDummy(PluginHelpers.TempQuest);
            }
            
            if (previousHovering && !PluginHelpers.hovering)
            {
                Framework.Instance()->Cursor->ActiveCursorType = (int)AddonCursorType.Arrow;
            }
            
            if (MouseButtonState.RightReleased)
            {
                PluginHelpers.startedHoveringOverQuestIcon = false;
            }
            
            fontDisposer?.Dispose();

            if (Plugin.Configuration!.showPosPicker)
            {
                //DrawFunctions.CircleXZ(worldPos, 0.5f, new Brush { Color = new Vector4(81, 54, 148, 180) / 255f, Fill = new Vector4(184, 83, 159, 123) / 255, Thickness = 3f });
                //DrawFunctions.CircleXZ(worldPos, 0.5f, new Brush { Color = new Vector4(81, 54, 148, 180) / 255f, Fill = new Vector4(184, 83, 159, 0) / 255, Thickness = 3f });
                //DrawFunctions.CircleXZ(worldPos, 0.3f, new Brush { Color = new Vector4(81, 54, 148, 180) / 255f, Fill = new Vector4(184, 83, 159, 0) / 255, Thickness = 3f });
                //DrawFunctions.CircleXZ(worldPos, 0.01f, new Brush { Color = new Vector4(81, 54, 148, 180) / 255f, Fill = new Vector4(184, 83, 159, 0) / 255, Thickness = 2.5f });
                DrawFunctions.CircleXZ(worldPos, 0.76f, new Brush { Color = new Vector4(171, 133, 130, 100) / 255f, Fill = new Vector4(171, 133, 130, 100) / 255f, Thickness = 2.5f }); // Transparent Tinge
                DrawFunctions.CircleXZ(worldPos, 0.7f, new Brush { Color = new Vector4(255, 255, 180, 200) / 255f, Fill = new Vector4(255, 255, 180, 0) / 255f, Thickness = 2.5f }); // Bright Outter Line
                DrawFunctions.CircleXZ(worldPos, 0.74f, new Brush { Color = new Vector4(236, 170, 108, 200) / 255f, Fill = new Vector4(236, 170, 108, 0) / 255f, Thickness = 2.5f }); // Orange Outter Tinge
                DrawFunctions.CircleXZ(worldPos, 0.72f, new Brush { Color = new Vector4(255, 204, 113, 200) / 255f, Fill = new Vector4(255, 204, 113, 0) / 255f, Thickness = 2.5f }); // Slightly Less Orange Outter Tinge
                DrawFunctions.CircleXZ(worldPos, 0.35f, new Brush { Color = new Vector4(255, 174, 78, 200) / 255f, Fill = new Vector4(255, 174, 78, 0) / 255f, Thickness = 2.5f }); // Bright Orange Inner Tinge
                DrawFunctions.CircleXZ(worldPos, 0.349f, new Brush { Color = new Vector4(255, 255, 166, 50) / 255f, Fill = new Vector4(255, 255, 166, 50) / 255f, Thickness = 2.5f }); // Bright Orange Inner Tinge
                DrawFunctions.RotatingCircle4SegmentsXZ(worldPos, 0.7f, new Brush { Color = new Vector4(236, 170, 108, 200) / 255f, Fill = new Vector4(236, 170, 108, 0) / 255f, Thickness = 25f });
                DrawFunctions.RotatingCircle4SegmentsXZ(worldPos, 0.15f, new Brush { Color = new Vector4(236, 170, 108, 200) / 255f, Fill = new Vector4(236, 170, 108, 0) / 255f, Thickness = 25f }, 45f, MathF.PI / 180f * 33);


                if (MouseButtonState.LeftPressed)
                {
                    Plugin.Configuration.showPosPicker = false;
                    Plugin.Configuration.lastWorldPos = new Vector3(MathF.Round(worldPos.X,2), MathF.Round(worldPos.Y, 2), MathF.Round(worldPos.Z, 2));
                }

            }
        }

        private bool hoveringOverSelectableRegion()
        {
            throw new NotImplementedException();
        }

        public override void PostDraw()
        {
            base.PostDraw();
            ImGui.PopStyleVar();
        }

    }
}
