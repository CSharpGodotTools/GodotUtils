﻿namespace GodotUtils.Netcode;

public abstract class ENetLow
{
	public abstract void Log(object message, ConsoleColor color);
	public abstract void Stop();

	protected CancellationTokenSource CTS { get; set; }
	protected abstract void DisconnectCleanup();
}
