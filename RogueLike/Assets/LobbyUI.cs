using System.Collections;
using System.Collections.Generic;
using TMPro;
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
}
