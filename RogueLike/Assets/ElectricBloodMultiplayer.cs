using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// This class is used to store the multiplayer/singleplayer settings.
/// </summary>

public class ElectricBloodMultiplayer : NetworkBehaviour
{
    private const int MAX_PLAYERS = 4;
    private const string PLAYER_PREFS = "PlayerName";

    public static ElectricBloodMultiplayer Instance { get; private set; }

    public static bool playMultiplayer = false;

    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;
    public event EventHandler OnPlayerDataNetworkListChanged;

    [SerializeField] private List<GameObject> playerCharacters; // list of available characters

    private NetworkList<PlayerData> playerDataNetworkList;
    private string playerName;
}