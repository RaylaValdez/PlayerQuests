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
using Dalamud.Interface.Textures;
using System.IO;

namespace PlayerQuests.Windows
{
    public class MiniMap : Window
    {
        public static ISharedImmediateTexture elipseShadow;
        public static Vector2 elipseShadowSize = new Vector2(74, 7);
        public static nint elipseShadowHandle;

        public static readonly Vector2 QuestIconSize = new Vector2(28f, 28f) * 1.5f;

        public MiniMap() : base("PQMiniMapWindow")
        {
            Size = new System.Numerics.Vector2(200, 200);
            Position = new System.Numerics.Vector2(200, 200);

            Flags |= ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBackground
                     | ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoNavFocus;

            ForceMainWindow = true;
            IsOpen = true;
            var assemblyDirectory = Plugin.PluginInterface.AssemblyLocation.Directory?.FullName!;
            var iconPath = Path.Combine(assemblyDirectory, "Drawing", "Textures", "Journal", "QuestContent");
            elipseShadow = Services.TextureProvider.GetFromFile(Path.Combine(iconPath, "shadow.png"));

        }

        protected static void DrawQuestIcon(ImDrawListPtr drawList, Quest quest)
        {

            var questScreenPosition = MarkerUtils.CalculateMapPosition(quest);
            if (questScreenPosition == Vector2.Zero)
            {
                return;

            }
            var questIcon = PluginHelpers.GetQuestIconTexture(quest.QuestType);
            drawList.AddImage(questIcon.GetWrapOrEmpty().ImGuiHandle, questScreenPosition - (QuestIconSize / 2f), questScreenPosition - (QuestIconSize / 2f) + QuestIconSize);
            if (PluginHelpers.hoveringOverSelectableRegion(questScreenPosition, QuestIconSize))
            {
                // set cursor pos quest screenpos + like 10 above it
                ImGui.SetCursorPos(questScreenPosition - new Vector2(ImGui.CalcTextSize(quest.Name).X + 10, ImGui.CalcTextSize(quest.Name).Y + 2.5f) / 2 - ImGui.GetWindowPos() + new Vector2(0, - ImGui.CalcTextSize(quest.Name).Y - 10f));
                //elipses shadow at font height, with quest.name length
                ImGui.Image(elipseShadow.GetWrapOrEmpty().ImGuiHandle, new Vector2(ImGui.CalcTextSize(quest.Name.ToString()).X + 10, ImGui.CalcTextSize(quest.Name.ToString()).Y + 2.5f));
                ImGui.SetCursorPos(questScreenPosition - new Vector2(ImGui.CalcTextSize(quest.Name).X + 10, ImGui.CalcTextSize(quest.Name).Y + 2.5f) / 2 - ImGui.GetWindowPos() + new Vector2(0, -ImGui.CalcTextSize(quest.Name).Y - 10f));
                //elipses shadow at font height, with quest.name length
                ImGui.Image(elipseShadow.GetWrapOrEmpty().ImGuiHandle, new Vector2(ImGui.CalcTextSize(quest.Name.ToString()).X + 10, ImGui.CalcTextSize(quest.Name.ToString()).Y + 2.5f));

                //canvas font handle for drawing tooltip on top
                ImGui.SetCursorPos(questScreenPosition - ImGui.CalcTextSize(quest.Name) / 2 - ImGui.GetWindowPos() + new Vector2(0, -ImGui.CalcTextSize(quest.Name).Y - 10f));

                WindowHelpers.ImGuiTextWithDropShadow(quest.Name, 2f, 10);
            }
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
