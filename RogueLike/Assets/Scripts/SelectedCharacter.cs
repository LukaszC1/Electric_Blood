using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SelectedCharacter : MonoBehaviour
{
    [SerializeField] private int _playerIndex;
    [SerializeField] private GameObject _readyGameObject;
    [SerializeField] private Button _kickButton;
    [SerializeField] private TextMeshPro _playerNameText;


    private void Awake()
    {
        _kickButton.onClick.AddListener(() => {
            PlayerData playerData = ElectricBloodMultiplayer.Instance.GetPlayerDataFromPlayerIndex(_playerIndex);
            ElectricBloodLobby.Instance.KickPlayer(playerData.playerId.ToString());
            ElectricBloodMultiplayer.Instance.KickPlayer(playerData.clientId);
        });
    }

    private void Start()
    {
        ElectricBloodMultiplayer.Instance.OnPlayerDataNetworkListChanged += KitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
        SelectReady.Instance.OnReadyChanged += CharacterSelectReady_OnReadyChanged;

        _kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);

        UpdatePlayer();
    }

    private void CharacterSelectReady_OnReadyChanged(object sender, System.EventArgs e)
    {
        UpdatePlayer();
    }

    private void KitchenGameMultiplayer_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e)
    {
        UpdatePlayer();
    }

    private void UpdatePlayer()
    {
        if (ElectricBloodMultiplayer.Instance.IsPlayerIndexConnected(_playerIndex))
        {
            Show();

            PlayerData playerData = ElectricBloodMultiplayer.Instance.GetPlayerDataFromPlayerIndex(_playerIndex);

            _readyGameObject.SetActive(SelectReady.Instance.IsPlayerReady(playerData.clientId));

            _playerNameText.text = playerData.playerName.ToString();

            //update any other player visuals here
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        ElectricBloodMultiplayer.Instance.OnPlayerDataNetworkListChanged -= KitchenGameMultiplayer_OnPlayerDataNetworkListChanged;
    }

}
