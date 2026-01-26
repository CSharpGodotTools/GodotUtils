#if NETCODE_ENABLED
using ENet;
using System.Collections.Concurrent;
using System;
using GodotUtils.Framework;

namespace GodotUtils.Netcode.Client;

// ENet API Reference: https://github.com/SoftwareGuy/ENet-CSharp/blob/master/DOCUMENTATION.md
public abstract class ENetClient : ENetLow
{
    // Protected Members
    protected ConcurrentQueue<Cmd<ENetClientOpcode>> ENetCmds          { get; } = new();
    protected ConcurrentQueue<Cmd<GodotOpcode>>      GodotCmdsInternal { get; } = new();
    protected ConcurrentQueue<ClientPacket>          Outgoing          { get; } = new();
    protected ConcurrentQueue<PacketData>            GodotPackets      { get; } = new();

    protected Peer _peer;
    protected long _connected;

    // Private Members
    private const uint PingIntervalMs = 1000;
    private const uint PeerTimeoutMs = 5000;
    private const uint PeerTimeoutMinimumMs = 5000;
    private const uint PeerTimeoutMaximumMs = 5000;

    private readonly ConcurrentQueue<Packet> _incoming = new();

    /// <summary>
    /// Log messages as the client. Thread safe.
    /// </summary>
    public override void Log(object message, BBColor color = BBColor.Aqua)
    {
        GameFramework.Logger.Log($"[Client] {message}", color);
    }

    protected override void ConcurrentQueues()
    {
        ProcessENetCommands();
        ProcessIncomingPackets();
        ProcessOutgoingPackets();
    }

    protected override void OnConnect(Event netEvent)
    {
        _connected = 1;
        GodotCmdsInternal.Enqueue(new Cmd<GodotOpcode>(GodotOpcode.Connected));
        Log("Client connected to server");
    }

    protected override void OnDisconnect(Event netEvent)
    {
        DisconnectOpcode opcode = (DisconnectOpcode)netEvent.Data;
        
        GodotCmdsInternal.Enqueue(new Cmd<GodotOpcode>(GodotOpcode.Disconnected, opcode));
        
        OnDisconnectCleanup(_peer);

        Log($"Received disconnect opcode from server: {opcode.ToString().ToLower()}");
    }

    protected override void OnTimeout(Event netEvent)
    {
        // I do not remember why I enqueued both a Timeout AND a Disconnected Godot cmds
        GodotCmdsInternal.Enqueue(new Cmd<GodotOpcode>(GodotOpcode.Disconnected, DisconnectOpcode.Timeout));
        GodotCmdsInternal.Enqueue(new Cmd<GodotOpcode>(GodotOpcode.Timeout));

        OnDisconnectCleanup(_peer);
        Log("Client connection timeout");
    }

    protected override void OnReceive(Event netEvent)
    {
        Packet packet = netEvent.Packet;
        if (packet.Length > GamePacket.MaxSize)
        {
            Log($"Tried to read packet from server of size {packet.Length} when max packet size is {GamePacket.MaxSize}");
            
            packet.Dispose();
            return;
        }

        _incoming.Enqueue(packet);
    }

    protected override void OnDisconnectCleanup(Peer peer)
    {
        base.OnDisconnectCleanup(peer);
        _connected = 0;
    }

    protected void WorkerThread(string ip, ushort port)
    {
        Host = new Host();
        Host.Create();

        _peer = Host.Connect(CreateAddress(ip, port));
        _peer.PingInterval(PingIntervalMs);
        _peer.Timeout(PeerTimeoutMs, PeerTimeoutMinimumMs, PeerTimeoutMaximumMs);

        try
        {
            WorkerLoop();
        }
        finally
        {
            Host.Dispose();
        }
        
        Log("Client has stopped");
    }

    private void ProcessENetCommands()
    {
        while (ENetCmds.TryDequeue(out Cmd<ENetClientOpcode> cmd))
        {
            if (cmd.Opcode == ENetClientOpcode.Disconnect)
            {
                if (CTS.IsCancellationRequested)
                {
                    Log("Client is in the middle of stopping");
                    break;
                }

                _peer.Disconnect((uint)DisconnectOpcode.Disconnected);
                OnDisconnectCleanup(_peer);
            }
        }
    }

    private void ProcessIncomingPackets()
    {
        while (_incoming.TryDequeue(out Packet packet))
        {
            PacketReader packetReader = new(packet);
            byte opcode = packetReader.ReadByte();

            Type type = PacketRegistry.ServerPacketTypes[opcode];
            ServerPacket handlePacket = PacketRegistry.ServerPacketInfo[type].Instance;

            /*
             * Instead of packets being handled client-side, they are handled on the Godot thread.
             * Note that handlePacket AND packetReader need to be sent over.
             */
            GodotPackets.Enqueue(new PacketData
            {
                Type = type,
                PacketReader = packetReader,
                HandlePacket = handlePacket
            });
        }
    }

    private void ProcessOutgoingPackets()
    {
        while (Outgoing.TryDequeue(out ClientPacket clientPacket))
        {
            Type type = clientPacket.GetType();

            LogOutgoingPacket(type, clientPacket);
            clientPacket.Send();
        }
    }

    private void LogOutgoingPacket(Type type, ClientPacket clientPacket)
    {
        if (!IgnoredPackets.Contains(type) && Options.PrintPacketSent)
        {
            Log($"Sent packet: {type.Name} {FormatByteSize(clientPacket.GetSize())}" +
                $"{(Options.PrintPacketData ? $"\n{clientPacket.ToFormattedString()}" : "")}");
        }
    }

    private static Address CreateAddress(string ip, ushort port)
    {
        Address address = new() { Port = port };
        address.SetHost(ip);
        return address;
    }
}
#endif
