using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWinManager : MonoBehaviour
{
    [SerializeField] GameObject winPanel;
    public GameObject equipedItemsPanel;

    public void Win()
    {
        equipedItemsPanel.SetActive(false);
        winPanel.SetActive(true);      
    }
}
