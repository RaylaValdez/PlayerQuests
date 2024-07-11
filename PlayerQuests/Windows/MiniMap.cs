using Dalamud.Interface.Windowing;
using ImGuiNET;
using PlayerQuests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Utility;

namespace PlayerQuests.Windows
{
    public class MiniMap : Window
    {
        public static readonly Vector2 QuestIconSize = new Vector2(28f, 28f);

        public MiniMap() : base("PQMiniMapWindow")
        {
            Size = new System.Numerics.Vector2(200, 200);
            Position = new System.Numerics.Vector2(200, 200);

            Flags |= ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBackground
                     | ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoNavFocus;

            ForceMainWindow = true;
            IsOpen = true;
        }

        protected static void DrawQuestIcon(ImDrawListPtr drawList, Quest quest)
        {
            var questScreenPosition = MarkerUtils.CalculateMapPosition(quest);
            var questIcon = PluginHelpers.GetQuestIconTexture(quest.QuestType);
            drawList.AddImage(questIcon.GetWrapOrEmpty().ImGuiHandle, questScreenPosition - (QuestIconSize / 2f), questScreenPosition + QuestIconSize);
        }
        
        public override void Draw()
        {
            var drawList = ImGui.GetWindowDrawList();

            var dotColor = ImGui.ColorConvertFloat4ToU32(new Vector4(1, 0, 0, 1));

            if (!PluginHelpers.TempQuest.QuestType.IsNullOrEmpty() && PluginHelpers.dummyIconVisible)
            {
                DrawQuestIcon(drawList, PluginHelpers.TempQuest);
            }

            foreach (var quest in PluginHelpers.Quests)
            {
                DrawQuestIcon(drawList, quest);
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
