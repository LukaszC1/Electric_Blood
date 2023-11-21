using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestingUI : MonoBehaviour
{
    [SerializeField] private Button createGameButton;
    [SerializeField] private Button joinGameButton;

    private void Awake()
    {
        createGameButton.onClick.AddListener(() => {
            ElectricBloodMultiplayer.Instance.StartHost();
            Loader.LoadNetwork(Loader.Scene.CharacterSelection);
        });
        joinGameButton.onClick.AddListener(() => {
            ElectricBloodMultiplayer.Instance.StartClient();
        });
    }
}
