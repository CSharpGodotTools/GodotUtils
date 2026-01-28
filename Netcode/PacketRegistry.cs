using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GodotUtils.Netcode;

public static class PacketRegistry
{
    public static Dictionary<Type, PacketInfo<ClientPacket>> ClientPacketInfoByType { get; private set; }
    public static Dictionary<byte, Type> ClientPacketTypeByOpcode { get; private set; }

    public static Dictionary<Type, PacketInfo<ServerPacket>> ServerPacketInfo { get; private set; }
    public static Dictionary<byte, Type> ServerPacketTypes { get; private set; }

    static PacketRegistry()
    {
        Type[] cachedTypes = Assembly.GetExecutingAssembly().GetTypes();

        ClientPacketInfoByType = MapPackets<ClientPacket>(cachedTypes);
        ClientPacketTypeByOpcode = ClientPacketInfoByType.ToDictionary(kvp => kvp.Value.Opcode, kvp => kvp.Key);

        ServerPacketInfo = MapPackets<ServerPacket>(cachedTypes);
        ServerPacketTypes = ServerPacketInfo.ToDictionary(kvp => kvp.Value.Opcode, kvp => kvp.Key);
    }

    private static Dictionary<Type, PacketInfo<T>> MapPackets<T>(Type[] cachedTypes)
    {
        Type[] packetTypes = [.. cachedTypes
            .Where(x => typeof(T).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
            .OrderBy(x => x.Name)];

        Dictionary<Type, PacketInfo<T>> dict = [];

        for (byte i = 0; i < packetTypes.Length; i++)
        {
            dict.Add(packetTypes[i], new PacketInfo<T>
            {
                Opcode = i,
                Instance = (T)Activator.CreateInstance(packetTypes[i])
            });
        }

        return dict;
    }
}

public class PacketInfo<T>
{
    public byte Opcode { get; set; }
    public T Instance { get; set; }
}
