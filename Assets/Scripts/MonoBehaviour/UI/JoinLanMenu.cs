using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinLanMenu : MonoBehaviour, IMenu
{
    [SerializeField] private MenuManager menuManager;
    [SerializeField] private ServerConfiguration serverConfiguration;
    [SerializeField] private NetworkDiscoverer networkDiscoverer;

    [SerializeField] private Button refreshButton;
    [SerializeField] private Button cancelButton;

    [SerializeField] private ServerList serverList;
    
    public void Init()
    {
        refreshButton.onClick.AddListener(RefreshServerList);

        cancelButton.onClick.AddListener(() => menuManager.SelectMenu<JoinGameMenu>());

        networkDiscoverer.OnServerDiscovered += (DiscoveryResult discoveryResult) =>
        {
            serverList.AddElement(discoveryResult);
        };
    }

    public void Enter()
    {
        gameObject.SetActive(true);
        networkDiscoverer.Discover();
    }

    public void Exit()
    {
        gameObject.SetActive(false);
    }

    private void RefreshServerList()
    {
        serverList.Clear();
        networkDiscoverer.Discover();
    }
}
