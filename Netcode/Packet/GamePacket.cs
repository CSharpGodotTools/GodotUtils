using ENet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GodotUtils.Netcode;

/// <summary>
/// A base class with common functionality for Client and Server packets
/// </summary>
public abstract class GamePacket
{
    private static readonly Dictionary<Type, PropertyInfo[]> _netSendPropertyCache = [];

    public static int MaxSize => 8192;

    protected Peer[] Peers { get; private set; }
    protected byte ChannelId { get; }

    // Packets are reliable by default
    private readonly PacketFlags _packetFlags = PacketFlags.Reliable;
    private long _size;
    private byte[] _data;

    public void Write()
    {
        using PacketWriter writer = new();
        writer.Write(GetOpcode());
        Write(writer);

        _data = writer.Stream.ToArray();
        _size = writer.Stream.Length;
    }

    public void SetPeer(Peer peer)
    {
        Peers = [peer];
    }

    public void SetPeers(Peer[] peers)
    {
        Peers = peers;
    }

    public long GetSize()
    {
        return _size;
    }

    public abstract byte GetOpcode();

    public virtual void Write(PacketWriter writer)
    {
        PropertyInfo[] properties = GetNetSendProperties();

        foreach (PropertyInfo property in properties)
        {
            writer.Write(property.GetValue(this));
        }
    }

    public virtual void Read(PacketReader reader)
    {
        PropertyInfo[] properties = GetNetSendProperties();

        foreach (PropertyInfo property in properties)
        {
            property.SetValue(this, reader.Read(property.PropertyType));
        }
    }

    private PropertyInfo[] GetNetSendProperties()
    {
        Type type = GetType();

        // Properties are cached by type instead of per instance for improved performance
        if (!_netSendPropertyCache.TryGetValue(type, out PropertyInfo[] props))
        {
            props = [.. type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.GetCustomAttributes(typeof(NetSendAttribute), true).Length != 0)
                .OrderBy(p => ((NetSendAttribute)p.GetCustomAttributes(typeof(NetSendAttribute), true).First()).Order)];

            _netSendPropertyCache[type] = props;
        }

        return props;
    }

    protected Packet CreateENetPacket()
    {
        Packet enetPacket = default;
        enetPacket.Create(_data, _packetFlags);
        return enetPacket;
    }
}
