using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using Dalamud.Interface.Textures;
using System.IO;
using System.Numerics;
using PlayerQuests.Helpers;

namespace PlayerQuests.Windows
{
    public class DummyQuestInteractWindow : JournalWindow
    {
        public ISharedImmediateTexture rewardIcon;
        public Vector2 rewardIconSize = new Vector2(36, 37) / 2.2f;
        public nint rewardIconHandle;

        public ISharedImmediateTexture rewardsBanner;
        public Vector2 rewardsBannerSize = new Vector2(746, 99) / 2.2f;
        public nint rewardsBannerHandle;

        public ISharedImmediateTexture descriptionIcon;
        public Vector2 descriptionIconSize = new Vector2(36, 37) / 2.2f;
        public nint descriptionIconHandle;

        public ISharedImmediateTexture gilIcon;
        public Vector2 gilIconSize = new Vector2(32, 32);
        public nint gilIconHandle;

        public ISharedImmediateTexture elipseShadow;
        public Vector2 elipseShadowSize = new Vector2(74, 7);
        public nint elipseShadowHandle;

        public ISharedImmediateTexture objectivesIcon;
        public Vector2 objectivesIconSize = new Vector2(36, 37) / 2.2f;
        public nint objectivesIconHandle;

        public DummyQuestInteractWindow() : base("Dummy Quest Window")
        {
            var assemblyDirectory = Plugin.PluginInterface.AssemblyLocation.Directory?.FullName!;
            var iconPath = Path.Combine(assemblyDirectory, "Drawing", "Textures", "Journal", "QuestContent");
            rewardIcon = Services.TextureProvider.GetFromFile(Path.Combine(iconPath, "RewardsIcon.png"));

            rewardsBanner = Services.TextureProvider.GetFromFile(Path.Combine(iconPath, "RewardsBanner.png"));

            descriptionIcon = Services.TextureProvider.GetFromFile(Path.Combine(iconPath, "DescriptionIcon.png"));

            gilIcon = Services.TextureProvider.GetFromGameIcon(65002);

            elipseShadow = Services.TextureProvider.GetFromFile(Path.Combine(iconPath, "shadow.png"));

            objectivesIcon = Services.TextureProvider.GetFromFile(Path.Combine(iconPath, "ObjectiveIcon.png"));

            this.SizeConstraints = new WindowSizeConstraints()
            {
                MinimumSize = MinSize,
                MaximumSize = new Vector2(432, 581),
            };


        }

        public override void DrawJournal()
        {
            
            var windowWidth = ImGui.GetWindowWidth();
            var windowsize = ImGui.GetWindowSize();
            var rewardIconWrap = rewardIcon.GetWrapOrEmpty();
            rewardIconHandle = rewardIconWrap.ImGuiHandle;
            var rewardsBannerWrap = rewardsBanner.GetWrapOrEmpty();
            rewardsBannerHandle = rewardsBannerWrap.ImGuiHandle;
            var descriptionIconWrap = descriptionIcon.GetWrapOrEmpty();
            descriptionIconHandle = descriptionIconWrap.ImGuiHandle;
            var elipseShadowWrap = elipseShadow.GetWrapOrEmpty();
            elipseShadowHandle = elipseShadowWrap.ImGuiHandle;
            var objectivesIconWrap = objectivesIcon.GetWrapOrEmpty();
            objectivesIconHandle = objectivesIconWrap.ImGuiHandle;


            var rewardsBannerWidth = rewardsBannerSize.X;

            if (ImGui.BeginChild("QuestContent", new Vector2(-1, -1), false, ImGuiWindowFlags.NoCollapse))
            {
                ImGui.Image(rewardIconHandle, rewardIconSize);
                ImGui.SameLine(rewardIconSize.X + 2f);
                WindowHelpers.ImGuiTextWithDropShadow("Reward", 2f);

                ImGui.SetCursorPosX((windowWidth / 2f) - (rewardsBannerWidth / 2f));
                ImGui.Image(rewardsBannerHandle, rewardsBannerSize);
                ImGui.SameLine();
                if (PluginHelpers.questReward > 0)
                {
                    var curCursorPos = ImGui.GetCursorPos();
                    ImGui.SetCursorPos(new Vector2(27f, curCursorPos.Y + 32f));
                    ImGui.Image(elipseShadowHandle, elipseShadowSize);
                    ImGui.SameLine();
                    ImGui.SetCursorPos(new Vector2(27f, curCursorPos.Y + 9f));
                    var gilIconWrap = gilIcon.GetWrapOrEmpty();
                    var gilIconHandle = gilIconWrap.ImGuiHandle;
                    ImGui.Image(gilIconHandle, gilIconSize);
                    ImGui.SameLine();
                    ImGui.SetCursorPos(new Vector2(68f, curCursorPos.Y + 20));
                    ImGui.Image(elipseShadowHandle, new Vector2(ImGui.CalcTextSize(PluginHelpers.questReward.ToString()).X, 14f));
                    ImGui.SameLine();
                    ImGui.SetCursorPos(new Vector2(68f, curCursorPos.Y + 13));
                    var seperatedRewardString = PluginHelpers.questReward.ToString("N0");
                    WindowHelpers.ImGuiTextWithDropShadow(seperatedRewardString, 2f, 10, true);
                }
                ImGui.Dummy(new Vector2(0, 2));


                ImGui.Image(descriptionIconHandle, descriptionIconSize);
                ImGui.SameLine(descriptionIconSize.X + 2f);
                WindowHelpers.ImGuiTextWithDropShadow("Description", 2f);
                ImGui.Indent(14);
                ImGui.PushTextWrapPos(windowWidth - 14);
                ImGui.TextWrapped(PluginHelpers.questDescription);
                ImGui.PopTextWrapPos();
                ImGui.Unindent(14);

                if (PluginHelpers.questObjectives.Count > 0)
                {
                    ImGui.Image(objectivesIconHandle, objectivesIconSize);
                    ImGui.SameLine(objectivesIconSize.X + 2f);
                    WindowHelpers.ImGuiTextWithDropShadow("Objectives", 2f);
                    foreach (QuestObjectiveSettings questObjective in PluginHelpers.questObjectives)
                    {
                        ImGui.Indent(14);
                        ImGui.PushTextWrapPos(windowWidth - 14);
                        ImGui.TextWrapped(questObjective.Objective.ToString());
                        ImGui.PopTextWrapPos();
                        ImGui.Unindent(14);
                    }

                }



                ImGui.EndChild();
            }
            
        }
    }
}
