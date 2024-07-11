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
using PlayerQuests.Drawing;

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
        { "8 Hours", 123456 },
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

    public static bool dummyIconVisible = false;

    public static string selectedQuestType = string.Empty;

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
    public static bool startedHoveringOverQuestIcon = false;


    public static List<QuestObjectiveSettings> questObjectives = new();

    public static List<Quest> Quests = [];

    public static Quest TempQuest => new Quest()
    {
        Name = questName,
        Description = questDescription,
        QuestType = questType,
        QuestPositionX = Plugin.Configuration.lastWorldPos.X,
        QuestPositionY = Plugin.Configuration.lastWorldPos.Y,
        QuestPositionZ = Plugin.Configuration.lastWorldPos.Z,
        Reward = questReward,
        QuestAuthor = questAuthor,
        QuestZone = "TODO",
        DatacenterName = questDatacenter,
        WorldName = questWorld,
        Id = -1, // TODO
        Accepted = questAccepted,
        Completed = questCompleted,
        ExpireTime = questExpireTime, // TODO
        TimePosted = questTimePosted, // TODO
    };

    private static Dictionary<uint, ISharedImmediateTexture> QuestIconTextures = new();

    public static ISharedImmediateTexture GetQuestIconTexture(uint icon)
    {
        if (QuestIconTextures.TryGetValue(icon, out var texture))
        {
            return texture;
        }

        // Load the quest icon from game icon
        var gameIcon = Services.TextureProvider.GetFromGameIcon(icon);
        QuestIconTextures[icon] = gameIcon;
        return gameIcon;
    }

    public static ISharedImmediateTexture GetQuestIconTexture(string questType) => GetQuestIconTexture(QuestIcons[questType]);
    
    public static void DrawDummy(Quest quest)
    {
        var questIconTexture = GetQuestIconTexture(quest.QuestType);
        var questPosition = quest.QuestPosition;

        Services.GameGui.WorldToScreen(questPosition + iconMaxOffset, out var screenPosForIcon, out var inView);
        var screenPosForText = screenPosForIcon;

        screenPosForIcon -= iconSize / 2;

        screenPosForText -= new Vector2(ImGui.CalcTextSize(quest.Name).X / 2, -50f);
        var textWidth = ImGui.CalcTextSize(quest.Name);
        var drawList = ImGui.GetWindowDrawList();

        var questIconHandle = questIconTexture.GetWrapOrEmpty().ImGuiHandle;

        var distanceToPlayer = Services.ClientState.LocalPlayer!.Position - questPosition;
        const float maxDistance = 25f;
        const float startFadeDistance = 20f;

        var opaqueColor = new Vector4(1, 1, 1, 1);
        var transparentColor = new Vector4(1, 1, 1, 0);

        if (dummyIconVisible)
        {
            var distance = distanceToPlayer.Length();
            if (distance < maxDistance)
            {
                float alpha;
                if (distance <= startFadeDistance)
                {
                    alpha = 1.0f;
                }
                else
                {
                    alpha = 1.0f - (distance - startFadeDistance) / (maxDistance - startFadeDistance);
                }

                var curColor = new Vector4(1, 1, 1, alpha);
                if (Raycaster.PointVisible(questPosition))
                {
                    IDisposable? fontDisposer = null;
                    if (Canvas.FontHandle?.Available ?? false)
                    {
                        fontDisposer = Canvas.FontHandle.Push();
                    }

                    drawList.AddImage(questIconHandle, screenPosForIcon, screenPosForIcon + iconSize, new Vector2(0, 0), new Vector2(1, 1), ImGui.ColorConvertFloat4ToU32(curColor));

                    drawList.AddText(ImGui.GetFont(), ImGui.GetFontSize(), screenPosForText, ImGui.ColorConvertFloat4ToU32(new Vector4(233, 255, 226, curColor.W * 255) / 255), quest.Name);

                    unsafe
                    {
                        var prevCursorType = Framework.Instance()->Cursor->ActiveCursorType;
                        if (hoveringOverSelectableRegion(screenPosForText + textWidth / 2, textWidth * 1.5f) || hoveringOverSelectableRegion(screenPosForIcon + iconSize / 2, iconSize * 1.5f))
                        {
                            hovering = true;
                            Framework.Instance()->Cursor->ActiveCursorType = (int)AddonCursorType.Clickable;
                            // Only if hovered at the start and end of a right click (and mouse not being captured by ImGui)
                            if (MouseButtonState.RightReleased && startedHoveringOverQuestIcon && !ImGui.GetIO().WantCaptureMouse)
                            {
                                Plugin.Instance.ToggleDummyWindow(quest);
                            }
                            else if (MouseButtonState.RightPressed)
                            {
                                startedHoveringOverQuestIcon = true;
                            }
                        }
                    }

                    fontDisposer?.Dispose();
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
