using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script attached to the in-game pause menu UI.
/// </summary>
public class GamePauseUI : NetworkBehaviour
{
    [SerializeField] private Button mainMenuButton;
    private void Awake()
    {
        mainMenuButton.onClick.AddListener(() => {
            Cleanup();
            NetworkManager.Singleton.Shutdown();

            PersistentUpgrades.Instance.Save(); 
            Destroy(PersistentUpgrades.Instance.gameObject);

            Loader.Load(Loader.Scene.MainMenu);
        });
    }
    private void Start()
    {
        GameManager.Instance.OnLocalGamePaused += GameManager_OnLocalGamePaused;
        GameManager.Instance.OnLocalGameUnpaused += GameManager_OnLocalGameUnpaused;

        Hide();
    }

    private void GameManager_OnLocalGameUnpaused(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void GameManager_OnLocalGamePaused(object sender, System.EventArgs e)
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

    private void Cleanup()
    {
        if (!IsServer) return;
        var enemiesManager = FindObjectOfType<EnemiesManager>();
        var enemies = enemiesManager.enemyList;

        if (enemies == null) return;
        
        foreach (var gameObject in enemies)
        {
            Destroy(gameObject);
        }

        FindObjectOfType<WeaponManager>().weapons.ForEach(w => Destroy(w.gameObject));
    }
}
