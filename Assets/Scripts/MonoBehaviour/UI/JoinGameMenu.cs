using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinGameMenu : MonoBehaviour, IMenu
{
    [SerializeField] private MenuManager menuManager;

    [SerializeField] private Button lanButton;
    [SerializeField] private Button onlineButton;
    [SerializeField] private Button cancelButton;
    
    public void Init()
    {
        lanButton.onClick.AddListener(() =>
        {
            menuManager.SelectMenu<JoinLanMenu>();
        });
        
        onlineButton.onClick.AddListener(() =>
        {
            menuManager.SelectMenu<JoinOnlineMenu>();
        });
        
        cancelButton.onClick.AddListener(() =>
        {
            menuManager.SelectMenu<MainMenu>();
        });
    }

    public void Enter()
    {
        gameObject.SetActive(true);
    }

    public void Exit()
    {
        gameObject.SetActive(false);
    }
}
