using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;


    private void Awake()
    {
        closeButton.onClick.AddListener(Hide);
    }

    private void Start()
    {
        ElectricBloodMultiplayer.Instance.OnFailedToJoinGame += ElectricBlood_OnFailedToJoinGame;
        ElectricBloodLobby.Instance.OnCreateLobbyStarted += ElectricBlood_OnCreateLobbyStarted;
        ElectricBloodLobby.Instance.OnCreateLobbyFailed += ElectricBlood_OnCreateLobbyFailed;
        ElectricBloodLobby.Instance.OnJoinStarted += ElectricBlood_OnJoinStarted;
        ElectricBloodLobby.Instance.OnJoinFailed += ElectricBlood_OnJoinFailed;
        ElectricBloodLobby.Instance.OnQuickJoinFailed += ElectricBlood_OnQuickJoinFailed;

        Hide();
    }

    private void ElectricBlood_OnQuickJoinFailed(object sender, System.EventArgs e)
    {
        ShowMessage("Could not Quick Join!");
    }

    private void ElectricBlood_OnJoinFailed(object sender, System.EventArgs e)
    {
        ShowMessage("Failed to join!");
    }

    private void ElectricBlood_OnJoinStarted(object sender, System.EventArgs e)
    {
        ShowMessage("Joining the lobby...");
    }

    private void ElectricBlood_OnCreateLobbyFailed(object sender, System.EventArgs e)
    {
        ShowMessage("Failed to create Lobby!");
    }

    private void ElectricBlood_OnCreateLobbyStarted(object sender, System.EventArgs e)
    {
        ShowMessage("Creating the lobby...");
    }

    private void ElectricBlood_OnFailedToJoinGame(object sender, System.EventArgs e)
    {
        if (NetworkManager.Singleton.DisconnectReason == "")
        {
            ShowMessage("Failed to connect.");
        }
        else
        {
            ShowMessage(NetworkManager.Singleton.DisconnectReason);
        }
    }

    private void ShowMessage(string message)
    {
        Show();
        messageText.text = message;
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
        ElectricBloodMultiplayer.Instance.OnFailedToJoinGame += ElectricBlood_OnFailedToJoinGame;
         ElectricBloodLobby.Instance.OnCreateLobbyStarted += ElectricBlood_OnCreateLobbyStarted;
         ElectricBloodLobby.Instance.OnCreateLobbyFailed += ElectricBlood_OnCreateLobbyFailed;
         ElectricBloodLobby.Instance.OnJoinStarted += ElectricBlood_OnJoinStarted;
         ElectricBloodLobby.Instance.OnJoinFailed += ElectricBlood_OnJoinFailed;
         ElectricBloodLobby.Instance.OnQuickJoinFailed += ElectricBlood_OnQuickJoinFailed;
    }

}