using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using ImGuiNET;
using PlayerQuests;
using PlayerQuests.Drawing;
using PlayerQuests.Helpers;
using XivCommon;

using Services = PlayerQuests.Services;


namespace PlayerQuests.Windows;

public class MainWindow : Window, IDisposable
{
    private readonly Plugin plugin;


    private string questObjectiveTemp = string.Empty;

    // We give this window a hidden ID using ##
    // So that the user will see "My Amazing Window" as window title,
    // but for ImGui the ID is "My Amazing Window##With a hidden ID"
    public MainWindow(Plugin plugin)
        : base("My Amazing Window##With a hidden ID", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };


        this.plugin = plugin;
    }


    public void Dispose() { GC.SuppressFinalize(this); }

    public override void Draw()
    {
        if (!string.IsNullOrEmpty(Plugin.Configuration!.tempQuestType))
        {
            PluginHelpers.questType = Plugin.Configuration.tempQuestType;
            Plugin.DummyWindow.QuestIcon = PluginHelpers.QuestIcons[Plugin.Configuration.tempQuestType];
        }

        if (!string.IsNullOrEmpty(Plugin.Configuration.tempQuestName))
        {
            PluginHelpers.questName = Plugin.Configuration.tempQuestName;
            Plugin.DummyWindow.Title = Plugin.Configuration.tempQuestName;
        }

        if (!string.IsNullOrEmpty(Plugin.Configuration.tempQuestDescription))
        {
            PluginHelpers.questDescription = Plugin.Configuration.tempQuestDescription;
        }

        if (Plugin.Configuration.tempReward != 0)
        {
            PluginHelpers.questReward = Plugin.Configuration.tempReward;
        }

        if (ImGui.BeginCombo("Quest Type", PluginHelpers.questType))
        {
            foreach (var questType in PluginHelpers.QuestIcons)
            {
                var typeString = questType.Key;
                if (ImGui.Selectable(questType.Key.ToString(), (typeString == PluginHelpers.questType)))
                {
                    Plugin.Configuration!.tempQuestType = typeString;
                    Plugin.Configuration.Save();
                }
            }
            ImGui.EndCombo();
        }
        if (ImGui.InputText("Quest Name", ref PluginHelpers.questName, PluginHelpers.maxCharacters, ImGuiInputTextFlags.None))
        {
            Plugin.Configuration!.tempQuestName = PluginHelpers.questName;
            Plugin.DummyWindow.Title = Plugin.Configuration.tempQuestName;
            Plugin.Configuration.Save();
        }

        if (ImGui.InputTextMultiline("Quest Description", ref PluginHelpers.questDescription, PluginHelpers.maxDescriptionCharacters, new Vector2(300, 150)))
        {
            Plugin.Configuration!.tempQuestDescription = PluginHelpers.questDescription;
            Plugin.Configuration.Save();
        }

        if (ImGui.InputInt("Reward (Gil)", ref PluginHelpers.questReward))
        {
            if (PluginHelpers.questReward < 0)
            {
                PluginHelpers.questReward = 0;
            }
            if (PluginHelpers.questReward > 1000000)
            {
                PluginHelpers.questReward = 1000000;
            }

            Plugin.Configuration!.tempReward = PluginHelpers.questReward;
            Plugin.Configuration.Save();
        }

        if (ImGui.Button("Choose Position"))
        {
            Plugin.Configuration!.showPosPicker = true;
            PluginHelpers.dummyIconVisible = true;
        }

        ImGui.SameLine();

        if (Plugin.Configuration?.lastWorldPos != null)
        {
            ImGui.Text(Plugin.Configuration.lastWorldPos.ToString());
        }

        if (ImGui.BeginChild("Objectives", new Vector2(350, 700), true))
        {
            using var id = ImRaii.PushId("questObjectives");

            ImGui.Columns(3);
            ImGui.SetColumnWidth(0, 18 + (5 * ImGuiHelpers.GlobalScale));
            ImGui.SetColumnWidth(1, ImGui.GetWindowContentRegionMax().X - ImGui.GetWindowContentRegionMin().X - (18 + 16 + 14) - ((5 + 45 + 26) * ImGuiHelpers.GlobalScale));
            ImGui.SetColumnWidth(2, 16 + (45 * ImGuiHelpers.GlobalScale));

            ImGui.Separator();

            ImGui.TextUnformatted("#");
            ImGui.NextColumn();
            ImGui.TextUnformatted("Objectives");
            ImGui.NextColumn();
            ImGui.TextUnformatted("Delete?");
            ImGui.NextColumn();
            ImGui.TextUnformatted(string.Empty);

            ImGui.Separator();

            QuestObjectiveSettings objectiveToRemove = null;

            var locNumber = 1;
            foreach (var questObjectiveSetting in PluginHelpers.questObjectives)
            {
                id.Push(questObjectiveSetting.Objective.ToString());

                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + (ImGui.GetColumnWidth() / 2) - 8 - (ImGui.CalcTextSize(locNumber.ToString()).X / 2));
                ImGui.TextUnformatted(locNumber.ToString());
                ImGui.NextColumn();

                ImGui.SetNextItemWidth(-1);
                var obj = questObjectiveSetting.Objective.ToString();
                if (ImGui.InputText("##Objectives", ref obj, 65535, ImGuiInputTextFlags.EnterReturnsTrue))
                {

                }
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + (ImGui.GetColumnWidth() / 2) - 7 - (12 * ImGuiHelpers.GlobalScale));

                ImGui.NextColumn();


                if (ImGuiComponents.IconButton(FontAwesomeIcon.Trash))
                {
                    objectiveToRemove = questObjectiveSetting; 
                }
                id.Pop();
                ImGui.NextColumn();
                ImGui.Separator();
                locNumber++;
            }

            if (objectiveToRemove != null)
            {
                PluginHelpers.questObjectives.Remove(objectiveToRemove);
            }
            ImGui.SetCursorPosX(ImGui.GetCursorPosX() + (ImGui.GetColumnWidth() / 2) - 8 - (ImGui.CalcTextSize(locNumber.ToString()).X / 2));
            ImGui.TextUnformatted(locNumber.ToString());
            ImGui.NextColumn();
            ImGui.SetNextItemWidth(-1);
            ImGui.InputText("##objectiveInput", ref this.questObjectiveTemp, 300);
            //ImGui.SameLine();
            var curCursorPos = ImGui.GetCursorPos();
            var posAfterInput = new Vector2(ImGui.CalcItemWidth() + 5f, curCursorPos.Y);
            if (!string.IsNullOrEmpty(this.questObjectiveTemp))
            {
                ImGui.SetCursorPos(posAfterInput);
                //  Services.Log.Debug("Temp Objective is not null/empty :" + this.questObjectiveTemp);
                if (ImGuiComponents.IconButton(FontAwesomeIcon.Plus))
                {
                    PluginHelpers.questObjectives.Add(new QuestObjectiveSettings
                    {
                        Objective = this.questObjectiveTemp,
                    });
                    this.questObjectiveTemp = string.Empty;
                }

            }
            


            ImGui.EndChild();
        }

        if (ImGui.BeginCombo("Quest Lifetime", PluginHelpers.questDuration))
        {
            foreach (var duration in PluginHelpers.QuestDurations)
            {
                var durationString = duration.Key;
                if (ImGui.Selectable(duration.Key.ToString(), (durationString == PluginHelpers.questDuration)))
                {
                    PluginHelpers.questDuration = durationString;
                }
            }
            ImGui.EndCombo();
        }

        if (ImGui.Button("Kill Dummy"))
        {
            PluginHelpers.dummyIconVisible = false;

        }

    }


}
