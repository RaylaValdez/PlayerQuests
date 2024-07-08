using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FFXIVClientStructs.FFXIV.Component.GUI.AtkUIColorHolder.Delegates;
using static System.Net.Mime.MediaTypeNames;

namespace PlayerQuests.Drawing;

public class QuestNameplateNode : NodeBase<AtkResNode>
{
    private readonly TextNode questNameplateTextNode;



    public QuestNameplateNode(uint nodeID) : base(NodeType.Res)
    {
        NodeID = nodeID;
        Width = 100f;
        Height = 100f;
        IsVisible = true;
        Margin = new Spacing(5.0f);


        var screenPosForText = new System.Numerics.Vector2(0, 0);
        var inView = false;

        PluginHelpers.questLocation = Plugin.Configuration!.lastWorldPos;


        Services.GameGui.WorldToScreen(PluginHelpers.questLocation + PluginHelpers.iconMaxOffset, out screenPosForText, out inView);

        questNameplateTextNode = new TextNode
        {
            NodeID = 300000 + nodeID,
            Position = screenPosForText,
            Size = new Vector2(250.0f + 24.0f, 32.0f),
            TextColor = KnownColor.White.Vector(),
            TextOutlineColor = KnownColor.Black.Vector(),
            IsVisible = true,
            FontSize = 26,
            FontType = FontType.Axis,
            TextFlags = TextFlags.Edge,
            TextFlags2 = TextFlags2.Ellipsis,
            Text = "Warning Will Robinson This Is Super Long!",
        };

        NodeSystem.nativeController.AttachToNode(questNameplateTextNode, this, NodePosition.AsLastChild);

    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            NodeSystem.nativeController.DetachFromNode(questNameplateTextNode);

            questNameplateTextNode.Dispose();

            base.Dispose(disposing);
        }
    }


    public void UpdateStyle()
    {
        questNameplateTextNode.IsVisible = PluginHelpers.dummyIconVisible;
    }
}
