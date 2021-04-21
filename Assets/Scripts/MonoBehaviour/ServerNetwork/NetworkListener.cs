using System.Net.Sockets;
using Unity.Entities;
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

    private GoInGameServerSystem goInGameServerSystem;

    private void Start()
    {
        listener = new UdpClient(serverConfiguration.listenerPort);

        foreach (var world in World.All)
        {
            var goInGameServerSystem = world.GetExistingSystem<GoInGameServerSystem>();
            if (goInGameServerSystem != null)
            {
                this.goInGameServerSystem = goInGameServerSystem;
                break;
            }
        }
    }

    private void Update()
    {
        if (listener.Available > 0)
        {
            IPEndPoint remoteEndpoint = new IPEndPoint(IPAddress.Any, 0);
            string receivedMessage = Encoding.ASCII.GetString(listener.Receive(ref remoteEndpoint));

            Debug.Log("Received " + receivedMessage + " from address: " + remoteEndpoint.Address);

            byte[] response = Encoding.ASCII.GetBytes(serverConfiguration.lanDiscoveryResponse + " " + GameSession.serverSession.serverPort + " " + goInGameServerSystem.connectedPlayers + " " + GameSession.serverSession.numberOfPlayers + " " + GameSession.serverSession.laps + " " + GameSession.serverSession.hostName);

            if (goInGameServerSystem.connectedPlayers < GameSession.serverSession.numberOfPlayers && receivedMessage.Equals(serverConfiguration.lanDiscoveryRequest))
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