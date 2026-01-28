namespace GodotUtils.Netcode.Sandbox.Topdown;

public partial class NetControlPanel : NetControlPanelLow<GameClient, GameServer>
{
    protected override ENetOptions Options { get; set; } = new()
    {
        PrintPacketByteSize = true,
        PrintPacketData = true,
        PrintPacketReceived = true,
        PrintPacketSent = true
    };
}
