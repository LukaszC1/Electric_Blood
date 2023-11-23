using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameOver : NetworkBehaviour
{
    public GameObject gameOverPanel;
    public GameObject equipedItemsPanel;

    //PauseManager pauseManager; todo : fix this

    private void Start()
    {
        //pauseManager = FindObjectOfType<PauseManager>(); todo : fix this
    }

    public void PlayerGameOver()
    {
        equipedItemsPanel.SetActive(false);
        gameOverPanel.SetActive(true);
        //pauseManager.PauseGame();
    }
}
