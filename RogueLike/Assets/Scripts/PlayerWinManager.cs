using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWinManager : MonoBehaviour
{
    [SerializeField] GameObject winPanel;
    public GameObject equipedItemsPanel;
    //PauseManager pauseManager; todo fix

    private void Start()
    {
        //pauseManager = GetComponent<PauseManager>(); todo fix
    }

    public void Win()
    {
        equipedItemsPanel.SetActive(false);
        winPanel.SetActive(true);
        //pauseManager.PauseGame();
    }
}
