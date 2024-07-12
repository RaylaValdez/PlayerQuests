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

    public string tempQuestType { get; set; } = null!;

    public string tempQuestName { get; set;} = null!;

    public string tempQuestDescription { get; set; } = null!;

    public string tempQuestObjectiveString { get; set; } = null!;

    public int tempReward {  get; set; } = 0;


    
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
