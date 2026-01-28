using GodotUtils.Netcode.Server;
using System;

namespace GodotUtils.Netcode;

/// <summary>
/// A packet sent from the client to the server
/// </summary>
public abstract class ClientPacket : GamePacket
{
    private readonly Type _type;

    public ClientPacket()
    {
        _type = GetType();
    }

    public void Send()
    {
        ENet.Packet enetPacket = CreateENetPacket();
        Peers[0].Send(ChannelId, ref enetPacket);
    }

    public override byte GetOpcode()
    {
        return PacketRegistry.ClientPacketInfoByType[_type].Opcode;
    }

    /// <summary>
    /// The packet handled server-side
    /// </summary>
    public abstract void Handle(ENetServer server, ENet.Peer client);
}
