// ConvexShape.cs
using ImGuiNET;
using System;
using System.Numerics;

namespace PlayerQuests.Drawing
{
    internal class ConvexShape
    {
        internal readonly Brush Brush;
        internal readonly ImDrawListPtr DrawList;
        internal bool cullObject = true;

        internal ConvexShape(Brush brush)
        {
            Brush = brush;
            DrawList = ImGui.GetWindowDrawList();
        }

        internal void Point(Vector3 worldPos)
        {
            var visible = Services.GameGui.WorldToScreen(worldPos, out Vector2 pos);
            DrawList.PathLineTo(pos);
            if (visible) { cullObject = false; }
        }

        internal void PointRadial(Vector3 center, float radius, float radians)
        {
            Point(new Vector3(
                center.X + (radius * (float)Math.Cos(radians)),
                center.Y,
                center.Z + (radius * (float)Math.Sin(radians))
            ));
        }

        internal void Arc(Vector3 center, float radius, float startRads, float endRads)
        {
            int segments = Maths.ArcSegments(startRads, endRads);
            var deltaRads = (endRads - startRads) / segments;

            DrawList.PathClear(); // Clear any previous path
            for (var i = 0; i < segments + 1; i++)
            {
                PointRadial(center, radius, startRads + (deltaRads * i));
            }
        }

        internal void Done()
        {
            if (cullObject)
            {
                DrawList.PathClear();
                return;
            }

            if (Brush.HasFill())
            {
                DrawList.PathFillConvex(ImGui.GetColorU32(Brush.Fill));
            }
            if (Brush.Thickness != 0)
            {
                DrawList.PathStroke(ImGui.GetColorU32(Brush.Color), ImDrawFlags.None, Brush.Thickness);
            }
            DrawList.PathClear();
        }
    }
}
