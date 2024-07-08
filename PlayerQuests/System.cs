using KamiToolKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerQuests
{
    public static class System
    {
        public static NativeController nativeController { get; set; } = null!;

        public static NativeUiOverlayController nativeUiOverlay { get; set; } = null!;
    }
}
