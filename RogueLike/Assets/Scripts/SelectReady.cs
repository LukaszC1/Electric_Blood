using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Class which handles the ready state of the players in the lobby.
/// </summary>
public class SelectReady : NetworkBehaviour
{
    /// <summary>
    /// Singleton instance.
    /// </summary>
    public static SelectReady Instance { get; private set; }

    /// <summary>
    /// Event which is invoked when the ready state of the player changes.
    /// </summary>
    public event EventHandler OnReadyChanged;

    private Dictionary<ulong, bool> _readyDict;

    private void Awake()
    {
        Instance = this;
        _readyDict = new();
    }

    /// <summary>
    /// Method which returns true/false if the player with a given clientId is ready.
    /// </summary>
    /// <param name="clientId"></param>
    /// <returns></returns>
    public bool IsPlayerReady(ulong clientId)
    {
        return _readyDict.ContainsKey(clientId) && _readyDict[clientId];
    }

    /// <summary>
    /// Methods which changes the ready state of the player.
    /// </summary>
    public void SetPlayerReady()
    {
        SetPlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {

        var senderClientId = serverRpcParams.Receive.SenderClientId;

        SetPlayerReadyClientRpc(senderClientId);
        _readyDict[senderClientId] = true;

        bool allClientsReady = true;

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!_readyDict.ContainsKey(clientId) || !_readyDict[clientId])
            {
                // This player is NOT ready
                allClientsReady = false;
                break;
            }
        }

        if (allClientsReady)
        {
            ElectricBloodLobby.Instance.DeleteLobby();
            Loader.LoadNetwork(Loader.Scene.GameScene);
        }
    }

    [ClientRpc]
    private void SetPlayerReadyClientRpc(ulong clientId)
    {
        _readyDict[clientId] = true;
        OnReadyChanged?.Invoke(this, EventArgs.Empty);
    }
}
