using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using FFXIVClientStructs.FFXIV.Common.Component.BGCollision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static FFXIVClientStructs.ThisAssembly;

namespace PlayerQuests.Helpers
{
    public class Raycaster
    {
        public static bool RayCastHit(Vector3 start, Vector3 end)
        {
            var difference = end - start;
            return BGCollisionModule.RaycastMaterialFilter(start, Vector3.Normalize(difference), out var hitInfo, difference.Length());

        }

        public static unsafe bool PointVisible(Vector3 point)
        {
            var cameraManager = CameraManager.Instance();
            if (cameraManager == null)
            {
                return default;
            }

            var camera = cameraManager->CurrentCamera;
            if (camera == null)
            {
                return false;
            }
            return !RayCastHit(camera->Position, point);
        }

    }
}
