using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button _mainMenuBtn;
    [SerializeField] private Button _createLobbyBtn;
    [SerializeField] private Button _quickJoinBtn;
    [SerializeField] private Button _joinCodeBtn;

    [SerializeField] private TMP_InputField _joinCodeInputField;
    [SerializeField] private TMP_InputField _playerNameInputField;

    [SerializeField] private CreateLobbyUI _lobbyCreateUI;

    [SerializeField] private Transform _lobbyContainer;
    [SerializeField] private Transform _lobbyTemplate;


    private void Awake()
    {
        _mainMenuBtn.onClick.AddListener(() =>
        {
            ElectricBloodLobby.Instance.LeaveLobby();
            Loader.Load(Loader.Scene.MainMenu);
        });
        _createLobbyBtn.onClick.AddListener(() =>
        {
            _lobbyCreateUI.Show();
        });
        _quickJoinBtn.onClick.AddListener(() =>
        {
            ElectricBloodLobby.Instance.QuickJoin();
        });
        _joinCodeBtn.onClick.AddListener(() =>
        {
            ElectricBloodLobby.Instance.JoinWithCode(_joinCodeInputField.text);
        });

        _lobbyTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        _playerNameInputField.text = ElectricBloodMultiplayer.Instance.GetPlayerName();
        _playerNameInputField.onValueChanged.AddListener((string newText) => {
        ElectricBloodMultiplayer.Instance.SetPlayerName(newText);
        });

        ElectricBloodLobby.Instance.OnLobbyListChanged += KitchenGameLobby_OnLobbyListChanged;
        UpdateLobbyList(new List<Lobby>());
    }

    private void KitchenGameLobby_OnLobbyListChanged(object sender, ElectricBloodLobby.OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }

    private void UpdateLobbyList(List<Lobby> lobbyList)
    {
        foreach (Transform child in _lobbyContainer)
        {
            if (child == _lobbyTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach (Lobby lobby in lobbyList)
        {
            Transform lobbyTransform = Instantiate(_lobbyTemplate, _lobbyContainer);
            lobbyTransform.gameObject.SetActive(true);
            lobbyTransform.GetComponent<LobbyListUI>().SetLobby(lobby);
        }
    }

    private void OnDestroy()
    {
        ElectricBloodLobby.Instance.OnLobbyListChanged -= KitchenGameLobby_OnLobbyListChanged;
    }
}
