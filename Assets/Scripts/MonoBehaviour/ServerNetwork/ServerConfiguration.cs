using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New ServerConfiguration", menuName = "Server Configuration")]
public class ServerConfiguration : ScriptableObject
{
    /// <summary> 服务器主机名称 </summary>
    public string lanHostName;
    /// <summary> 监听端口 </summary>
    public UInt16 listenerPort;
    /// <summary> 网络发现请求 数据 </summary>
    public string lanDiscoveryRequest;
    /// <summary> 网络请求响应 数据 </summary>
    public string lanDiscoveryResponse;
}