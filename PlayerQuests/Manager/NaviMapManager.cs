using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;

namespace PlayerQuests.Manager
{
    public unsafe class NaviMapManager : IDisposable
    {
        public int X;

        public int Y;

        public float NaviScale;

        public float ZoneScale;

        public float Rotation;

        public bool Visible;

        public float Zoom;

        public short OffsetX;
        public short OffsetY;

        public bool Loading;

        public bool DebugMode = false;

        public bool IsLocked;

        public bool InCombat { get; set; }

        private AtkUnitBase* NaviMapPointer => (AtkUnitBase*)Services.GameGui.GetAddonByName("_NaviMap");

        private readonly ExcelSheet<Map>? _maps;


        public NaviMapManager()
        {
            Services.GameInteropProvider.InitializeFromAttributes(this);

            _maps = Services.DataManager.GetExcelSheet<Map>();
            UpdateNaviMap();
            UpdateMap();
        }


        public bool UpdateNaviMap()
        {

            if (NaviMapPointer == null)
            {
                return false;
            }

            //There's probably a better way of doing this but I don't know it for now
            IsLocked = ((AtkComponentCheckBox*)NaviMapPointer->GetNodeById(4)->GetComponent())->IsChecked;

            if (NaviMapPointer->UldManager.LoadedState != AtkLoadState.Loaded)
            {
                return false;
            }
            try
            {
                Rotation = NaviMapPointer->GetNodeById(8)->Rotation;
                Zoom = NaviMapPointer->GetNodeById(18)->GetComponent()->GetImageNodeById(6)->ScaleX;
            }
            catch
            {
                // ignored
            }

            X = NaviMapPointer->X;
            Y = NaviMapPointer->Y;
            NaviScale = NaviMapPointer->Scale;
            Visible = (NaviMapPointer->IsVisible && NaviMapPointer->VisibilityFlags == 0);

            return true;
        }

        public bool CheckIfLoading()
        {
            var locationTitle = (AtkUnitBase*)Services.GameGui.GetAddonByName("_LocationTitle");
            var fadeMiddle = (AtkUnitBase*)Services.GameGui.GetAddonByName("FadeMiddle");
            return Loading =
                locationTitle->IsVisible ||
                fadeMiddle->IsVisible;
        }

        public void UpdateMap()
        {
            if (_maps != null)
            {
                var map = _maps.GetRow(GetMapId());

                if (map == null) { return; }

                if (map.SizeFactor != 0)
                {
                    ZoneScale = (float)map.SizeFactor / 100;
                }
                else
                {
                    ZoneScale = 1;
                }
                OffsetX = map.OffsetX;
                OffsetY = map.OffsetY;

            }
        }

        private uint GetMapId()
        {
            return AgentMap.Instance()->CurrentMapId;
        }


        public void Dispose()
        {

        }
    }
}
