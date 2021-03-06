using System.Net.NetworkInformation;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine.Events;
using System.Text;
using UnityEngine;
using System.Net;
using System;

public struct DiscoveryResult
{
    public string RemoteServerIpAddress;
    public UInt16 RemoteServerPort;
    public string HostName;
    public uint ConnectedPlayers;
    public uint NumberOfPlayers;
    public uint Laps;
}

/// <summary>
/// 查找发现局域网中的服务器
/// </summary>
public class NetworkDiscoverer : MonoBehaviour
{
    public UnityAction<DiscoveryResult> OnServerDiscovered;

    [SerializeField] private ServerConfiguration serverConfiguration;

    private List<UdpClient> udpClients = new List<UdpClient>();

    /// <summary>
    /// 发现 验证所有端口
    /// </summary>
    public void Discover()
    {
        // Iterating over all network interfaces. Source:
        // https://stackoverflow.com/questions/1096142/broadcasting-udp-message-to-all-the-available-network-cards
        var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
        udpClients.Clear();
        foreach (var adapter in networkInterfaces)
        {
            if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                adapter == networkInterfaces[NetworkInterface.LoopbackInterfaceIndex])
            {
                if (adapter.Supports(NetworkInterfaceComponent.IPv4))
                {
                    try
                    {
                        IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                        foreach (var ua in adapterProperties.UnicastAddresses)
                        {
                            if (ua.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                IPEndPoint localAddress = new IPEndPoint(ua.Address, 0);
                                UdpClient udpClient = new UdpClient(localAddress);
                                udpClient.EnableBroadcast = true;
                                udpClients.Add(udpClient);
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        Debug.Log($"发现网络适配器:{udpClients.Count}个, 发送验证请求");
        byte[] request = Encoding.UTF8.GetBytes(serverConfiguration.lanDiscoveryRequest);

        foreach (var udpClient in udpClients)
        {
            udpClient.Send(request, request.Length, "255.255.255.255", serverConfiguration.listenerPort);
        }
    }

    /// <summary>
    /// 等待 回复数据
    /// </summary>
    private void Update()
    {
        foreach (var udpClient in udpClients)
        {
            if (udpClient.Available > 0)
            {
                IPEndPoint remoteEndpoint = new IPEndPoint(IPAddress.Any, 0);

                string receivedMessage = Encoding.UTF8.GetString(udpClient.Receive(ref remoteEndpoint));

                Debug.Log("Received " + receivedMessage + " from address: " + remoteEndpoint.Address);

                string[] receivedMessageWords = receivedMessage.Split(' ');

                if (receivedMessageWords.Length >= 6 && receivedMessageWords[0].Equals(serverConfiguration.lanDiscoveryResponse))
                {
                    Debug.Log("Server discovered on LAN. Address: " + remoteEndpoint.Address);

                    OnServerDiscovered?.Invoke(new DiscoveryResult
                    {
                        RemoteServerIpAddress = remoteEndpoint.Address.ToString(),
                        RemoteServerPort = Convert.ToUInt16(receivedMessageWords[1]),
                        HostName = receivedMessageWords[5],
                        ConnectedPlayers = Convert.ToUInt32(receivedMessageWords[2]),
                        NumberOfPlayers = Convert.ToUInt32(receivedMessageWords[3]),
                        Laps = Convert.ToUInt32(receivedMessageWords[4])
                    });
                }
            }
        }
    }

    private void OnDisable()
    {
        foreach (var udpClient in udpClients)
        {
            udpClient.Close();
        }
    }
}
