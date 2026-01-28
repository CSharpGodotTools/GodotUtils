using ENet;
using Godot;
using GodotUtils.Netcode.Client;

namespace GodotUtils.Netcode.Sandbox.Topdown;

public partial class GameClient : GodotClient
{
    protected override void OnConnect(Event netEvent)
    {
        base.OnConnect(netEvent);
        Send(new CPacketPlayerInfo { Username = "Valky", Position = new Vector2(100, 100) });
    }
}
