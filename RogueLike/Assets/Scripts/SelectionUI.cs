using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script handling the character/level selection.
/// </summary>
public class SelectionUI : MonoBehaviour
{
    [SerializeField] private Button _mainMenuBtn;
    [SerializeField] private Button _readyBtn;
    [SerializeField] private Button _selectCharacter;
    [SerializeField] private Button _selectLevel;
    [SerializeField] private CharacterSelectPanel _characterSelectPanel;
    [SerializeField] private LevelSelectionPanel _levelSelectionPanel;
    [SerializeField] private TextMeshProUGUI _lobbyNameText;
    [SerializeField] private TextMeshProUGUI _lobbyCodeText;

    private void Awake()
    {
        _mainMenuBtn.onClick.AddListener(() =>
        {
            ElectricBloodLobby.Instance.LeaveLobby();
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenu);
        });
        _readyBtn.onClick.AddListener(() =>
        {
            SelectReady.Instance.SetPlayerReady();
        });
        _selectCharacter.onClick.AddListener(() =>
        {
            _characterSelectPanel.gameObject.SetActive(true);
        });
        _selectLevel.onClick.AddListener(() =>
        {
            _levelSelectionPanel.gameObject.SetActive(true);
        });
    }
    private void Start()
    {
        Lobby lobby = ElectricBloodLobby.Instance.GetLobby();

        _lobbyNameText.text = "Lobby Name: " + lobby.Name;
        _lobbyCodeText.text = "Lobby Code: " + lobby.LobbyCode;

        _selectLevel.gameObject.SetActive(NetworkManager.Singleton.IsServer); //level selection only for host
    }
}
