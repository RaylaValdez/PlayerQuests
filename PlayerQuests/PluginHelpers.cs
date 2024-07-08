using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface.Textures.TextureWraps;
using ImGuiNET;
using PlayerQuests;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using CameraManager = FFXIVClientStructs.FFXIV.Client.Game.Control.CameraManager;
using Dalamud.Plugin;
using Dalamud.Interface;
using System.Drawing;
using FFXIVClientStructs;
using FFXIVClientStructs.FFXIV.Client.UI;
using Dalamud.Interface.GameFonts;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Dalamud.Interface.ManagedFontAtlas;

internal static class PluginHelpers
{
    private static float PreviousSmoothedSigmoidValue = 0;

    public static Dictionary<string, int> QuestIcons = new()
    {
        { "NormalQuest", 61431 },
        { "RepeatableQuest", 61433 },
        { "SpecialQuest", 61439 },
        { "FuckedOff", 64121 }
    };

    public static Dictionary<string, int> QuestDurations = new()
    {
        { "8 Hours", 123456},
        { "16 Hours", 123456 },
        { "24 Hours", 123456 },
        { "48 Hours", 123456 },
        { "1 Week", 123456 }
    };

    public static Vector3 playerPosition => Services.ClientState.LocalPlayer!.Position;
    public static Vector3 iconMaxOffset = new(0, 2.5f, 0);
    public static Vector3 iconMinOffset = new(0, 1f, 0);
    public static Vector3 textOffset = new(0, 2.25f, 0);

    public static Vector2 iconSize = new(96, 96);

    //public static float playerZoom => CameraManager.

    public static IDalamudTextureWrap? QuestIcon = null;

    public static string selectedQuestType = string.Empty;

    public static bool iconVisible = false;

    public static bool dummyIconVisible = false;

    public const uint maxCharacters = 50;
    
    public const uint maxDescriptionCharacters = 560;

    public static string questType = string.Empty;

    public static string questName = string.Empty;

    public static string questDescription = string.Empty;

    public static int questReward = 0;

    public static Vector3 questLocation = new(0, 0, 0);

    public static string questDatacenter = string.Empty;

    public static string questWorld = string.Empty;

    public static Int64 questTimePosted = 0;

    public static Int64 questExpireTime = 0;

    public static string questDuration = string.Empty;

    public static bool questAccepted = false;

    public static bool questCompleted = false;

    public static string questAuthor = string.Empty;

















    public static void DrawDummy(string questName, Vector3 lastWorldPos)
    {
        QuestIcon = Services.TextureProvider.GetFromGameIcon(QuestIcons[questType]).GetWrapOrEmpty();
        var screenPosForIcon = new Vector2(0, 0);
        var screenPosForText = new Vector2(0, 0);
        var inView = false;

        questLocation = lastWorldPos;

        Services.GameGui.WorldToScreen(questLocation + iconMaxOffset, out screenPosForIcon, out inView);
        Services.GameGui.WorldToScreen(questLocation + iconMaxOffset, out screenPosForText, out inView);

        screenPosForIcon -= iconSize / 2;

        // Calculate the size of the questName text
        //var textSize = ImGui.CalcTextSize(questName);
        // Adjust screenPosForText to center the text


        if (dummyIconVisible)
        {
            //push
            screenPosForText -= new Vector2(ImGui.CalcTextSize(questName).X / 2, -50f);

            var IDrawList = ImGui.GetWindowDrawList();

            IDrawList.AddImage(QuestIcon.ImGuiHandle, screenPosForIcon, screenPosForIcon + iconSize);
            
            IDrawList.AddText(ImGui.GetFont(), ImGui.GetFontSize(), screenPosForText, ImGui.ColorConvertFloat4ToU32(new Vector4(233, 255, 226, 256) / 255), questName);
            // pop
        }



    }



    public static void DrawIcon(string questIcon)
    {
        QuestIcon = Services.TextureProvider.GetFromGameIcon(QuestIcons[questIcon]).GetWrapOrEmpty();

        var screenPos = new Vector2(0, 0);
        var inView = false;
        unsafe
        {
            var cameraInstance = CameraManager.Instance();
            var activeCamera = cameraInstance->GetActiveCamera();
            var cameraDistance = CameraManager.Instance()->GetActiveCamera()->Distance;

            Services.GameGui.WorldToScreen(playerPosition + Vector3.Lerp(iconMinOffset, iconMaxOffset, SmoothedSigmoid(cameraDistance / 15f, 0.1f)), out screenPos, out inView);

            screenPos -= iconSize / 2;
            if (iconVisible)
            {
                var IDrawList = ImGui.GetBackgroundDrawList();

                IDrawList.AddImage(QuestIcon.ImGuiHandle, screenPos, screenPos + iconSize);
            }
        }
    }


    public static float Sigmoid(float value)
    {
        return 1 / (1 + (float)Math.Exp(-value));
    }

    public static float SmoothedSigmoid(float value, float smoothingFactor)
    {
        if (smoothingFactor <= 0 || smoothingFactor > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(smoothingFactor), "Smoothing factor must be between 0 and 1.");
        }

        var sigmoidValue = Sigmoid(value);
        var smoothedValue = (smoothingFactor * sigmoidValue) + ((1 - smoothingFactor) * PreviousSmoothedSigmoidValue);
        PreviousSmoothedSigmoidValue = smoothedValue;
        return smoothedValue;
    }
}
