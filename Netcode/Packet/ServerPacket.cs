using ENet;
using System;
using GodotUtils.Netcode.Client;

namespace GodotUtils.Netcode;

/// <summary>
/// A packet sent from the server to other client(s)
/// </summary>
public abstract class ServerPacket : GamePacket
{
    private SendType _sendType;
    private readonly Type _type;

    public ServerPacket()
    {
        _type = GetType();
    }

    public void Send()
    {
        Packet enetPacket = CreateENetPacket();
        Peers[0].Send(ChannelId, ref enetPacket);
    }

    public void Broadcast(Host host)
    {
        Packet enetPacket = CreateENetPacket();

        if (Peers.Length == 0)
        {
            host.Broadcast(ChannelId, ref enetPacket);
        }
        else if (Peers.Length == 1)
        {
            host.Broadcast(ChannelId, ref enetPacket, Peers[0]);
        }
        else
        {
            host.Broadcast(ChannelId, ref enetPacket, Peers);
        }
    }

    public void SetSendType(SendType sendType)
    {
        _sendType = sendType;
    }

    public SendType GetSendType()
    {
        return _sendType;
    }

    public override byte GetOpcode()
    {
        return PacketRegistry.ServerPacketInfo[_type].Opcode;
    }

    /// <summary>
    /// The packet handled client-side (Godot thread)
    /// </summary>
    public abstract void Handle(ENetClient client);
}

public enum SendType
{
    Peer,
    Broadcast
}
