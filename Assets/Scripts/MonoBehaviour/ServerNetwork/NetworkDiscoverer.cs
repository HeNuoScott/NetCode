using System.Net.NetworkInformation;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine.Events;
using System.Text;
using UnityEngine;
using System.Net;
using System;

/// <summary>
/// 查找发现局域网中的服务器
/// </summary>
public class NetworkDiscoverer : MonoBehaviour
{
    public UnityAction<DiscoveryResult> OnServerDiscovered;

    [SerializeField] private ServerConfiguration serverConfiguration;

    private List<UdpClient> udpClients = new List<UdpClient>();

    private byte[] request;

    /// <summary>
    /// 初始化 查找所有端口
    /// </summary>
    public void Init()
    {
        request = Encoding.ASCII.GetBytes(serverConfiguration.lanDiscoveryRequest);

        // Iterating over all network interfaces. Source:
        // https://stackoverflow.com/questions/1096142/broadcasting-udp-message-to-all-the-available-network-cards

        var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

        foreach (var adapter in networkInterfaces)
        {
            if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet || adapter == networkInterfaces[NetworkInterface.LoopbackInterfaceIndex])
            {
                if (adapter.Supports(NetworkInterfaceComponent.IPv4))
                {
                    try
                    {
                        IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                        foreach (var ua in adapterProperties.UnicastAddresses)
                        {
                            if (ua.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
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
    }

    /// <summary>
    /// 发现 验证所有端口
    /// </summary>
    public void Discover()
    {
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
                string receivedMessage = Encoding.ASCII.GetString(udpClient.Receive(ref remoteEndpoint));

                Debug.Log("Received " + receivedMessage + " from address: " + remoteEndpoint.Address);

                string[] receivedMessageWords = receivedMessage.Split(' ');

                if (receivedMessageWords.Length >= 3 && receivedMessageWords[0].Equals(serverConfiguration.lanDiscoveryResponse))
                {
                    Debug.Log("Server discovered on LAN. Address: " + remoteEndpoint.Address);

                    OnServerDiscovered?.Invoke(new DiscoveryResult
                    {
                        RemoteServerIpAddress = remoteEndpoint.Address.ToString(),
                        RemoteServerPort = Convert.ToUInt16(receivedMessageWords[2]),
                        HostName = receivedMessageWords[1],
                    });
                }
            }
        }
    }

    private void OnDestroy()
    {
        foreach (var udpClient in udpClients)
        {
            udpClient.Close();
        }
    }
}
