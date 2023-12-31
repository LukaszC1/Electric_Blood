using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Script handling the Game Over state.
/// </summary>
public class GameOver : NetworkBehaviour
{
    /// <summary>
    /// Reference to the Game Over panel.
    /// </summary>
    public GameObject gameOverPanel;

    /// <summary>
    /// Reference to the equiped items panel.
    /// </summary>
    public GameObject equipedItemsPanel;

    private void Start()
    {
        gameOverPanel = GameManager.Instance.gameOverPanel;
        equipedItemsPanel = FindObjectOfType<PlayerWinManager>().equipedItemsPanel;
    }

    /// <summary>
    /// Method invoking the Game Over state.
    /// </summary>
    public void PlayerGameOver()
    {
        GameManager.Instance.TogglePauseGame(false);
        equipedItemsPanel.SetActive(false);
        gameOverPanel.SetActive(true);
    }
}
