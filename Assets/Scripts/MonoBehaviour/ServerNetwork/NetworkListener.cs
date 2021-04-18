using System.Net.Sockets;
using UnityEngine;
using System.Text;
using System.Net;

/// <summary>
/// 服务器端 监听udp申请  
/// 收到申请后 反馈服务器信息
/// </summary>
public class NetworkListener : MonoBehaviour
{
    [SerializeField] private ServerConfiguration serverConfiguration;

    private UdpClient listener;

    private void Start()
    {
        listener = new UdpClient(serverConfiguration.listenerPort);
    }

    private void Update()
    {
        if (listener.Available > 0)
        {
            IPEndPoint remoteEndpoint = new IPEndPoint(IPAddress.Any, 0);
            string receivedMessage = Encoding.ASCII.GetString(listener.Receive(ref remoteEndpoint));

            Debug.Log("Received " + receivedMessage + " from address: " + remoteEndpoint.Address);

            byte[] response = Encoding.ASCII.GetBytes(serverConfiguration.lanDiscoveryResponse + " " + serverConfiguration.lanHostName + " " + serverConfiguration.listenerPort);

            if (receivedMessage.Equals(serverConfiguration.lanDiscoveryRequest))
            {
                listener.Send(response, response.Length, remoteEndpoint);
            }
        }
    }

    private void OnDestroy()
    {
        listener.Close();
    }
}