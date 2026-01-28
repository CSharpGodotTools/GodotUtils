using GodotUtils.Netcode.Client;

namespace GodotUtils.Netcode;

public interface IGameClientFactory
{
    GodotClient CreateClient();
}
