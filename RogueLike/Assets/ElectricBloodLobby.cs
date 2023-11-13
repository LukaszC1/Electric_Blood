using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class ElectricBloodLobby : MonoBehaviour
{
    public static ElectricBloodLobby Instance { get; private set; }

    private Lobby currentLobby;

    private void Awake()
    {
        Instance = this;

        InitializeUnityAuthentication();

        DontDestroyOnLoad(gameObject);
    }

    private async void InitializeUnityAuthentication()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(Random.Range(0,10000).ToString());

            await UnityServices.InitializeAsync(initializationOptions);

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    /// <summary>
    /// Function handling creating a lobby private/public with a given name.
    /// </summary>
    /// <param name="lobbyName"></param>
    /// <param name="isPrivate"></param>
    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        try {
            currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, 4, new CreateLobbyOptions
            {
                IsPrivate = isPrivate
            }) ;

        } catch(LobbyServiceException e)
            {
                Debug.Log(e);
            }
    }

    /// <summary>
    /// Function handling quickjoin.
    /// </summary>
    public async void QuickJoin()
    {
        try
        {
            currentLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
       
    }
}
