#if NETCODE_ENABLED
using ENet;
using System.Collections.Generic;
using System.Threading;
using System;
using GodotUtils.Framework;

namespace GodotUtils.Netcode;

/// <summary>
/// ENetServer and ENetClient both extend from this class.
/// </summary>
public abstract class ENetLow
{
    // Properties
    protected Host Host { get; set; }
    protected CancellationTokenSource CTS { get; set; }
    protected ENetOptions Options { get; set; }
    protected HashSet<Type> IgnoredPackets { get; private set; } = [];
    
    // Fields
    protected long _running; // Interlocked.Read requires this to be a field

    // Methods
    public bool IsRunning => Interlocked.Read(ref _running) == 1;
    public abstract void Log(object message, BBColor color);
    public abstract void Stop();

    protected virtual void OnDisconnectCleanup(Peer peer)
    {
        CTS.Cancel();
    }

    protected void InitIgnoredPackets(Type[] ignoredPackets)
    {
        IgnoredPackets = new HashSet<Type>(ignoredPackets);
    }

    protected void WorkerLoop()
    {
        while (!CTS.IsCancellationRequested)
        {
            bool polled = false;

            ConcurrentQueues();

            while (!polled)
            {
                if (Host.CheckEvents(out Event netEvent) <= 0)
                {
                    if (Host.Service(15, out netEvent) <= 0)
                    {
                        break;
                    }

                    polled = true;
                }

                switch (netEvent.Type)
                {
                    case EventType.None:
                        // do nothing
                        break;
                    case EventType.Connect:
                        OnConnect(netEvent);
                        break;
                    case EventType.Disconnect:
                        OnDisconnect(netEvent);
                        break;
                    case EventType.Timeout:
                        OnTimeout(netEvent);
                        break;
                    case EventType.Receive:
                        OnReceive(netEvent);
                        break;
                }
            }
        }

        Host.Flush();
        _running = 0;
        OnStopped();
    }

    protected abstract void ConcurrentQueues();
    protected virtual void OnStopped() { }
    protected virtual void OnStarting() { }
    protected abstract void OnConnect(Event netEvent);
    protected abstract void OnDisconnect(Event netEvent);
    protected abstract void OnTimeout(Event netEvent);
    protected abstract void OnReceive(Event netEvent);

    /// <summary>
    /// A simple function that transforms the number of bytes into a readable string. For
    /// example if bytes is 1 then "1 byte" is returned. If bytes is 2 then "2 bytes" is 
    /// returned. A empty string is returned if printing the packet size is disabled in
    /// options.
    /// </summary>
    protected string FormatByteSize(long bytes)
    {
        return Options.PrintPacketByteSize ? $"({bytes} byte{(bytes == 1 ? "" : "s")}) " : "";
    }
}
#endif
