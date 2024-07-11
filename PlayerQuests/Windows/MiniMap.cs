using Dalamud.Interface.Windowing;
using ImGuiNET;
using PlayerQuests.Helpers;
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
        public MiniMap() : base("PQMiniMapWindow")
        {
            Size = new System.Numerics.Vector2(200, 200);
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

                foreach (var quest in MarkerUtils.questLocations)
                {
                    drawList.AddCircle(MarkerUtils.CalculateMapPosition(quest), 5f, dotColor, 0, 1f);

                }
            }
        }

        public override bool DrawConditions()
        {
            if (!MarkerUtils.RunChecks())
            {
                return false;
            }
            if (Services.NaviMapManager.InCombat)
            {
                return false;
            }

            if (Services.ClientState.IsPvPExcludingDen)
            {
                return false;
            }

            return true;
        }

        public override void PreDraw()
        {
            MarkerUtils.PrepareDrawOnMinimap();

        }
    }
}
