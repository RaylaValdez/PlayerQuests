using FFXIVClientStructs.FFXIV.Client.Game.Character;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using PlayerQuests.Manager;

namespace PlayerQuests.Helpers
{
    internal class MarkerUtils
    {
        private static readonly int NaviMapSize = 218;

        public static Vector2 MapSize;
        public static Vector2 MapPos;
        public static Vector2 WindowPos;
        public static Vector2 PlayerPos = new(0, 0);
        public static Vector2 PlayerCirclePos;

        private static float _minimapRadius;


        public static bool ChecksPassed;

        private static Vector2 RotateForMiniMap(Vector2 center, Vector2 pos, float angle)
        {
            var angleInRadians = angle;
            var cosTheta = Math.Cos(angleInRadians);
            var sinTheta = Math.Sin(angleInRadians);

            var rotatedPoint = pos;

            rotatedPoint.X = (float)(cosTheta * (pos.X - center.X) -
            sinTheta * (pos.Y - center.Y) + center.X);

            rotatedPoint.Y = (float)
            (sinTheta * (pos.X - center.X) +
            cosTheta * (pos.Y - center.Y) + center.Y);

            return rotatedPoint;
        }

        public static void PrepareDrawOnMinimap()
        {
            //Get ffxiv window position on screen
            WindowPos = ImGui.GetWindowViewport().Pos;

            //Player Circle position will always be center of the minimap, this is also our pivot point
            PlayerCirclePos = new Vector2(Services.NaviMapManager.X + MapSize.X / 2, Services.NaviMapManager.Y + MapSize.Y / 2) + WindowPos;

            //to line up with minimap pivot better
            PlayerCirclePos.Y -= 5f;

            

            ////for debugging center point
            //Services.NaviMapManager.CircleData.Add(new CircleData(playerCirclePos, circleCategory));

        }

        public static bool RunChecks()
        {
            if (!Services.ClientState.IsLoggedIn)
            {
                return false;
            }
            if (!Services.NaviMapManager.UpdateNaviMap())
            {
                ChecksPassed = false;
                return false;
            }

            if (!Services.NaviMapManager.Visible)
            {
                return false;
            }
            if (Services.NaviMapManager.CheckIfLoading())
            {
                return false;
            }



            MapSize = new Vector2(NaviMapSize * Services.NaviMapManager.NaviScale, NaviMapSize * Services.NaviMapManager.NaviScale);
            _minimapRadius = MapSize.X * 0.315f;
            MapPos = new Vector2(Services.NaviMapManager.X, Services.NaviMapManager.Y);
            Plugin.NaviMapWindow.Size = MapSize;
            Plugin.NaviMapWindow.Position = MapPos;

            unsafe
            {
                var player = (Character*)Services.ObjectTable[0]!.Address;
                if (player == null)
                {
                    return false;
                }

                PlayerPos = new Vector2(player->GameObject.Position.X, player->GameObject.Position.Z);
            }
            ChecksPassed = true;
            return true;
        }

    }
}
