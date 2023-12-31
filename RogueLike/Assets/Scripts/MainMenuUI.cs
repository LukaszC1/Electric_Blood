using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script attached to the main menu UI.
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button singleplayerButton;
    [SerializeField] private Button multiplayerButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button shopButton;
    [SerializeField] private GameObject shopPanel;

    private void Awake()
    {
        singleplayerButton.onClick.AddListener(()=>
        {
            ElectricBloodMultiplayer.playMultiplayer = false;
            Loader.Load(Loader.Scene.LobbyScene);
        });

        multiplayerButton.onClick.AddListener(() =>
        {
            ElectricBloodMultiplayer.playMultiplayer = true;
            Loader.Load(Loader.Scene.LobbyScene);
        });

        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        shopButton.onClick.AddListener(() =>
        {
            shopPanel.SetActive(true);
        });
    }
}
