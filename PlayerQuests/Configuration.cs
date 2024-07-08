using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.Numerics;

namespace PlayerQuests;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public bool IsConfigWindowMovable { get; set; } = true;
    public bool SomePropertyToBeSavedAndWithADefault { get; set; } = true;

    public bool showPosPicker = false;
    
    public Vector3 lastWorldPos { get; set; }

    // the below exist just to make saving less cumbersome
    [NonSerialized]
    private IDalamudPluginInterface? pluginInterface;

    public void Initialize(IDalamudPluginInterface pluginInterface)
    {
        this.pluginInterface = pluginInterface;
    }

    public void Save()
    {
        pluginInterface!.SavePluginConfig(this);
    }
}
