using UnityEngine.SceneManagement;
using System.Net.Sockets;
using UnityEngine.UI;
using UnityEngine;
using System.Text;
using System.Net;
using System;

public class TestNetworkListener : MonoBehaviour
{
    [SerializeField] private ServerConfiguration serverConfiguration;

    private UdpClient listener;

    public Button button_Return = null;

    private void Start()
    {
        listener = new UdpClient(serverConfiguration.listenerPort);
        if (button_Return != null)
        {
            button_Return.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
            });
        }
    }

    private void Update()
    {
        if (listener.Available > 0)
        {
            IPEndPoint remoteEndpoint = new IPEndPoint(IPAddress.Any, 0);
            string receivedMessage = Encoding.UTF8.GetString(listener.Receive(ref remoteEndpoint));

            Debug.Log("Received " + receivedMessage + " from address: " + remoteEndpoint.Address);

            byte[] response = Encoding.UTF8.GetBytes(serverConfiguration.lanDiscoveryResponse + " " + (UInt16)7979 + " " + 2 + " " + 3 + " " + 4 + " " + "host");

            if (receivedMessage.Equals(serverConfiguration.lanDiscoveryRequest))
            {
                listener.Send(response, response.Length, remoteEndpoint);
                Debug.Log($"Response to address: {remoteEndpoint.Address}: {remoteEndpoint.Port}" );
            }
        }
    }

    private void OnDestroy()
    {
        listener.Close();
    }
}
