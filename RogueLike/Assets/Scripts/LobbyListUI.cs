using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Single lobby UI element on which we can click to join the lobby.
/// </summary>
public class LobbyListUI : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI _lobbyNameText;
    private Lobby _lobby;

    /// <summary>
    /// Setter for the lobby to join to and the name of the lobby.
    /// </summary>
    /// <param name="lobby"></param>
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
