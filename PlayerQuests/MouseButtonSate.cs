using Dalamud.Game.ClientState.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PlayerQuests
{
    public static class MouseButtonState
    {
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(UInt16 virtualKeyCode);

        private static readonly bool[] PrevPressed = [false, false, false];
        private static readonly bool[] Pressed = [false, false, false];

        /// <summary>
        /// Call once per frame to update the state
        /// </summary>
        public static void UpdateState()
        {
            // Copy previous state
            PrevPressed[0] = Pressed[0];
            PrevPressed[1] = Pressed[1];
            PrevPressed[2] = Pressed[2];

            // Set new state
            Pressed[0] = GetAsyncKeyState((ushort)VirtualKey.LBUTTON) != 0;
            Pressed[1] = GetAsyncKeyState((ushort)VirtualKey.RBUTTON) != 0;
            Pressed[2] = GetAsyncKeyState((ushort)VirtualKey.MBUTTON) != 0;
        }

        // Get if the mouse button is currently down:
        public static bool LeftDown => Pressed[0];
        public static bool RightDown => Pressed[1];
        public static bool MiddleDown => Pressed[2];

        // Get if the mouse button has just been pressed down:
        public static bool LeftPressed => Pressed[0] && !PrevPressed[0];
        public static bool RightPressed => Pressed[1] && !PrevPressed[1];
        public static bool MiddlePressed => Pressed[2] && !PrevPressed[2];

        public static bool LeftReleased => !Pressed[0] && PrevPressed[0];
        public static bool RightReleased => !Pressed[1] && PrevPressed[1];
        public static bool MiddleReleased => !Pressed[2] && PrevPressed[2];
    }
}
