using System;
using System.Numerics;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using ImGuiNET;
using PlayerQuests;
using XivCommon;

using Services = PlayerQuests.Services;


namespace PlayerQuests.Windows;

public class MainWindow : Window, IDisposable
{
    private readonly Plugin plugin;


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

    //public MainWindow(Plugin plugin, IDalamudTextureWrap? dalamudTextureWrap)
    //{
    //    Plugin = plugin;
    //    this.dalamudTextureWrap = dalamudTextureWrap;
    //}

    public void Dispose() { GC.SuppressFinalize(this); }

    public override void Draw()
    {
        if (ImGui.BeginCombo("Quest Type", PluginHelpers.questType))
        {
            foreach (var questType in PluginHelpers.QuestIcons)
            {
                var typeString = questType.Key;
                if (ImGui.Selectable(questType.Key.ToString(), (typeString == PluginHelpers.questType)))
                {
                    PluginHelpers.questType = typeString;
                }
            }
            ImGui.EndCombo();
        }
        if (ImGui.InputText("Quest Name", ref PluginHelpers.questName, PluginHelpers.maxCharacters, ImGuiInputTextFlags.None))
        {

        }

        if (ImGui.InputTextMultiline("Quest Description", ref PluginHelpers.questDescription, PluginHelpers.maxCharacters, new Vector2(300, 150)))
        {

        }

        if (ImGui.InputInt("Reward (Gil)", ref PluginHelpers.questReward))
        {

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

    }


}
