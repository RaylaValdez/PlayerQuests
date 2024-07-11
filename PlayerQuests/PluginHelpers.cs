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
using Dalamud.Game.Addon.Events;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using Dalamud.Interface.Textures;
using PlayerQuests.Helpers;

internal static class PluginHelpers
{
    private static float PreviousSmoothedSigmoidValue = 0;

    public static Dictionary<string, uint> QuestIcons = new()
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

    public static ISharedImmediateTexture? QuestIcon = null;

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



    public static bool hovering = false;


    public static List<QuestObjectiveSettings> questObjectives = new();












    public static void DrawDummy(string questName, Vector3 lastWorldPos)
    {
        QuestIcon = Services.TextureProvider.GetFromGameIcon(QuestIcons[questType]);
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
            var textWidth = ImGui.CalcTextSize(questName);


            var IDrawList = ImGui.GetWindowDrawList();

            IDrawList.AddImage(QuestIcon.GetWrapOrEmpty().ImGuiHandle, screenPosForIcon, screenPosForIcon + iconSize);
            
            IDrawList.AddText(ImGui.GetFont(), ImGui.GetFontSize(), screenPosForText, ImGui.ColorConvertFloat4ToU32(new Vector4(233, 255, 226, 256) / 255), questName);

            // pop
            unsafe
            {
                var prevCursorType = Framework.Instance()->Cursor->ActiveCursorType;
                if (hoveringOverSelectableRegion(screenPosForText + textWidth / 2, textWidth * 1.5f) || hoveringOverSelectableRegion(screenPosForIcon + iconSize / 2, iconSize * 1.5f))
                {
                    hovering = true;
                    Framework.Instance()->Cursor->ActiveCursorType = (int)AddonCursorType.Clickable;
                    if (MouseButtonState.RightReleased)
                    {
                        Plugin.Instance.ToggleDummyWindow();
                    }
                }
            }

        }



    }


    public static bool hoveringOverSelectableRegion(Vector2 centerPosition, Vector2 size)
    {
        var mouseCursorLocation = ImGui.GetMousePos();
        var hoveringOver = false;

        var startXY = centerPosition - (size / 2f);
        var endXY = startXY + size;

        if (mouseCursorLocation.X >= startXY.X && mouseCursorLocation.X <= endXY.X && mouseCursorLocation.Y >= startXY.Y && mouseCursorLocation.Y <= endXY.Y)
        {
            // mouse is hovered here
            hoveringOver = true;
        }

        return hoveringOver;
    }

    public static void DrawIcon(string questIcon)
    {
        QuestIcon = Services.TextureProvider.GetFromGameIcon(QuestIcons[questIcon]);

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

                IDrawList.AddImage(QuestIcon.GetWrapOrEmpty().ImGuiHandle, screenPos, screenPos + iconSize);
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
