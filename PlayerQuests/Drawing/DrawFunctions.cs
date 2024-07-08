using PlayerQuests.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PlayerQuests.Drawing
{
    internal class DrawFunctions
    {
        internal static void CircleXZ(Vector3 gamePos, float radius, Brush brush)
        {
            var startRads = 0f;
            var endRads = MathF.Tau;
            var shape = new ConvexShape(brush);
            shape.Arc(gamePos, radius, startRads, endRads);
            shape.Done();
        }
    }
}
