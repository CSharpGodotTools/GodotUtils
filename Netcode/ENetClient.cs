﻿using ENet;
using Sandbox2;

namespace GodotUtils.Netcode.Client;

// ENet API Reference: https://github.com/SoftwareGuy/ENet-CSharp/blob/master/DOCUMENTATION.md
public abstract class ENetClient : ENetLow
{
    public bool IsConnected => Interlocked.Read(ref _connected) == 1;

    private ConcurrentQueue<Packet> Incoming { get; } = new();
    protected ConcurrentQueue<ClientPacket> Outgoing { get; } = new();
    private ConcurrentQueue<PacketData> GodotPackets { get; } = new();

    private ConcurrentQueue<Cmd<ENetClientOpcode>> ENetCmds { get; } = new();
    private Peer Peer { get; set; }
    private uint PingInterval { get; } = 1000;
    private uint PeerTimeout { get; } = 5000;
    private uint PeerTimeoutMinimum { get; } = 5000;
    private uint PeerTimeoutMaximum { get; } = 5000;

    private long _connected;

    static ENetClient()
    {
        APacketServer.MapOpcodes();
    }

    public async void Connect(string ip, ushort port, params Type[] ignoredPackets)
    {
        Starting();
        InitIgnoredPackets(ignoredPackets);

        _running = 1;
        CTS = new CancellationTokenSource();
        using var task = Task.Run(() => WorkerThread(ip, port), CTS.Token);

        try
        {
            await task;
        }
        catch (Exception e)
        {
            Logger.LogErr(e, "Client");
        }
    }

    public override void Stop()
    {
        Log("Requesting to stop client..");
        ENetCmds.Enqueue(new Cmd<ENetClientOpcode>(ENetClientOpcode.Disconnect));
    }

    public void Send(APacketClient data = null, PacketFlags flags = PacketFlags.Reliable)
    {
        Outgoing.Enqueue(new ClientPacket(Convert.ToByte(data.GetOpcode()), flags, data));
    }

    protected override void ConcurrentQueues()
    {
        // ENetCmds
        while (ENetCmds.TryDequeue(out Cmd<ENetClientOpcode> cmd))
        {
            if (cmd.Opcode == ENetClientOpcode.Disconnect)
            {
                if (CTS.IsCancellationRequested)
                {
                    Log("Client is in the middle of stopping");
                    break;
                }

                Peer.Disconnect(0);
                DisconnectCleanup(Peer);
            }
        }

        // Incoming
        while (Incoming.TryDequeue(out var packet))
        {
            var packetReader = new PacketReader(packet);
            var opcode = packetReader.ReadByte();

            var type = APacketServer.PacketMapBytes[opcode];
            var handlePacket = APacketServer.PacketMap[type].Instance;

            /*
            * Instead of packets being handled client-side, they are handled
            * on the Godot thread.
            * 
            * Note that handlePacket AND packetReader need to be sent over otherwise
            * the following issue will happen...
            * https://github.com/Valks-Games/Multiplayer-Template/issues/8
            */
            GodotPackets.Enqueue(new PacketData
            {
                Type = type,
                PacketReader = packetReader,
                HandlePacket = handlePacket
            });
        }

        // Outgoing
        while (Outgoing.TryDequeue(out var clientPacket))
        {
            byte channelID = 0; // The channel all networking traffic will be going through
            var packet = default(Packet);
            packet.Create(clientPacket.Data, clientPacket.PacketFlags);
            Peer.Send(channelID, ref packet);
        }
    }

    public void HandlePackets()
    {
        while (Net.Client.GodotPackets.TryDequeue(out PacketData packetData))
        {
            var packetReader = packetData.PacketReader;
            var handlePacket = packetData.HandlePacket;
            var type = packetData.Type;

            handlePacket.Read(packetReader);
            packetReader.Dispose();

            handlePacket.Handle();

            if (!IgnoredPackets.Contains(type))
                Log($"Received packet: {type.Name}\n" +
                    $"{handlePacket.PrintFull()}");
        }
    }

    protected override void Connect(Event netEvent)
    {
        _connected = 1;
        Log("Client connected to server");
    }

    protected override void Disconnect(Event netEvent)
    {
        DisconnectCleanup(Peer);

        var opcode = (DisconnectOpcode)netEvent.Data;
        Log($"Client was {opcode.ToString().ToLower()} from server");
    }

    protected override void Timeout(Event netEvent)
    {
        DisconnectCleanup(Peer);
        Log("Client connection timeout");
    }

    protected override void Receive(Event netEvent)
    {
        var packet = netEvent.Packet;
        if (packet.Length > GamePacket.MaxSize)
        {
            Log($"Tried to read packet from server of size {packet.Length} when max packet size is {GamePacket.MaxSize}");
            packet.Dispose();
            return;
        }

        Incoming.Enqueue(packet);
    }

    private void WorkerThread(string ip, ushort port)
    {
        Host = new Host();
        var address = new Address {
            Port = port
        };

        address.SetHost(ip);
        Host.Create();

        Peer = Host.Connect(address);
        Peer.PingInterval(PingInterval);
        Peer.Timeout(PeerTimeout, PeerTimeoutMinimum, PeerTimeoutMaximum);

        WorkerLoop();

        Host.Dispose();
        Log("Client is no longer running");
    }

    protected override void DisconnectCleanup(Peer peer)
    {
        base.DisconnectCleanup(peer);
        _connected = 0;
    }

    public override void Log(object message, ConsoleColor color = ConsoleColor.Cyan) => 
        Logger.Log($"[Client] {message}", color);
}

public enum ENetClientOpcode
{
    Disconnect
}

public class PacketData
{
    public Type Type { get; set; }
    public PacketReader PacketReader { get; set; }
    public APacketServer HandlePacket { get; set; }
}
