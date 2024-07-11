using System;
using ImGuiNET;
using Dalamud.Interface.GameFonts;
using Dalamud.Interface.ManagedFontAtlas;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Windowing;
using PlayerQuests.Helpers;

namespace PlayerQuests.Windows
{
    internal class WindowHelpers
    {
        public static IFontHandle TrumpGothicFontHandle { get; set; } = null!;

        public static void ImGuiTextWithDropShadow(string text, float shadowThickness, int shadowLayers = 10, bool gothic = false)
        {
            if (string.IsNullOrEmpty(text))
                return;

            var originalPos = ImGui.GetCursorPos();

            // Text color (white)
            var textColor = new System.Numerics.Vector4(1, 1, 1, 1);

            if (gothic)
            {
                var fontPusher = TrumpGothicFontHandle.Push();
            }

            for (int i = 1; i <= shadowLayers; i++)
            {
                float offset = i * shadowThickness / shadowLayers;
                float alpha = 0.15f * (shadowLayers - i + 1) / shadowLayers; // more transparent for farther shadows

                // Shadow color (varying transparency black)
                var shadowColor = new System.Numerics.Vector4(0, 0, 0, alpha);
                
                // Draw shadow layers at different offsets in all directions
                ImGui.SetCursorPos(new System.Numerics.Vector2(originalPos.X + offset, originalPos.Y + offset));
                ImGui.TextColored(shadowColor, text);
                ImGui.SetCursorPos(new System.Numerics.Vector2(originalPos.X - offset, originalPos.Y + offset));
                ImGui.TextColored(shadowColor, text);
                ImGui.SetCursorPos(new System.Numerics.Vector2(originalPos.X + offset, originalPos.Y - offset));
                ImGui.TextColored(shadowColor, text);
                ImGui.SetCursorPos(new System.Numerics.Vector2(originalPos.X - offset, originalPos.Y - offset));
                ImGui.TextColored(shadowColor, text);
            }

            // Draw the actual text
            ImGui.SetCursorPos(originalPos);
            ImGui.TextColored(textColor, text);

            if (gothic)
            {
                TrumpGothicFontHandle.Pop();
            }
        }
    }
}
