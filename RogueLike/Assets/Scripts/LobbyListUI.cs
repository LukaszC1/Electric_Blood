using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyListUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _lobbyNameText;
    private Lobby _lobby;

    public void SetLobby(Lobby lobby)
    {
        _lobby = lobby;
        _lobbyNameText.text = lobby.Name;
    }

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => {
        ElectricBloodLobby.Instance.JoinWithId(_lobby.Id);
    });
    }
}
