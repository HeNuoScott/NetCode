using System;

public static class GameSession
{
    public static ServerSession serverSession = null;
    public static ClientSession clientSession = null;
}

public class ServerSession
{
    public UInt16 serverPort;
    public string hostName;
    public uint numberOfPlayers;
    public uint laps;
}

public class ClientSession
{
    public string remoteServerIpAddress;
    public UInt16 remoteServerPort;
}

/// <summary> 发现结果 </summary>
public struct DiscoveryResult
{
    public string HostName;
    public UInt16 RemoteServerPort;
    public string RemoteServerIpAddress;
}
/// <summary> 网络身份 </summary>
public struct NetworkIdentity
{
    public string Name;
    public string IpAddress;
}