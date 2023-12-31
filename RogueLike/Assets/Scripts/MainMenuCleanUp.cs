using System.Linq;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Cleans up the classes that have the DontDestroyOnLoad attribute.
/// </summary>
public class MainMenuCleanUp : MonoBehaviour
{
    private void Awake()
    {
        if (NetworkManager.Singleton != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }

        if (ElectricBloodMultiplayer.Instance != null)
        {
            Destroy(ElectricBloodMultiplayer.Instance.gameObject);
        }

        if (ElectricBloodLobby.Instance != null)
        {
            Destroy(ElectricBloodLobby.Instance.gameObject);
        }
    }
}