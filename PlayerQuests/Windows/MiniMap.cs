using Dalamud.Interface.Windowing;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PlayerQuests.Windows
{
    public class MiniMap : Window
    {
        public MiniMap() : base("MiniMapWindow")
        {
            Size = new System.Numerics.Vector2(200,200);
            Position = new System.Numerics.Vector2(200, 200);

            Flags |= ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBackground
                | ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoNavFocus;

            ForceMainWindow = true;
            IsOpen = true;
        }

        public override void Draw()
        {
            var drawList = ImGui.GetWindowDrawList();

            if (PluginHelpers.questLocation != Vector3.Zero)
            {
                var dotColor = ImGui.ColorConvertFloat4ToU32(new Vector4(1, 0, 0, 1));
                var isVisible = Services.GameGui.WorldToScreen(PluginHelpers.questLocation, out var screenPos);
                if (isVisible)
                {
                    drawList.AddCircle(screenPos, 5f, dotColor, 0, 1f);
                }
            }
        }
    }
}
