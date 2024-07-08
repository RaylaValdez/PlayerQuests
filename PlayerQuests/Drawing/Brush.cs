using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PlayerQuests.Drawing;

[Serializable]
public struct Brush
{
    public float Thickness;
    public Vector4 Color;
    public Vector4 Fill;

    public readonly bool HasFill()
    {
        return Fill.W != 0;
    }
}
