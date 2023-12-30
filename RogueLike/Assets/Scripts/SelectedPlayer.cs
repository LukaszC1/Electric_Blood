using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.UI;

public class SelectedPlayer : NetworkBehaviour
{
    [SerializeField] private int playerIndex;
    [SerializeField] private Image playerVisual;
    [SerializeField] private Button kickBtn;
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI readyText;

    private void Awake()
    {
        kickBtn.onClick.AddListener(() =>
        {
            PlayerData playerData = ElectricBloodMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            ElectricBloodLobby.Instance.KickPlayer(playerData.playerId.ToString());
            ElectricBloodMultiplayer.Instance.KickPlayer(playerData.clientId);
        });
    }

    private void Start()
    {
        SelectReady.Instance.OnReadyChanged += Instance_OnReadyChanged;
        kickBtn.gameObject.SetActive(NetworkManager.Singleton.IsServer);
        UpdatePlayer();
        
        var playerData = ElectricBloodMultiplayer.Instance.GetPlayerDataFromClientId(NetworkManager.LocalClientId);
        if (playerData.playerName.Equals(playerName.text))
        {
            kickBtn.gameObject.SetActive(false); //disable kick button for the player owned by host
        }

    }

    private void Instance_OnReadyChanged(object sender, EventArgs e)
    {
       UpdatePlayer();
    }

    private void Instance_OnPlayerDataNetworkListChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }

    private void UpdatePlayer()
    {
       if(ElectricBloodMultiplayer.Instance.IsPlayerIndexConnected(playerIndex))
        {
            Show();

            PlayerData playerData = ElectricBloodMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);

            readyText.color = SelectReady.Instance.IsPlayerReady(playerData.clientId) ? Color.green : Color.red;

            playerName.text = playerData.playerName.ToString();

            var selectedCharacter = ElectricBloodMultiplayer.Instance.availableCharacters[playerData.characterIndex] as CharacterData;
            playerVisual.sprite = selectedCharacter.characterSprite;
        }
       else
            Hide();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
  
