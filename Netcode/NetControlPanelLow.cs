using Godot;
using GodotUtils.Netcode.Client;
using GodotUtils.Netcode.Server;
using System;

namespace GodotUtils.Netcode;

public abstract partial class NetControlPanelLow<TGameClient, TGameServer> : Control
    where TGameClient : GodotClient, new()
    where TGameServer : GodotServer, new()
{
    public Net Net { get; private set; }

    [Export] private LineEdit _usernameLineEdit;
    [Export] private LineEdit _ipLineEdit;
    [Export] private Button _startServerBtn;
    [Export] private Button _stopServerBtn;
    [Export] private Button _startClientBtn;
    [Export] private Button _stopClientBtn;

    protected abstract ENetOptions Options { get; set; }

    protected virtual int DefaultMaxClients { get; } = 100;
    protected virtual string DefaultLocalIp { get; } = "127.0.0.1";
    protected virtual ushort DefaultPort { get; } = 25565;

    private string _username = "";
    private ushort _port;
    private string _ip;

    public override void _Ready()
    {
        _port = DefaultPort;
        _ip = DefaultLocalIp;

        ServerFactory serverFactory = new(() => new TGameServer());
        ClientFactory clientFactory = new(() => new TGameClient());

        Net = new Net(clientFactory, serverFactory);

        SetupButtons();
        SetupInputFields();
        SetupClientEvents();
    }

    public override void _Process(double delta)
    {
        Net.Client?.HandlePackets();
    }

    private void SetupButtons()
    {
        _startServerBtn.Pressed += () => Net.StartServer(_port, DefaultMaxClients, Options);
        _stopServerBtn.Pressed += Net.StopServer;
        _startClientBtn.Pressed += OnStartClientBtnPressed;
        _stopClientBtn.Pressed += Net.StopClient;
    }

    private async void OnStartClientBtnPressed()
    {
        await Net.StartClient(_ip, _port);
    }

    private void SetupInputFields()
    {
        _ipLineEdit.TextChanged += OnIpChanged;
        _usernameLineEdit.TextChanged += OnUsernameChanged;
    }

    private void OnIpChanged(string text)
    {
        _ip = FetchIpFromString(text, ref _port);
    }

    private void OnUsernameChanged(string text)
    {
        _username = text.IsAlphaNumeric() ? text : _username;
    }

    private void SetupClientEvents()
    {
        Net.ClientCreated += OnClientCreated;
    }

    private void OnClientCreated(GodotClient client)
    {
        client.Connected += OnClientConnected;
        client.Disconnected += OnClientDisconnected;
    }

    private void OnClientConnected()
    {
        if (!Net.Server.IsRunning)
        {
            _startServerBtn.Disabled = true;
            _stopServerBtn.Disabled = true;
        }

        GetTree().UnfocusCurrentControl();
    }

    private void OnClientDisconnected(DisconnectOpcode opcode)
    {
        _startServerBtn.Disabled = false;
        _stopServerBtn.Disabled = false;
    }

    private static string FetchIpFromString(string ipString, ref ushort port)
    {
        string[] parts = ipString.Split(":");
        string ip = parts[0];

        if (parts.Length > 1 && ushort.TryParse(parts[1], out ushort foundPort))
        {
            port = foundPort;
        }

        return ip;
    }

    private record ClientFactory(Func<GodotClient> Creator) : IGameClientFactory
    {
        public GodotClient CreateClient() => Creator();
    }

    private record ServerFactory(Func<GodotServer> Creator) : IGameServerFactory
    {
        public GodotServer CreateServer() => Creator();
    }
}
