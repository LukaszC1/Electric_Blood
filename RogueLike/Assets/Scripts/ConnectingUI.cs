using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingUI : MonoBehaviour
{
    private void Start()
    {
        ElectricBloodMultiplayer.Instance.OnTryingToJoinGame += ElectricBlood_OnTryingToJoinGame;
        ElectricBloodMultiplayer.Instance.OnFailedToJoinGame += ElectricBlood_OnFailedToJoinGame;

        Hide();
    }

    private void ElectricBlood_OnFailedToJoinGame(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void ElectricBlood_OnTryingToJoinGame(object sender, System.EventArgs e)
    {
        Show();
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
        ElectricBloodMultiplayer.Instance.OnTryingToJoinGame -= ElectricBlood_OnTryingToJoinGame;
        ElectricBloodMultiplayer.Instance.OnFailedToJoinGame -= ElectricBlood_OnFailedToJoinGame;
    }

}