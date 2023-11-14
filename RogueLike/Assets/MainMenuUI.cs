using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button singleplayerButton;
    [SerializeField] private Button multiplayerButton;
    [SerializeField] private Button quitButton;

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
    }
}
