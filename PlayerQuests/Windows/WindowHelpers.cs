using System;
using ImGuiNET;

namespace PlayerQuests.Windows
{
    internal class WindowHelpers
    {
        public static void ImGuiTextWithDropShadow(string text, float shadowThickness, int shadowLayers = 10)
        {
            if (string.IsNullOrEmpty(text))
                return;

            var originalPos = ImGui.GetCursorPos();

            // Text color (white)
            var textColor = new System.Numerics.Vector4(1, 1, 1, 1);

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
        }
    }
}
