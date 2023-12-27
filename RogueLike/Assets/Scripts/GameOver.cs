using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameOver : NetworkBehaviour
{
    public GameObject gameOverPanel;
    public GameObject equipedItemsPanel;

    private void Start()
    {
        gameOverPanel = GameManager.Instance.gameOverPanel;
        equipedItemsPanel = FindObjectOfType<PlayerWinManager>().equipedItemsPanel;
    }
    public void PlayerGameOver()
    {
        GameManager.Instance.TogglePauseGame(false);
        equipedItemsPanel.SetActive(false);
        gameOverPanel.SetActive(true);
    }
}
