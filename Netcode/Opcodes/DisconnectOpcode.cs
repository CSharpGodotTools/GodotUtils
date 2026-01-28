namespace GodotUtils.Netcode;

public enum DisconnectOpcode
{
    Disconnected,
    Maintenance,
    Restarting,
    Stopping,
    Timeout,
    Kicked,
    Banned
}
