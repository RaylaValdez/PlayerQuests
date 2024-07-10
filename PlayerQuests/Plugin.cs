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

namespace PlayerQuests;

public sealed class Plugin : IDalamudPlugin
{
    private const string CommandName = "/pq";

    public static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    private ICommandManager CommandManager { get; init; }
    public static Configuration? Configuration { get; private set; }
    

    public readonly WindowSystem WindowSystem = new("PlayerQuest");
    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }
    private Canvas CanvasWindow {  get; init; }

    public Plugin(
        IDalamudPluginInterface pluginInterface,
        ICommandManager commandManager,
        ITextureProvider textureProvider)
    {
        PluginInterface = (IDalamudPluginInterface)pluginInterface;
        CommandManager = commandManager;

        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Configuration.Initialize(PluginInterface);

        pluginInterface.Create<Services>();

        ConfigWindow = new ConfigWindow(this);
        MainWindow = new MainWindow(this);
        CanvasWindow = new Canvas();


        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);
        WindowSystem.AddWindow(CanvasWindow);

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
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();


        CommandManager.RemoveHandler(CommandName);
    }

    private void OnCommand(string command, string args)
    {
        MainWindow.Toggle();
    }
    private void DrawUI() => WindowSystem.Draw();

    public void ToggleConfigUI() => ConfigWindow.Toggle();
    public void ToggleMainUI() => MainWindow.Toggle();



    
}
