using System;

namespace GodotUtils.Netcode.Client;

public class PacketData
{
    public ServerPacket HandlePacket { get; set; }
    public PacketReader PacketReader { get; set; }
    public Type         Type         { get; set; }
}
