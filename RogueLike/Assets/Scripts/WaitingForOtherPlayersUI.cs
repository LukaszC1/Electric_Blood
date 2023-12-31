using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script attached to WaitingForOtherPlayersUI screen.
/// </summary>
public class WaitingForOtherPlayersUI : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Method which changes the visibility of the screen.
    /// </summary>
    public void ChangeVisibility()
    {
        if(gameObject.activeInHierarchy)
            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);
    }
}
