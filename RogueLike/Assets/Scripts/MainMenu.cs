using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    PauseManager pauseManager;
    
        
    
    void Awake()
    {
        pauseManager = GetComponent<PauseManager>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(mainMenu.activeInHierarchy == false)
            OpenMenu();
            else
                CloseMenu();
        }
    }

    public void CloseMenu()
    {
        pauseManager.UnPauseGame();
        mainMenu.SetActive(false);
    }

    public void OpenMenu()
    {
        pauseManager.PauseGame();
        mainMenu.SetActive(true);
    }
}
