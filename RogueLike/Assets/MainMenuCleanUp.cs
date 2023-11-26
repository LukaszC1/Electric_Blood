using Unity.Netcode;
using UnityEngine;

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