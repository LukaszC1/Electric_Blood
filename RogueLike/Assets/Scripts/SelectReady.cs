using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SelectReady : MonoBehaviour
{
    public static SelectReady Instance { get; private set; }
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

        bool allPlayersReady = true;

        foreach(var clientId in _readyDict.Keys)
        {
            if (!_readyDict[clientId])
            {
                // If any player is not ready, then all players are not ready.
                allPlayersReady = false;
                break;
            }
        }
        if (allPlayersReady)
        {
            ElectricBloodLobby.Instance.DeleteLobby();
            Loader.Load(Loader.Scene.GameScene);
        }
    }

    [ClientRpc]
    private void SetPlayerReadyClientRpc(ulong clientId)
    {
        _readyDict[clientId] = true;
        OnReadyChanged?.Invoke(this, EventArgs.Empty);
    }
}
