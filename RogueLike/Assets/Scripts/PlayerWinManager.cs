using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that manages the player win.
/// </summary>
public class PlayerWinManager : MonoBehaviour
{
    /// <summary>
    /// Reference to the equipedItemsPanel.
    /// </summary>
    public GameObject equipedItemsPanel;

    [SerializeField] GameObject winPanel;

    /// <summary>
    /// Method that is called when the player wins displaying the winPanel.
    /// </summary>
    public void Win()
    {
        equipedItemsPanel.SetActive(false);
        winPanel.SetActive(true);      
    }
}
