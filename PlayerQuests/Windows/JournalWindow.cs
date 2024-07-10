using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Dalamud.Interface.GameFonts;
using Dalamud.Interface.ManagedFontAtlas;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using PlayerQuests.Helpers;

namespace PlayerQuests.Windows;

public class JournalWindow : Window
{
    protected uint QuestIconInternal = 71141;
    protected ISharedImmediateTexture? QuestIconTexture;

    public Vector4 BackgroundColor { get; set; } = new Vector4(229, 224, 214, 255) / 255f;
    public string Title { get; set; } = "Journal Title";

    public uint QuestIcon
    {
        get => this.QuestIconInternal;
        set
        {
            QuestIconInternal = value;
            UpdateQuestIcon();
        }
    }

    protected enum JournalTexture : int
    {
        TopCorner,
        BottomCorner,
        TopBorder,
        TopMiddleBorder,
        BottomBorder,
        SideBorder,
        CloseButton
    }

    protected readonly ISharedImmediateTexture[] JournalTextures;

    protected IDalamudTextureWrap JournalTextureWrap(JournalTexture image)
    {
        return JournalTextures[(int)image].GetWrapOrEmpty();
    }

    protected static readonly Vector2 MinSize = new Vector2(432f, 320f);
    protected static readonly Vector2 QuestIconOffset = new Vector2(0f, 0f);
    protected static readonly Vector2 QuestIconSize = new Vector2(72f, 72f);
    protected static readonly Vector2 Margin = new(32, 30);
    protected const float CrossLineHeight = 44f;
    protected const float ExtraMarginTop = 50f;

    protected readonly IFontHandle Axis14FontHandle;
    protected readonly IFontHandle Axis18FontHandle;

    public JournalWindow(string name = "Journal Window") : base(name, ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoScrollWithMouse, false)
    {
        SizeConstraints = new WindowSizeConstraints()
        {
            MinimumSize = MinSize,
            MaximumSize = new Vector2(8000f, 8000f),
        };

        // Load journal textures
        var assemblyDirectory = Plugin.PluginInterface.AssemblyLocation.Directory?.FullName!;
        var texturePath = Path.Combine(assemblyDirectory, "Drawing", "Textures", "Journal");
        Services.Log.Debug($"Loading journal textures from {texturePath}");
        JournalTextures = new ISharedImmediateTexture[((int)JournalTexture.CloseButton) + 1];
        JournalTextures[(int)JournalTexture.TopCorner] = Services.TextureProvider.GetFromFile(Path.Combine(texturePath, "TopCorner.png"));
        JournalTextures[(int)JournalTexture.BottomCorner] = Services.TextureProvider.GetFromFile(Path.Combine(texturePath, "BottomCorner.png"));
        JournalTextures[(int)JournalTexture.TopBorder] = Services.TextureProvider.GetFromFile(Path.Combine(texturePath, "TopBorder.png"));
        JournalTextures[(int)JournalTexture.TopMiddleBorder] = Services.TextureProvider.GetFromFile(Path.Combine(texturePath, "TopMiddleBorder.png"));
        JournalTextures[(int)JournalTexture.BottomBorder] = Services.TextureProvider.GetFromFile(Path.Combine(texturePath, "BottomBorder.png"));
        JournalTextures[(int)JournalTexture.SideBorder] = Services.TextureProvider.GetFromFile(Path.Combine(texturePath, "SideBorder.png"));
        JournalTextures[(int)JournalTexture.CloseButton] = Services.TextureProvider.GetFromFile(Path.Combine(texturePath, "CloseButton.png"));

        UpdateQuestIcon();

        Axis14FontHandle = GameFontBuilder.GetFont(GameFontFamilyAndSize.Axis12);
        Axis18FontHandle = GameFontBuilder.GetFont(GameFontFamilyAndSize.Axis18);
    }

    protected void UpdateQuestIcon()
    {
        QuestIconTexture = Services.TextureProvider.TryGetFromGameIcon(new GameIconLookup()
        {
            IconId = QuestIconInternal,
            HiRes = true,
            ItemHq = false,
            Language = null,
        }, out var iconTexture)
            ? iconTexture
            : null;
    }

    /// <summary>
    /// Override this function to draw inside the journal
    /// </summary>
    public virtual void DrawJournal()
    {
        
    }

    protected float PreviousHeight = 0f;

    public override void Draw()
    {
        if (!Axis14FontHandle.Available || !Axis18FontHandle.Available)
        {
            return;
        }

        using var fontPusher = Axis14FontHandle.Push();
        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0, 0, 0, 255f));

        DrawBordersAndBackground();
        var windowSize = ImGui.GetWindowSize();
        var interiorSize = windowSize - (Margin * 2f);

        using (var fontTitlePusher = Axis18FontHandle.Push())
        {
            var titleTextSize = ImGui.CalcTextSize(Title);
            ImGui.SetCursorPos(new Vector2((windowSize.X / 2f) - (titleTextSize.X / 2f), PreviousHeight + Margin.Y));
            ImGui.Text(Title);
        }

        ImGui.SetCursorPos(new Vector2(Margin.X, PreviousHeight + Margin.Y + ExtraMarginTop));
        if (ImGui.BeginChild("##JournalChild", interiorSize))
        {
            DrawJournal();
            ImGui.EndChild();
        }

        ImGui.PopStyleColor();
    }

    protected void DrawBordersAndBackground()
    {
        var drawList = ImGui.GetWindowDrawList();

        var sizeWindow = ImGui.GetWindowSize();
        var posTopLeft = ImGui.GetWindowPos();
        var posBottomRight = posTopLeft + sizeWindow;
        var posTopRight = new Vector2(posBottomRight.X, posTopLeft.Y);
        var posBottomLeft = new Vector2(posTopLeft.X, posBottomRight.Y);
        var sizeHalf = sizeWindow / 2f;
        const float baseMultiplier = 1f / 2.05f;
        const float offsetDown = 64f * baseMultiplier;

        #region Calculations & Texture Wraps

        // Top corners
        var topCorner = JournalTextureWrap(JournalTexture.TopCorner);
        var sizeTopCorner = new Vector2(topCorner.Width * baseMultiplier, topCorner.Height * baseMultiplier);
        var posTopLeftCorner = posTopLeft;
        posTopLeftCorner.Y += offsetDown;

        // Top middle
        var topMiddle = JournalTextureWrap(JournalTexture.TopMiddleBorder);
        var sizeTopMiddle = new Vector2(topMiddle.Width * baseMultiplier, topMiddle.Height * baseMultiplier);
        var posTopMiddle = new Vector2(posTopLeft.X + sizeHalf.X - (sizeTopMiddle.X / 2f), posTopLeft.Y + offsetDown);

        var topBorder = JournalTextureWrap(JournalTexture.TopBorder);

        // Bottom corner
        var bottomCorner = JournalTextureWrap(JournalTexture.BottomCorner);
        var sizeBottomCorner = new Vector2(bottomCorner.Width * baseMultiplier, bottomCorner.Height * baseMultiplier);
        var posBottomLeftCorner = new Vector2(posBottomLeft.X, posBottomLeft.Y - sizeBottomCorner.Y);
        var posBottomRightCorner = new Vector2(posBottomRight.X - sizeBottomCorner.X, posBottomRight.Y - sizeBottomCorner.Y);

        // Bottom border
        var bottomBorder = JournalTextureWrap(JournalTexture.BottomBorder);
        var sizeBottomBorder = new Vector2(bottomBorder.Width * baseMultiplier, bottomBorder.Height * baseMultiplier);
        var offsetBottomBorder = new Vector2(sizeBottomCorner.X, -sizeBottomBorder.Y);

        // Side border
        var sideBorder = JournalTextureWrap(JournalTexture.SideBorder);
        var sizeSideBorder = new Vector2(sideBorder.Width * baseMultiplier, sideBorder.Height * baseMultiplier);

        // Close button
        var closeButton = JournalTextureWrap(JournalTexture.CloseButton);
        var sizeCloseButton = new Vector2(closeButton.Width * baseMultiplier * 0.5f, closeButton.Height * baseMultiplier); // divide width by 2 as it has 2 items
        var posCloseButton = posTopRight + new Vector2(0, offsetDown) + new Vector2(-sizeCloseButton.X - (8.0f * baseMultiplier), sizeCloseButton.Y / 2f);

        // For the main background
        var posBackgroundStart = posTopLeft + new Vector2(0, offsetDown + sizeTopMiddle.Y);

        // For use in Draw
        PreviousHeight = posTopMiddle.Y + sizeTopMiddle.Y - posTopLeft.Y;

        #endregion

        #region Drawing

        // Main background
        drawList.AddRectFilled(posBackgroundStart, posBottomRight, ImGui.ColorConvertFloat4ToU32(BackgroundColor));

        // Top cross line
        var posTopLine = posTopLeft + new Vector2(0f, PreviousHeight + Margin.Y + CrossLineHeight);
        drawList.AddLine(posTopLine, posTopLine + new Vector2(sizeWindow.X, 0f), ImGui.ColorConvertFloat4ToU32(new Vector4(0, 0, 0, 0.33f)), 1f); // dark top
        drawList.AddLine(posTopLine + new Vector2(0, 1f), posTopLine + new Vector2(sizeWindow.X, 1f), ImGui.ColorConvertFloat4ToU32(new Vector4(1, 1, 1, 0.33f)), 1f); // light below

        {
            var remainingHeight = sizeWindow.Y - (sizeTopCorner.Y + sizeBottomCorner.Y);

            // Side border
            var sideBorderCount = MathF.Ceiling(remainingHeight / sizeSideBorder.Y);
            for (var i = 0; i < sideBorderCount; i++)
            {
                var currentSideBorderOffset = new Vector2(0, i * sizeSideBorder.Y);
                var posSideBorderLeft = posTopLeft + new Vector2(0, sizeTopCorner.Y) + currentSideBorderOffset;
                var posSideBorderRight = posTopRight + new Vector2(-sizeSideBorder.X, sizeTopCorner.Y) + currentSideBorderOffset;
                // Left side
                drawList.AddImage(sideBorder.ImGuiHandle, posSideBorderLeft, posSideBorderLeft + sizeSideBorder);
                // Right side
                drawList.AddImage(sideBorder.ImGuiHandle, posSideBorderRight, posSideBorderRight + sizeSideBorder, new Vector2(1, 0), new Vector2(0, 1));
            }
        }

        {
            var remainingWidth = sizeWindow.X - (sizeTopCorner.X * 2f);

            // Top border
            var sizeTopBorder = new Vector2(topBorder.Width * baseMultiplier, topBorder.Height * baseMultiplier);
            var topBorderCount = MathF.Ceiling(remainingWidth / sizeTopBorder.X) + 1;
            for (var i = 0; i < topBorderCount; i++)
            {
                var currentTopBorderPosition = posTopLeftCorner + new Vector2(i * sizeTopBorder.X, 0) + new Vector2(sizeTopCorner.X, 0f);
                drawList.AddImage(topBorder.ImGuiHandle, currentTopBorderPosition, currentTopBorderPosition + sizeTopBorder);
            }

            // Top left corner
            drawList.AddImage(topCorner.ImGuiHandle, posTopLeftCorner, posTopLeftCorner + sizeTopCorner);
            // Top right corner
            var posTopRightCorner = new Vector2(posTopRight.X - sizeTopCorner.X, posTopRight.Y + offsetDown);
            drawList.AddImage(topCorner.ImGuiHandle, posTopRightCorner, posTopRightCorner + sizeTopCorner, new Vector2(1, 0), new Vector2(0, 1));

            // Top middle
            drawList.AddImage(topMiddle.ImGuiHandle, posTopMiddle, posTopMiddle + sizeTopMiddle);
        }

        // Quest icon
        var sizeQuestIcon = QuestIconSize;
        var posQuestIcon = new Vector2(posTopLeft.X + sizeHalf.X - (sizeQuestIcon.X / 2f), posTopLeft.Y) + QuestIconOffset;
        if (QuestIconTexture != null)
        {
            drawList.AddImage(QuestIconTexture.GetWrapOrEmpty().ImGuiHandle, posQuestIcon, posQuestIcon + sizeQuestIcon);
        }

        {
            var remainingWidth = sizeWindow.X - (sizeBottomCorner.X * 2f);

            // Bottom border
            var bottomBorderCount = MathF.Ceiling(remainingWidth / sizeBottomBorder.X) + 1;
            for (var i = 0; i < bottomBorderCount; i++)
            {
                var currentBottomBorderPosition = posBottomLeft + new Vector2(i * sizeBottomBorder.X, 0) + offsetBottomBorder;
                drawList.AddImage(bottomBorder.ImGuiHandle, currentBottomBorderPosition, currentBottomBorderPosition + sizeBottomBorder);
            }

            // Bottom left corner
            drawList.AddImage(bottomCorner.ImGuiHandle, posBottomLeftCorner, posBottomLeftCorner + sizeBottomCorner);

            // Bottom right corner
            drawList.AddImage(bottomCorner.ImGuiHandle, posBottomRightCorner, posBottomRightCorner + sizeBottomCorner, new Vector2(1, 0), new Vector2(0, 1));
        }

        #endregion

        #region Close Button

        {
            var cursorPos = ImGui.GetMousePos();
            var closeButtonPos = posCloseButton + (sizeCloseButton / 2f);
            var closeButtonMouseDistance = Vector2.Distance(closeButtonPos, cursorPos);
            var closeButtonHovered = closeButtonMouseDistance < (sizeCloseButton.X / 2f);

            var uvCloseButton = new Vector2(closeButtonHovered ? 0.5f : 0f, 0f);
            drawList.AddImage(closeButton.ImGuiHandle, posCloseButton, posCloseButton + sizeCloseButton, uvCloseButton, new Vector2(closeButtonHovered ? 1.0f : 0.5f, 1.0f));
            if (closeButtonHovered && MouseButtonState.LeftReleased)
            {
                IsOpen = false;
            }
        }

        #endregion
    }
}
