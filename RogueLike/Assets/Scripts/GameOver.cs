using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameOver : NetworkBehaviour
{
    public GameObject gameOverPanel;
    public GameObject equipedItemsPanel;

    PauseManager pauseManager;

    private void Start()
    {
        pauseManager = FindObjectOfType<PauseManager>();
    }

    public void PlayerGameOver()
    {
        equipedItemsPanel.SetActive(false);
        gameOverPanel.SetActive(true);
        pauseManager.PauseGame();
    }
}
