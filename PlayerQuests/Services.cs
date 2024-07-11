using Dalamud.IoC;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayerQuests.Manager;
using Dalamud.Interface.Windowing;

namespace PlayerQuests
{
    internal class Services
    {
        [PluginService] public static IClientState ClientState { get; private set; } = null!;

        [PluginService] public static ITextureProvider TextureProvider { get; private set; } = null!;

        [PluginService] public static IGameGui GameGui { get; private set; } = null!;

        [PluginService] public static IPluginLog Log { get; private set; } = null!;

        [PluginService] public static IObjectTable ObjectTable { get; private set; } = null!; // nulln't
        
        [PluginService] public static IDataManager DataManager { get; private set; } = null!;

        [PluginService] public static IGameInteropProvider GameInteropProvider { get; private set; } = null!;



        public static NaviMapManager NaviMapManager { get; set; } = null!;

        

    }
}
