#if NETCODE_ENABLED
using ENet;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System;
using System.IO;
using GodotUtils.Framework;

namespace GodotUtils.Netcode.Server;

// ENet API Reference: https://github.com/SoftwareGuy/ENet-CSharp/blob/master/DOCUMENTATION.md
public abstract class ENetServer : ENetLow
{
    protected ConcurrentQueue<Cmd<ENetServerOpcode>> ENetCmds { get; } = new();
    protected System.Timers.Timer EmitLoop { get; set; }

    private readonly ConcurrentQueue<(Packet, Peer)> _incoming = new();
    private readonly ConcurrentQueue<ServerPacket> _outgoing = new();

    /// <summary>
    /// This Dictionary is NOT thread safe and should only be accessed on the ENet Thread
    /// </summary>
    private readonly Dictionary<uint, Peer> _peers = [];

    /// <summary>
    /// Log a message as the server. This function is thread safe.
    /// </summary>
    public override void Log(object message, BBColor color = BBColor.Green)
    {
        GameFramework.Logger.Log($"[Server] {message}", color);
    }

    /// <summary>
    /// Kick everyone on the server with a specified opcode. Thread safe.
    /// </summary>
    public void KickAll(DisconnectOpcode opcode)
    {
        ENetCmds.Enqueue(new Cmd<ENetServerOpcode>(ENetServerOpcode.KickAll, opcode));
    }

    protected abstract void OnEmit();

    protected void EnqueuePacket(ServerPacket packet)
    {
        _outgoing.Enqueue(packet);
    }

    protected override void ConcurrentQueues()
    {
        ProcessEnetCommands();
        ProcessIncomingPackets();
        ProcessOutgoingPackets();
    }

    protected override void OnConnect(Event netEvent)
    {
        _peers[netEvent.Peer.ID] = netEvent.Peer;
        Log("Client connected - ID: " + netEvent.Peer.ID);
    }

    protected abstract void OnDisconnected(Event netEvent);

    protected override void OnDisconnect(Event netEvent)
    {
        _peers.Remove(netEvent.Peer.ID);
        Log("Client disconnected - ID: " + netEvent.Peer.ID);
        OnDisconnected(netEvent);
    }

    protected override void OnTimeout(Event netEvent)
    {
        _peers.Remove(netEvent.Peer.ID);
        Log("Client timeout - ID: " + netEvent.Peer.ID);
        OnDisconnected(netEvent);
    }

    protected override void OnReceive(Event netEvent)
    {
        Packet packet = netEvent.Packet;

        if (packet.Length > GamePacket.MaxSize)
        {
            Log($"Tried to read packet from client of size {packet.Length} when max packet size is {GamePacket.MaxSize}");
            packet.Dispose();
            return;
        }

        _incoming.Enqueue((packet, netEvent.Peer));
    }

    protected void WorkerThread(ushort port, int maxClients)
    {
        Host = CreateServerHost(port, maxClients);

        if (Host == null)
            return;

        Log("Server is running");

        try
        {
            WorkerLoop();
        }
        finally
        {
            Host.Dispose();
        }
        
        Log("Server has stopped");
    }

    protected override void OnDisconnectCleanup(Peer peer)
    {
        base.OnDisconnectCleanup(peer);
        _peers.Remove(peer.ID);
    }

    /// <returns>Host or null if failed to create host</returns>
    private Host CreateServerHost(ushort port, int maxClients)
    {
        Host host = new();

        try
        {
            host.Create(new Address { Port = port }, maxClients);
        }
        catch (InvalidOperationException e)
        {
            Log($"A server is running on port {port} already! {e.Message}");
            return null;
        }

        return host;
    }

    private void ProcessEnetCommands()
    {
        while (ENetCmds.TryDequeue(out Cmd<ENetServerOpcode> cmd))
        {
            switch (cmd.Opcode)
            {
                case ENetServerOpcode.Stop:
                    HandleStopCommand();
                    break;

                case ENetServerOpcode.Kick:
                    HandleKickCommand(cmd);
                    break;

                case ENetServerOpcode.KickAll:
                    HandleKickAllCommand(cmd);
                    break;
            }
        }
    }

    private void HandleStopCommand()
    {
        KickAll(DisconnectOpcode.Stopping);

        if (CTS.IsCancellationRequested)
        {
            Log("Server is in the middle of stopping");
            return;
        }

        CTS.Cancel();
    }

    private void HandleKickCommand(Cmd<ENetServerOpcode> cmd)
    {
        uint id = (uint)cmd.Data[0];
        DisconnectOpcode opcode = (DisconnectOpcode)cmd.Data[1];

        if (!_peers.TryGetValue(id, out Peer peer))
        {
            Log($"Tried to kick peer with id '{id}' but this peer does not exist");
            return;
        }

        peer.DisconnectNow((uint)opcode);
        _peers.Remove(id);
    }

    private void HandleKickAllCommand(Cmd<ENetServerOpcode> cmd)
    {
        DisconnectOpcode opcode = (DisconnectOpcode)cmd.Data[0];

        foreach (Peer peer in _peers.Values)
        {
            peer.DisconnectNow((uint)opcode);
        }

        _peers.Clear();
    }

    private void ProcessIncomingPackets()
    {
        while (_incoming.TryDequeue(out (Packet enetPacket, Peer peer) packetPeer))
        {
            PacketReader reader = new(packetPeer.enetPacket);

            try
            {
                if (!TryGetPacketHandler(reader, out ClientPacket handler, out Type type))
                    continue;

                if (!TryReadPacket(handler, reader, out string err))
                {
                    Log($"Received malformed packet: {err} (Ignoring)");
                    continue;
                }

                handler.Handle(this, packetPeer.peer);
                LogPacketReceived(type, packetPeer.peer.ID, handler);
            }
            finally
            {
                reader.Dispose();
            }
        }
    }

    private bool TryGetPacketHandler(PacketReader reader, out ClientPacket handler, out Type type)
    {
        handler = null;

        // Note: reader is positioned at start of packet when constructed
        byte opcode = reader.ReadByte();

        if (!PacketRegistry.ClientPacketTypeByOpcode.TryGetValue(opcode, out type))
        {
            Log($"Received malformed opcode: {opcode} (Ignoring)");
            return false;
        }

        handler = PacketRegistry.ClientPacketInfoByType[type].Instance;
        return true;
    }

    private static bool TryReadPacket(ClientPacket handler, PacketReader reader, out string error)
    {
        error = null;

        try
        {
            handler.Read(reader);
            return true;
        }
        catch (EndOfStreamException e)
        {
            error = e.Message;
            return false;
        }
    }

    private void LogPacketReceived(Type type, uint clientId, ClientPacket packet)
    {
        if (!IgnoredPackets.Contains(type) && Options.PrintPacketReceived)
        {
            string packetData = Options.PrintPacketData ? $"\n{packet.ToFormattedString()}" : string.Empty;

            Log($"Received packet: {type.Name} from client {clientId}{packetData}", BBColor.LightGreen);
        }
    }

    private void ProcessOutgoingPackets()
    {
        while (_outgoing.TryDequeue(out ServerPacket packet))
        {
            SendType sendType = packet.GetSendType();

            switch (sendType)
            {
                case SendType.Peer:
                    packet.Send();
                    break;

                case SendType.Broadcast:
                    packet.Broadcast(Host);
                    break;
            }
        }
    }
}
#endif
