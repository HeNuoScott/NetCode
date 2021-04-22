using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour, IMenu
{
    [SerializeField] private MenuManager menuManager;

    [SerializeField] private Button hostGameButton;
    [SerializeField] private Button joinGameButton;
    [SerializeField] private Button exitButton;

    public void Init()
    {
        hostGameButton.onClick.AddListener(() =>
        {
            menuManager.SelectMenu<HostGameMenu>();
        });

        joinGameButton.onClick.AddListener(() =>
        {
            menuManager.SelectMenu<JoinGameMenu>();
        });

        exitButton.onClick.AddListener(() =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
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
