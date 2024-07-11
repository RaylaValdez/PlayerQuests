using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using PlayerQuests.Windows;
using PlayerQuests.Drawing;
using Dalamud.Interface.Textures.TextureWraps;
using Lumina.Excel.GeneratedSheets2;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using PlayerQuests;
using System.Collections.Generic;
using ImGuiNET;
using FFXIVClientStructs.FFXIV.Client.System.Input;
using Dalamud.Game.Addon.Events;
using Dalamud.Interface.GameFonts;
using PlayerQuests.Helpers;
using PlayerQuests.Manager;

namespace PlayerQuests;

public sealed class Plugin : IDalamudPlugin
{
    private const string CommandName = "/pq";

    public static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    private ICommandManager CommandManager { get; init; }
    public static Configuration Configuration { get; private set; } = null!;

    public static Plugin Instance { get; private set; } = null!;

    public static WindowSystem WindowSystem = new("PlayerQuest");
    public static MiniMap NaviMapWindow = new();

    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }
    private Canvas CanvasWindow { get; init; }
    private JournalWindow JournalWindow { get; init; }
    public static DummyQuestInteractWindow DummyWindow { get; private set; } = null!;

    public Plugin(
        IDalamudPluginInterface pluginInterface,
        ICommandManager commandManager,
        ITextureProvider textureProvider)
    {
        Instance = this;
        PluginInterface = (IDalamudPluginInterface)pluginInterface;
        CommandManager = commandManager;

        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Configuration.Initialize(PluginInterface);


        
        pluginInterface.Create<Services>();
        Services.NaviMapManager = new NaviMapManager();

        ConfigWindow = new ConfigWindow(this);
        MainWindow = new MainWindow(this);
        CanvasWindow = new Canvas();
        JournalWindow = new JournalWindow("Journal Window 1");
        DummyWindow = new DummyQuestInteractWindow();

        WindowHelpers.TrumpGothicFontHandle = GameFontBuilder.GetFont(GameFontFamilyAndSize.TrumpGothic23);

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);
        WindowSystem.AddWindow(CanvasWindow);
        WindowSystem.AddWindow(JournalWindow);
        WindowSystem.AddWindow(DummyWindow);    
        WindowSystem.AddWindow(NaviMapWindow);
        

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "A useful message to display in /xlhelp"
        });

        PluginInterface.UiBuilder.Draw += DrawUI;

        // This adds a button to the plugin installer entry of this plugin which allows
        // to toggle the display status of the configuration ui
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;

        // Adds another button that is doing the same but for the main ui of the plugin
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;

        Services.ClientState.TerritoryChanged += TerritoryChanged;
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();

        Services.ClientState.TerritoryChanged -= TerritoryChanged;

        CommandManager.RemoveHandler(CommandName);
    }

    private void OnCommand(string command, string args)
    {
        MainWindow.Toggle();
    }

    private void DrawUI()
    {
        MouseButtonState.UpdateState();
        WindowSystem.Draw();
    }

    public void ToggleConfigUI() => ConfigWindow.Toggle();
    public void ToggleMainUI() => MainWindow.Toggle();
    public void ToggleJournalWindow() => JournalWindow.Toggle();
    public void ToggleDummyWindow() => DummyWindow.Toggle();

    private void TerritoryChanged(ushort _)
    {
        Services.NaviMapManager.UpdateMap();

    }
}
